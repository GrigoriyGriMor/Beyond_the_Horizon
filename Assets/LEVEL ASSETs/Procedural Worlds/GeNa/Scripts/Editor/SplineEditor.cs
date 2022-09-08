using System.Collections.Generic;
using System.Linq;
// Engine
using UnityEngine;
// Editor
using UnityEditor;
using UnityEditorInternal;
// Procedural Worlds
using PWCommon5;
using Object = UnityEngine.Object;
namespace GeNa.Core
{
    [CustomEditor(typeof(Spline))]
    public class SplineEditor : GeNaEditor
    {
        #region Static
        // Colors
        private static Color SPLINE_CURVE_COLOR = new Color(0.8f, 0.8f, 0.8f);
        private static Color SPLINE_SELECTED_CURVE_COLOR = Color.green; //new Color(0.8f, 0.8f, 0.8f);
        private static Color NODE_COLOR = new Color(0.6f, 0.6f, 0.6f);
        private static Color FIRST_NODE_COLOR = new Color(0.0f, 0.8f, 0.0f);
        private static Color LAST_NODE_COLOR = new Color(0.0f, 0.0f, 0.8f);
        private static Color EXTRUSION_CURVE_COLOR = new Color(0.8f, 0.8f, 0.8f);
        private static Color DIRECTION_BUTTON_COLOR = Color.blue; //Color.red;
        private static Color TANGENT_LINE_COLOR = Color.blue;
        private static Color UP_BUTTON_COLOR = Color.green;
        // Panels
        private static bool m_showQuickStart = true;
        private static bool m_showOverview = true;
        private static bool m_showPathFinding = false;
        private static bool m_showExtensions = true;
        private static bool m_showAdvancedSettings = false;
        #endregion
        #region Definitions
        public enum SelectionType
        {
            Node,
            StartTangent,
            EndTangent,
            Up,
            Scale
        }
        #endregion
        #region Variables
        #region Static
        private static int SPLINE_QUAD_SIZE = 25;
        private static int SPLINE_STYLE_QUAD_SIZE = 15;
        private static int EXTRUSION_QUAD_SIZE = 25;
        private static bool showUpVector = false;
        #endregion
        #region GUI
        private ReorderableList m_extensionReorderable;
        private GeNaSplineExtension m_selectedExtension;
        private ExtensionEntry m_selectedExtensionEntry;
        private GeNaSplineExtensionEditor m_selectedExtensionEditor;
        private Tool m_previousTool;

        // Switch to drop custom ground level for ingestion
        private bool m_dropGround = false;
        private bool m_splineDirty = false;
        #endregion
        #region Spline
        // Core
        private GeNaSpline m_spline;
        private SplineSettings m_settings;
        private GeNaNode m_selectedNode;
        private GeNaCurve _selectedGeNaCurve;
        private int m_selectedVertex = -1;
        private SelectionType m_selectionType;
        private float m_mouseDragThreshold = .1f;
        private bool m_splineModified = false;
        private Vector2 m_mouseClickPoint = Vector2.zero;
        #endregion
        #endregion
        #region Node Selection Undo/Redo
        private bool IsDifferent(List<Vector3> right, List<Vector3> left)
        {
            if (right == null && left == null)
                return false;
            if (right == null || left == null)
                return true;
            if (right.Count != left.Count)
                return true;
            int count = right.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 a = right[i];
                Vector3 b = left[i];
                if (a != b)
                    return true;
            }
            return false;
        }
        #endregion
        #region Methods
        #region Unity
        private void CheckHierarchyChanged()
        {
        }
        private void DrawCurve(GeNaCurve geNaCurve, Color color)
        {
            // Default Bezier
            Handles.DrawBezier(geNaCurve.P0, geNaCurve.P3, geNaCurve.P1, geNaCurve.P2, color, null, 2);
        }
        private void OnUndoProcessed()
        {
            m_spline = target as GeNaSpline;
            m_selectedNode = null;
            DeselectAllExtensionEntries();
        }
        protected void OnEnable()
        {
            Undo.undoRedoPerformed -= OnUndoProcessed;
            Undo.undoRedoPerformed += OnUndoProcessed;
            if (m_editorUtils == null)
                // If there isn't any Editor Utils Initialized
                m_editorUtils = PWApp.GetEditorUtils(this, null, null, null);
            #region Initialization
            // Get target Spline
            m_spline = target as GeNaSpline;
            m_splineDirty = true;
            // if (m_spline != null)
            // {
            //     ReconnectSpline();
            // }
            m_settings = m_spline.Settings;
            CheckHierarchyChanged();
            // Subscribe Refresh Curves to Undo
            //TODO : Manny : Re-register Undo!
            //Undo.undoRedoPerformed -= m_spline.RefreshCurves;
            //Undo.undoRedoPerformed += m_spline.RefreshCurves;
            //Hide its m_transform
            // Create the Extension List
            CreateExtensionList();
            #endregion
            m_spline.OnSubscribe();
            m_spline.transform.hideFlags = HideFlags.HideInInspector;
            Tools.hidden = true;
        }
        protected void OnDisable()
        {
            m_splineDirty = false;
            m_selectedNode = null;
            m_spline.OnUnSubscribe();
            Tools.hidden = false;
            GeNaEvents.Destroy(GeNaSpawnerInternal.TempGameObject);
            DeselectAllExtensionEntries();
        }
        public override void OnSceneGUI()
        {
            if (m_splineDirty)
            {
                SelectExtensionEntry(m_spline.SelectedExtensionIndex);
                m_splineDirty = false;
            }
            m_spline.OnSceneGUI();
            if (m_selectedExtensionEditor != null)
                m_selectedExtensionEditor.OnSceneGUI();
            if (m_settings.Advanced.DebuggingEnabled)
                DrawDebug();
            Initialize();
            #region Events
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            bool mouseUp = false;
            switch (e.type)
            {
                case EventType.MouseDown:
                {
                    // Spline
                    //TODO : Manny : Re-register Undo!
                    //Undo.RegisterCompleteObjectUndo(m_spline, "change spline or extrusion topography");
                    m_mouseClickPoint = e.mousePosition;
                    break;
                }
                case EventType.MouseUp:
                {
                    mouseUp = true;
                    break;
                }
                case EventType.KeyDown:
                    // If the Delete Key is Pressed
                    if (e.keyCode == KeyCode.Delete)
                    {
                        //TODO : Manny : Re-register Undo!
                        //Undo.RegisterCompleteObjectUndo(m_spline, "delete");
                        // Check if a vertex is selected
                        if (m_selectedVertex >= 0)
                        {
                            e.Use();
                            break;
                        }
                        // Check if a Node is Selected
                        if (m_selectedNode != null)
                        {
                            if (m_spline.Nodes.Contains(m_selectedNode))
                            {
                                var selected = m_selectedNode;
                                m_spline.RecordUndoSnapshot("Remove Node", () =>
                                {
                                    m_spline.RemoveNode(selected);
                                    m_selectedNode = m_spline.Nodes.Count > 0 ? m_spline.Nodes.Last() : null;
                                });
                                e.Use();
                            }
                            break;
                        }
                    }
                    // If the F Key is Pressed
                    if (e.keyCode == KeyCode.F)
                    {
                        if (m_selectedNode != null)
                        {
                            Vector3 nodePosition = m_selectedNode.Position;
                            Vector3 size = Vector3.one * 5f;
                            FocusPosition(nodePosition, size);
                            e.Use();
                        }
                    }
                    break;
            }
            // Check Raw Events
            switch (e.rawType)
            {
                case EventType.MouseUp:
                {
                    // RecordSelectedNodeChanges("Node Change");
                    mouseUp = true;
                    break;
                }
            }
            #endregion
            #region Tools
            Tool currentTool = Tools.current;
            if (m_previousTool != currentTool)
            {
                switch (currentTool)
                {
                    case Tool.Scale:
                        m_selectionType = SelectionType.Scale;
                        break;
                    case Tool.Move:
                        m_selectionType = SelectionType.Node;
                        break;
                }
            }
            #endregion
            List<GeNaCurve> connectedCurves = new List<GeNaCurve>();
            if (m_selectedNode != null)
            {
                connectedCurves = m_spline.GetConnectedCurves(m_selectedNode);
            }
            // Draw a bezier curve for each curve in the m_spline
            foreach (GeNaCurve curve in m_spline.Curves)
            {
                Color color = SPLINE_CURVE_COLOR;
                if (connectedCurves.Contains(curve))
                    color = SPLINE_SELECTED_CURVE_COLOR;
                DrawCurve(curve, color);
            }
            // At least one node?
            if (m_spline.Nodes.Count > 0)
            {
                // Node Selected?
                if (m_selectedNode != null)
                {
                    Quaternion rotation = Quaternion.identity;
                    // If Tools are set to Local AND Spline Smoothing is NOT enabled
                    // Draw the nodeSelection handles
                    switch (m_selectionType)
                    {
                        case SelectionType.Node:
                        {
                            Vector3 point = m_selectedNode.Position;
                            Vector3 result = Handles.PositionHandle(point, rotation);
                            // place a handle on the node and manage m_position change
                            if (result != point)
                            {
                                if (m_splineModified == false)
                                    m_spline.RecordUndoSnapshot("Node Moved", () => { m_selectedNode.Position = result; });
                                else
                                    m_selectedNode.Position = result;
                                m_spline.IsDirty = true;
                                m_splineModified = true;
                            }
                            break;
                        }
                        case SelectionType.StartTangent:
                        {
                            Vector3 point = _selectedGeNaCurve.P1;
                            Vector3 result = Handles.PositionHandle(point, rotation);
                            if (result != point)
                            {
                                if (m_splineModified == false)
                                    m_spline.RecordUndoSnapshot("Start Tangent Moved", () => { _selectedGeNaCurve.P1 = result; });
                                else
                                    _selectedGeNaCurve.P1 = result;
                                m_spline.IsDirty = true;
                                m_splineModified = true;
                            }
                            break;
                        }
                        case SelectionType.EndTangent:
                        {
                            Vector3 point = _selectedGeNaCurve.P2;
                            Vector3 result = Handles.PositionHandle(point, rotation);
                            if (result != point)
                            {
                                if (m_splineModified == false)
                                    m_spline.RecordUndoSnapshot("End Tangent Moved", () => { _selectedGeNaCurve.P2 = result; });
                                else
                                    _selectedGeNaCurve.P2 = result;
                                m_spline.IsDirty = true;
                                m_splineModified = true;
                            }
                            break;
                        }
                        case SelectionType.Up:
                        {
                            Vector3 point = m_selectedNode.Position + m_selectedNode.Up * 8f;
                            Vector3 result = Handles.PositionHandle(point, rotation);
                            if (result != point)
                            {
                                if (m_splineModified == false)
                                {
                                    m_spline.RecordUndoSnapshot("Up Moved", () => { m_selectedNode.Up = (result - m_selectedNode.Position).normalized; });
                                }
                                else
                                {
                                    m_selectedNode.Up = (result - m_selectedNode.Position).normalized;
                                }
                                m_spline.IsDirty = true;
                                m_splineModified = true;
                            }
                            break;
                        }
                        case SelectionType.Scale:
                        {
                            if (e.isMouse && e.type == EventType.MouseDown)
                                m_selectedNode.Scale = Vector3.one;
                            Vector3 point = m_selectedNode.Position;
                            Vector3 result = Vector3.one;
                            float size = HandleUtility.GetHandleSize(point);
                            EditorGUI.BeginChangeCheck();
                            {
                                result = Handles.ScaleHandle(m_selectedNode.Scale, point, rotation, size);
                            }
                            if (EditorGUI.EndChangeCheck())
                            {
                                if (m_splineModified == false)
                                {
                                    m_spline.RecordUndoSnapshot("Node Scale Changed", () => { m_selectedNode.Scale = result; });
                                }
                                else
                                {
                                    m_selectedNode.Scale = result;
                                }
                                m_spline.IsDirty = true;
                                m_splineModified = true;
                            }
                            break;
                        }
                    }
                }
            }
            Handles.BeginGUI();
            if (m_selectedNode != null)
            {
                List<GeNaCurve> curves = m_spline.GetConnectedCurves(m_selectedNode);
                foreach (GeNaCurve curve in curves)
                    if (!DrawCurveHandles(curve))
                        break;
            }
            foreach (GeNaNode node in m_spline.Nodes)
            {
                Vector3 pos = node.Position;
                // First we check if at least one thing is in the camera field of view 
                if (!IsOnScreen(pos))
                    // Continue to next Element
                    continue;
                if (!DrawNodeHandles(node))
                    break;
            }
            Handles.EndGUI();
            #region Add Splines
            bool raycastHit = GetRayCast(out RaycastHit hitInfo);
            //Check for the ctrl + left mouse button event - spawn (ignore if Shift is pressed)
            if (e.control && e.isMouse && !e.shift)
            {
                // Left button
                if (e.button == 0)
                {
                    switch (e.type)
                    {
                        case EventType.MouseDown:
                            GUIUtility.hotControl = 0;
                            if (raycastHit)
                            {
                                // Cache variable for action
                                var selected = m_selectedNode;
                                m_spline.RecordUndoSnapshot("Add Node", () =>
                                {
                                    GeNaNode newNode = m_spline.CreateNewNode(hitInfo.point);
                                    if (m_spline.AddNode(selected, newNode))
                                    {
                                        m_selectedNode = newNode;
                                    }
                                });
                            }
                            e.Use();
                            break;
                    }
                }
            }
            if (m_splineModified)
            {
                if (mouseUp)
                {
                    float distance = Vector2.SqrMagnitude(e.mousePosition - m_mouseClickPoint);
                    if (distance > m_mouseDragThreshold)
                    {
                        m_spline.OnSplineEndChanged();
                    }
                    m_splineModified = false;
                }
            }
            #endregion
            #region Footer
            if (m_spline.IsDirty)
            {
                EditorUtility.SetDirty(m_spline);
                m_spline.IsDirty = false;
            }
            #endregion
        }
        private void ReconnectSpline()
        {
            foreach (ExtensionEntry entry in m_spline.Extensions)
            {
                if (entry == null)
                    continue;
                GeNaSplineExtension extension = entry.Extension;
                if (extension == null)
                    continue;
                if (extension.Spline == null)
                {
                    // extension.SetSpline(m_spline);
                }
            }
        }
        private void DrawDebug()
        {
            List<GeNaNode> nodes = m_spline.Nodes;
            foreach (GeNaNode node in nodes)
            {
                string text = $"Node ID: {node.ID}";
                Vector3 worldPos = node.Position;
                GUIStyle guiStyle = EditorStyles.numberField;
                Vector2 screenSize = GeNaEditorUtility.CalculateScreenSize(text, guiStyle);
                Vector2 halfScreenSize = screenSize * .5f;
                float yOffset = screenSize.y;
                Vector2 screenOffset = new Vector2(-halfScreenSize.x, yOffset);
                GeNaEditorUtility.DrawString(text, worldPos, screenOffset, guiStyle);
            }
        }
        private bool DrawCurveHandles(GeNaCurve geNaCurve)
        {
            // First we check if at least one thing is in the camera field of view 
            if (IsOnScreen(geNaCurve.P1))
            {
                Vector2 guiStartPos = HandleUtility.WorldToGUIPoint(geNaCurve.P0);
                Vector2 guiStartTangent = HandleUtility.WorldToGUIPoint(geNaCurve.P1);
                Color oldColor = Handles.color;
                Color blue = new Color(0.3f, 0.3f, 1.0f, 1f);
                Handles.color = blue;
                Handles.DrawLine(guiStartTangent, guiStartPos);
                Handles.color = oldColor;
                // Draw directional button handles
                if (Button(guiStartTangent, Styles.knobTexture2D, blue))
                {
                    _selectedGeNaCurve = geNaCurve;
                    m_selectionType = SelectionType.StartTangent;
                    return false;
                }
                Handles.color = oldColor;
            }
            // First we check if at least one thing is in the camera field of view 
            if (IsOnScreen(geNaCurve.P2))
            {
                Vector2 guiEndTangent = HandleUtility.WorldToGUIPoint(geNaCurve.P2);
                Vector2 guiEndPos = HandleUtility.WorldToGUIPoint(geNaCurve.P3);
                Color oldColor = Handles.color;
                Handles.color = Color.red;
                Handles.DrawLine(guiEndTangent, guiEndPos);
                Handles.color = oldColor;
                if (Button(guiEndTangent, Styles.knobTexture2D, Color.red))
                {
                    _selectedGeNaCurve = geNaCurve;
                    m_selectionType = SelectionType.EndTangent;
                    return false;
                }
                Handles.color = oldColor;
            }
            return true;
        }
        private void DrawConnectedNodeHandles(GeNaNode node, Texture2D texture, Color color)
        {
            Vector3 pos = node.Position;
            Vector2 guiPos = HandleUtility.WorldToGUIPoint(pos);
            Vector3 up = node.Position + node.Up * 8f;
            Vector2 guiUp = HandleUtility.WorldToGUIPoint(up);
            // For the selected node, we also draw a line and place two buttons for directions
            Handles.color = Color.red;
            // Draw quads direction and inverse direction if they are not selected
            if (m_selectionType != SelectionType.Node)
                if (Button(guiPos, texture, color))
                    m_selectionType = SelectionType.Node;
            if (m_spline.Nodes.Contains(node))
            {
                if (showUpVector)
                {
                    Handles.color = Color.green;
                    Handles.DrawLine(guiPos, guiUp);
                    if (m_selectionType != SelectionType.Up)
                    {
                        if (Button(guiUp, texture, color))
                        {
                            m_selectedNode = node;
                            m_selectionType = SelectionType.Up;
                        }
                    }
                }
            }
        }
        private void DrawConnectedNodeHandles(GeNaNode node, GUIStyle style, Color color)
        {
            Vector3 pos = node.Position;
            Vector2 guiPos = HandleUtility.WorldToGUIPoint(pos);
            Vector3 up = node.Position + node.Up * 8f;
            Vector2 guiUp = HandleUtility.WorldToGUIPoint(up);
            // For the selected node, we also draw a line and place two buttons for directions
            Handles.color = Color.red;
            // Draw quads direction and inverse direction if they are not selected
            if (m_selectionType != SelectionType.Node)
                if (Button(guiPos, style, color))
                    m_selectionType = SelectionType.Node;
            if (m_spline.Nodes.Contains(node))
            {
                if (showUpVector)
                {
                    Handles.color = Color.green;
                    Handles.DrawLine(guiPos, guiUp);
                    if (m_selectionType != SelectionType.Up)
                    {
                        if (Button(guiUp, style, color))
                        {
                            m_selectedNode = node;
                            m_selectionType = SelectionType.Up;
                        }
                    }
                }
            }
        }
        private bool DrawNodeHandles(GeNaNode prevNode)
        {
            Vector3 pos = prevNode.Position;
            Vector3 guiPos = HandleUtility.WorldToGUIPoint(pos);
            List<GeNaNode> nodes = m_spline.Nodes;
            bool isFirstNode = prevNode == nodes.First();
            bool isLastNode = prevNode == nodes.Last();
            if (prevNode == m_selectedNode)
            {
                if (isFirstNode || isLastNode)
                    DrawConnectedNodeHandles(prevNode, Styles.knobTexture2D, isFirstNode ? FIRST_NODE_COLOR : LAST_NODE_COLOR);
                else
                    DrawConnectedNodeHandles(prevNode, Styles.nodeBtn, NODE_COLOR);
            }
            else
            {
                bool buttonPressed = false;
                if (isFirstNode || isLastNode)
                    buttonPressed = Button(guiPos, Styles.knobTexture2D, isFirstNode ? FIRST_NODE_COLOR : LAST_NODE_COLOR);
                else
                    buttonPressed = Button(guiPos, Styles.knobTexture2D, NODE_COLOR);
                if (buttonPressed)
                {
                    Event e = Event.current;
                    if (e.control)
                        if (m_selectedNode != null)
                        {
                            var selected = m_selectedNode;
                            m_spline.RecordUndoSnapshot("Connect nodes", () =>
                            {
                                m_spline.AddNode(selected, prevNode); //, node);
                            });
                        }
                    m_selectedNode = prevNode;
                    m_selectionType = SelectionType.Node;
                    return false;
                }
            }
            return true;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            #region Header
            m_editorUtils.Initialize();
            GUILayout.Space(3f);
            m_editorUtils.GUIHeader();
            GeNaEditorUtility.DisplayWarnings();
            m_editorUtils.GUINewsHeader();
            #endregion
            #region Panels
            m_showQuickStart = m_editorUtils.Panel("Quick Start", QuickStartPanel, m_showQuickStart);
            // Overview Panel
            GUIStyle overviewLabelStyle = Styles.panelLabel;
            string overviewText = string.Format("{0} : {1}", m_editorUtils.GetTextValue("Overview Panel Label"), m_spline.Name);
            GUIContent overviewPanelLabel = new GUIContent(overviewText, m_editorUtils.GetTooltip("Overview Panel Label"));
            m_showOverview = m_editorUtils.Panel(overviewPanelLabel, OverviewPanel, overviewLabelStyle, m_showOverview);
            m_showPathFinding = m_editorUtils.Panel("Path Finding Label", PathFindingPanel, m_showPathFinding);
            m_showExtensions = m_editorUtils.Panel("Extensions Label", ExtensionPanel, m_showExtensions);
            // Advanced Panel
            m_showAdvancedSettings = m_editorUtils.Panel("Advanced Panel Label", AdvancedPanel, m_showAdvancedSettings);
            #endregion
            if (m_spline.IsDirty)
            {
                GeNaEditorUtility.ForceUpdate();
                EditorUtility.SetDirty(m_spline);
                m_spline.IsDirty = false;
            }
            m_previousTool = Tools.current;
            m_editorUtils.GUINewsFooter(false);
        }
        #endregion
        #region Utilities
        public static bool IsOnScreen(Vector3 position)
        {
            Vector3 onScreen = Camera.current.WorldToViewportPoint(position);
            return onScreen.z > 0f && onScreen.x > 0f &&
                   onScreen.y > 0f && onScreen.x < 1f &&
                   onScreen.y < 1f;
        }
        /// <summary>
        /// Makes the Scene View Focus on a Given Point and Size
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        public void FocusPosition(Vector3 pos, Vector3 size)
        {
            SceneView.lastActiveSceneView.Frame(new Bounds(pos, size), false);
        }
        /// <summary>
        /// Checks if the mouse is over the SceneView
        /// </summary>
        private bool MouseOverSceneView(out Vector2 mousePos)
        {
            mousePos = Event.current.mousePosition;
            if (mousePos.x < 0f || mousePos.y < 0f)
                return false;
            Rect swPos = SceneView.lastActiveSceneView.position;
            return !(mousePos.x > swPos.width) &&
                   !(mousePos.y > swPos.height);
        }
        /// <summary>
        /// Shows the outline of the spawn range and does the raycasting.
        /// </summary>
        /// <returns>The Raycast hit info.</returns>
        private bool GetRayCast(out RaycastHit hitInfo)
        {
            //Stop if not over the SceneView
            if (!MouseOverSceneView(out Vector2 mousePos))
            {
                hitInfo = new RaycastHit();
                return false;
            }

            //Let's do the raycast first
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
            return Physics.Raycast(ray, out hitInfo, 10000f);
        }
        #endregion
        #region Spline Extension Reorderable
        private void CreateExtensionList()
        {
            m_extensionReorderable = new ReorderableList(m_spline.Extensions, typeof(GeNaSplineExtension), true, true, true, true);
            m_extensionReorderable.elementHeightCallback = OnElementHeightExtensionListEntry;
            m_extensionReorderable.drawElementCallback = DrawExtensionListElement;
            m_extensionReorderable.drawHeaderCallback = DrawExtensionListHeader;
            m_extensionReorderable.onAddCallback = OnAddExtensionListEntry;
            m_extensionReorderable.onRemoveCallback = OnRemoveExtensionListEntry;
            m_extensionReorderable.onReorderCallback = OnReorderExtensionList;
        }
        private void OnReorderExtensionList(ReorderableList reorderableList)
        {
            //Do nothing, changing the order does not immediately affect anything in the stamper
        }
        private void OnRemoveExtensionListEntry(ReorderableList reorderableList)
        {
            m_spline.RecordUndoSnapshot("Extension Removed", () =>
            {
                m_selectedExtensionEntry = null;
                int indexToRemove = reorderableList.index;
                m_spline.RemoveExtension(indexToRemove);
                reorderableList.list = m_spline.Extensions;
                if (indexToRemove >= reorderableList.list.Count)
                    indexToRemove = reorderableList.list.Count - 1;
                reorderableList.index = indexToRemove;
                if (reorderableList.list.Count > 0)
                {
                    int lastIndex = reorderableList.list.Count - 1;
                    ExtensionEntry nextEntry = m_spline.Extensions[lastIndex];
                    SelectExtensionEntry(nextEntry);
                }
            });
        }
        private void OnAddExtensionListEntry(ReorderableList reorderableList)
        {
            m_spline.RecordUndoSnapshot("Extension Added", () =>
            {
                ExtensionEntry extension = m_spline.AddExtension(null);
                reorderableList.list = m_spline.Extensions;
                SelectExtensionEntry(extension);
            });
        }
        private void DrawExtensionListHeader(Rect rect)
        {
            DrawExtensionListHeader(rect, true, m_spline.Extensions, m_editorUtils);
        }
        private void DrawExtensionListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            ExtensionEntry entry = m_spline.Extensions[index];
            DrawExtensionListElement(rect, entry, m_editorUtils, isFocused);
        }
        private float OnElementHeightExtensionListEntry(int index)
        {
            return OnElementHeight();
        }
        public float OnElementHeight()
        {
            return EditorGUIUtility.singleLineHeight + 4f;
        }
        public void DrawExtensionListHeader(Rect rect, bool currentFoldOutState, List<ExtensionEntry> extensionList, EditorUtils editorUtils)
        {
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.LabelField(rect, editorUtils.GetContent("SpawnerEntryHeader"));
            EditorGUI.indentLevel = oldIndent;
        }
        public void DrawExtensionList(ReorderableList list, EditorUtils editorUtils)
        {
            Rect maskRect = EditorGUILayout.GetControlRect(true, list.GetHeight());
            list.DoList(maskRect);
        }
        public void SelectAllExtensionEntries()
        {
            foreach (ExtensionEntry entry in m_spline.Extensions)
                entry.IsSelected = true;
        }
        public void DeselectAllExtensionEntries()
        {
            foreach (ExtensionEntry entry in m_spline.Extensions)
                entry.IsSelected = false;
            m_selectedExtensionEditor = null;
            m_selectedExtensionEntry = null;
            m_selectedExtension = null;
        }
        public void SelectExtensionEntry(int entryIndex)
        {
            if (entryIndex < 0 || entryIndex >= m_spline.Extensions.Count)
            {
                m_selectedExtension = null;
                return;
            }
            SelectExtensionEntry(m_spline.Extensions[entryIndex]);
        }
        public void SelectExtensionEntry(ExtensionEntry entryToSelect)
        {
            foreach (ExtensionEntry entry in m_spline.Extensions)
            {
                if (entry == entryToSelect)
                    continue;
                entry.IsSelected = false;
            }
            entryToSelect.IsSelected = true;
            if (m_selectedExtensionEditor != null)
                m_selectedExtensionEditor.OnDeselected();
            m_selectedExtensionEntry = entryToSelect;
            m_selectedExtensionEditor = CreateEditor(entryToSelect.Extension) as GeNaSplineExtensionEditor;
            m_selectedExtension = entryToSelect.Extension;
            int selectedExtensionIndex = m_extensionReorderable.list.IndexOf(entryToSelect);
            m_extensionReorderable.index = selectedExtensionIndex;
            m_spline.SelectedExtensionIndex = selectedExtensionIndex;
            if (m_selectedExtensionEditor != null)
                m_selectedExtensionEditor.OnSelected();
        }
        public void DrawExtensionListElement(Rect rect, ExtensionEntry entry, EditorUtils editorUtils, bool isFocused)
        {
            if (isFocused)
            {
                if (m_selectedExtension != entry.Extension)
                {
                    DeselectAllExtensionEntries();
                    entry.IsSelected = true;
                    SelectExtensionEntry(entry);
                }
            }
            // Spawner Object
            EditorGUI.BeginChangeCheck();
            {
                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                EditorGUI.LabelField(new Rect(rect.x, rect.y + 1f, rect.width * 0.18f, EditorGUIUtility.singleLineHeight), editorUtils.GetContent("SpawnerEntryActive"));
                entry.IsActive = EditorGUI.Toggle(new Rect(rect.x + rect.width * 0.18f, rect.y, rect.width * 0.1f, EditorGUIUtility.singleLineHeight), entry.IsActive);
                GeNaSplineExtension extension = entry.Extension;
                extension = (GeNaSplineExtension)EditorGUI.ObjectField(new Rect(rect.x + rect.width * 0.4f, rect.y + 1f, rect.width * 0.6f, EditorGUIUtility.singleLineHeight), extension, typeof(GeNaSplineExtension), false);
                if (extension != entry.Extension)
                {
                    if (entry.Extension != null)
                    {
                        m_spline.RecordUndoSnapshot("Extension Removed", () => { m_spline.RemoveExtension(entry.Extension); });
                    }
                    if (extension != null)
                    {
                        m_spline.RecordUndoSnapshot("Extension Added", () =>
                        {
                            GeNaSplineExtension newExtension = m_spline.CopyExtension(extension);
                            ExtensionEntry newEntry = m_spline.AddExtension(newExtension);
                            m_spline.RemoveExtensionEntry(entry);
                            SelectExtensionEntry(newEntry);
                        });
                    }
                }
                EditorGUI.indentLevel = oldIndent;
            }
            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
        }
        #endregion
        #region Panels
        private void QuickStartPanel(bool helpEnabled)
        {
            if (ActiveEditorTracker.sharedTracker.isLocked)
                EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("Inspector locked warning"), MessageType.Warning);
            if (m_showQuickStart)
            {
                m_editorUtils.Label("Create Nodes Help", m_editorUtils.Styles.help);
                m_editorUtils.Label("Remove Nodes Help", m_editorUtils.Styles.help);
                if (m_editorUtils.Button("View Tutorials Btn"))
                    Application.OpenURL(PWApp.CONF.TutorialsLink);
            }
        }
        /// <summary>
        /// Handle drop area for new objects
        /// </summary>
        public bool DrawExtensionGUI()
        {
            // Ok - set up for drag and drop
            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            string dropMsg = m_dropGround ? m_editorUtils.GetTextValue("Drop ground lvl box msg") : m_editorUtils.GetTextValue("Attach Extensions");
            GUI.Box(dropArea, dropMsg, Styles.gpanel);
            bool recordedUndo = false;
            if (evt.type == EventType.DragPerform || evt.type == EventType.DragUpdated)
            {
                if (!dropArea.Contains(evt.mousePosition))
                    return false;
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    //Handle game objects / prefabs
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is GameObject go)
                        {
                            GeNaSpawner spawner = go.GetComponent<GeNaSpawner>();
                            if (spawner != null)
                            {
                                spawner.Load();
                                GeNaSpawnerExtension geNaSpawnerExtension = CreateInstance<GeNaSpawnerExtension>();
                                geNaSpawnerExtension.name = spawner.name;
                                geNaSpawnerExtension.Spawner = spawner;
                                GeNaSpawnerData spawnerData = spawner.SpawnerData;
                                SpawnerEntry spawnerEntry = geNaSpawnerExtension.SpawnerEntry;
                                spawnerEntry.SpawnRange = spawnerData.SpawnRange;
                                spawnerEntry.ThrowDistance = spawnerData.ThrowDistance;
                                if (!recordedUndo)
                                {
                                    m_spline.RecordUndoSnapshot("Extensions Added", () =>
                                    {
                                        ExtensionEntry entry = m_spline.AddExtension(geNaSpawnerExtension);
                                        SelectExtensionEntry(entry);
                                    });
                                }
                                else
                                {
                                    ExtensionEntry entry = m_spline.AddExtension(geNaSpawnerExtension);
                                    SelectExtensionEntry(entry);
                                }
                            }
                        }
                        if (draggedObject is GeNaSplineExtension extensionReference)
                        {
                            if (extensionReference != null)
                            {
                                if (!recordedUndo)
                                {
                                    m_spline.RecordUndoSnapshot("Extensions Added", () =>
                                    {
                                        GeNaSplineExtension newExtension = m_spline.CopyExtension(extensionReference);
                                        ExtensionEntry entry = m_spline.AddExtension(newExtension);
                                        SelectExtensionEntry(entry);
                                    });
                                }
                                else
                                {
                                    GeNaSplineExtension newExtension = m_spline.CopyExtension(extensionReference);
                                    ExtensionEntry entry = m_spline.AddExtension(newExtension);
                                    SelectExtensionEntry(entry);
                                }
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        private void OverviewPanel(bool helpEnabled)
        {
            m_editorUtils.InlineHelp("Overview Panel Label", helpEnabled);
            EditorGUI.BeginChangeCheck();
            {
                m_spline.Name = m_editorUtils.TextField("Spline Name", m_spline.Name, helpEnabled);
                #region Smooth Spline
                m_spline.SmoothStrength = m_editorUtils.Slider("Smooth Strength", m_spline.SmoothStrength, 0f, 1f, helpEnabled);
                m_spline.AutoSmooth = m_editorUtils.Toggle("Auto Smooth", m_spline.AutoSmooth);
                m_spline.SmoothIntersections = m_editorUtils.Toggle("Smooth Intersections", m_spline.SmoothIntersections);
                m_spline.AutoSnapOnSubdivide = m_editorUtils.Toggle("Snap to Ground", m_spline.AutoSnapOnSubdivide);
                #endregion
                #region Simplify Spline
                m_spline.SimplifyEpsilon = m_editorUtils.Slider("Simplify Strength", m_spline.SimplifyEpsilon, 0.5f, 5.0f);
                m_spline.SimplifyScale = m_editorUtils.Slider("Simplify Y Scale", m_spline.SimplifyScale, 0.5f, 1.5f);
                #endregion
                EditorGUILayout.BeginVertical();
                {
                    if (m_spline.Nodes.Count == 0)
                        GUI.enabled = false;
                    #region Sub Divisions
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (m_editorUtils.Button("Subdivide", helpEnabled))
                        {
                            m_spline.RecordUndoSnapshot("Subdivide", m_spline.Subdivide);
                        }
                        if (m_editorUtils.Button("Simplify", helpEnabled))
                        {
                            m_spline.RecordUndoSnapshot("Simplify", () => m_spline.SimplifyNodesAndCurves(m_spline.SimplifyScale, m_spline.SimplifyEpsilon));
                        }
                        if (m_editorUtils.Button("Smooth Spline", helpEnabled))
                        {
                            m_spline.RecordUndoSnapshot("Smooth", () => m_spline.Smooth(m_spline.SmoothStrength));
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion
                    #region Clear All Nodes
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (m_editorUtils.Button("Clear All Nodes", helpEnabled))
                        {
                            m_spline.RecordUndoSnapshot("Clear All Nodes", () => m_spline.RemoveAllNodes());
                            m_selectedNode = null;
                        }
                        if (m_editorUtils.Button("Snap Nodes To Ground", helpEnabled))
                        {
                            m_spline.RecordUndoSnapshot("Snap to Ground", () => m_spline.SnapNodesToGround());
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion
                    GUI.enabled = true;
                }
                EditorGUILayout.EndVertical();
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_spline.IsDirty = true;
            }
        }
        private void PathFindingPanel(bool helpEnabled)
        {
            EditorGUI.BeginChangeCheck();
            {
                bool defaultEnabled = GUI.enabled;
                m_spline.PathFindingEnabled = m_editorUtils.Toggle("Path Finding Enabled", m_spline.PathFindingEnabled, helpEnabled);
                bool thisEnabled = GUI.enabled = m_spline.PathFindingEnabled;
                m_spline.UseExistingCurves = m_editorUtils.Toggle("UseExistingCurves", m_spline.UseExistingCurves, helpEnabled);
                GUI.enabled = thisEnabled && m_spline.UseExistingCurves;
                EditorGUI.indentLevel++;
                m_spline.CurveTravelCost = m_editorUtils.Slider("CurveTravelCost", m_spline.CurveTravelCost, 0.02f, 1.0f, helpEnabled);
                EditorGUI.indentLevel--;
                GUI.enabled = thisEnabled;
                m_spline.PathFinderIgnoreMask = EditorUtilsExtensions.LayerMaskField(m_editorUtils, "Ignore Mask", m_spline.PathFinderIgnoreMask, helpEnabled);
                m_spline.MaxGrade = m_editorUtils.Slider("Max Grade", m_spline.MaxGrade, 1.0f, 30.0f, helpEnabled);
                m_spline.CellSize = m_editorUtils.IntSlider("Cell Size", m_spline.CellSize, 2, 20, helpEnabled);
                if (GeNaUtility.Gaia2Present)
                {
                    EditorGUILayout.BeginHorizontal();
                    m_spline.MinHeight = m_editorUtils.FloatField("Min Height", m_spline.MinHeight, helpEnabled);
                    if (m_editorUtils.Button("GetGaiaSeaLevel", GUILayout.MaxWidth(125f)))
                    {
                        m_spline.MinHeight = GeNaEvents.GetSeaLevel(m_spline.MinHeight);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    m_spline.MinHeight = m_editorUtils.FloatField("Min Height", m_spline.MinHeight, helpEnabled);
                }
                m_spline.MaxHeight = m_editorUtils.FloatField("Max Height", m_spline.MaxHeight, helpEnabled);
                m_spline.UseHeuristicB = m_editorUtils.Toggle("Use Heuristic B", m_spline.UseHeuristicB, helpEnabled);
                GUI.enabled = GUI.enabled && !m_spline.UseHeuristicB;
                m_spline.SlopeStrengthFactor = m_editorUtils.Slider("Slope Strength Factor", m_spline.SlopeStrengthFactor, 0.1f, 2.0f, helpEnabled);
                GUI.enabled = true;
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_spline.IsDirty = true;
            }
        }
        private void ExtensionPanel(bool helpEnabled)
        {
            EditorGUI.BeginChangeCheck();
            {
                if (DrawExtensionGUI())
                    return;
                Rect listRect = EditorGUILayout.GetControlRect(true, m_extensionReorderable.GetHeight());
                m_extensionReorderable.DoList(listRect);
                if (m_selectedExtensionEntry != null)
                {
                    // Spawner Selected?
                    if (m_selectedExtension != null)
                    {
                        GUI.enabled = m_selectedExtensionEntry.IsActive;
                        EditorGUILayout.BeginHorizontal(Styles.gpanel);
                        EditorGUILayout.LabelField($"{m_selectedExtension.name} Extension Settings", Styles.boldLabel);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginVertical(Styles.gpanel);
                        if (m_selectedExtensionEditor == null)
                        {
                            SerializedObject so = new SerializedObject(m_selectedExtension);
                            so.Update();
                            SerializedProperty iter = so.GetIterator();
                            iter.NextVisible(true);
                            while (iter.NextVisible(false))
                            {
                                EditorGUILayout.PropertyField(iter);
                            }
                            so.ApplyModifiedProperties();
                        }
                        else
                        {
                            m_selectedExtensionEditor.HelpEnabled = helpEnabled;
                            m_selectedExtensionEditor.OnInspectorGUI();
                        }
                        EditorGUILayout.EndVertical();
                        GUI.enabled = true;
                    }
                }
                // if (m_editorUtils.Button("FinalizeAll"))
                // {
                //     if (EditorUtility.DisplayDialog(m_editorUtils.GetTextValue("FinalizeTitle"),
                //         m_editorUtils.GetTextValue("FinalizeMessage"), m_editorUtils.GetTextValue("FinalizeYes"),
                //         m_editorUtils.GetTextValue("FinalizeNo")))
                //     {
                //         Terrain terrainParent = Terrain.activeTerrain;
                //         if (terrainParent != null)
                //         {
                //             GeNaEditorUtility.FinalizeAll(m_spline, terrainParent.gameObject);
                //         }
                //         else
                //         {
                //             GeNaEditorUtility.FinalizeAll(m_spline, null);
                //         }
                //     }
                // }
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_spline.IsDirty = true;
            }
        }
        private void AdvancedPanel(bool helpEnabled)
        {
            EditorGUI.BeginChangeCheck();
            {
                m_editorUtils.InlineHelp("Advanced Panel", helpEnabled);
                SplineSettings.AdvancedSettings advancedSettings = m_settings.Advanced;
                advancedSettings.DebuggingEnabled = m_editorUtils.Toggle("Adv Debugging Enabled", advancedSettings.DebuggingEnabled, helpEnabled);
                advancedSettings.VisualizationProximity = m_editorUtils.FloatField("Adv Visualization Proximity", advancedSettings.VisualizationProximity, helpEnabled);
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_spline.IsDirty = true;
            }
        }
        #endregion
        #region GUI
        public static bool Button(Vector2 position, Texture2D texture2D, Color color)
        {
            Vector2 quadSize = new Vector2(SPLINE_QUAD_SIZE, SPLINE_QUAD_SIZE);
            Vector2 halfQuadSize = quadSize * .5f;
            Rect buttonRect = new Rect(position - halfQuadSize, quadSize);
            Color oldColor = GUI.color;
            GUI.color = color;
            bool result = GUI.Button(buttonRect, texture2D, GUIStyle.none);
            GUI.color = oldColor;
            return result;
        }
        public static bool Button(Vector2 position, GUIStyle style, Color color)
        {
            Vector2 quadSize = new Vector2(SPLINE_STYLE_QUAD_SIZE, SPLINE_STYLE_QUAD_SIZE);
            Vector2 halfQuadSize = quadSize * .5f;
            Rect buttonRect = new Rect(position - halfQuadSize, quadSize);
            Color oldColor = GUI.color;
            GUI.color = color;
            bool result = GUI.Button(buttonRect, GUIContent.none, style);
            GUI.color = oldColor;
            return result;
        }
        public static void DrawQuad(Rect rect, Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            GUI.skin.box.normal.background = texture;
            GUI.Box(rect, GUIContent.none);
        }
        public static void DrawQuad(Vector2 position, Color color)
        {
            Vector2 quadSize = new Vector2(EXTRUSION_QUAD_SIZE, EXTRUSION_QUAD_SIZE);
            Vector2 halfQuadSize = quadSize * .5f;
            Rect quad = new Rect(position - halfQuadSize, quadSize);
            DrawQuad(quad, color);
        }
        #endregion
        #endregion
    }
}