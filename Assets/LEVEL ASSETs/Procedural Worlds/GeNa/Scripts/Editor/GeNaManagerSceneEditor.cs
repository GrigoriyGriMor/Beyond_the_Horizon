// Engine
using UnityEngine;

// Editor
using UnityEditor;

// Procedural Worlds
namespace GeNa.Core
{
    [CustomEditor(typeof(GeNaManager))]
    public class GeNaManagerSceneEditor : GeNaEditor
    {
        #region Variables
        #region Static
        private GeNaManager m_manager;
        #endregion
        #endregion
        #region Methods
        #region Unity
        protected void OnEnable()
        {
            #region Initialization
            // If there isn't any Editor Utils Initialized
            if (m_editorUtils == null)
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this, null, null, null);
            // If there is no target associated with Editor Script
            if (target == null)
                // Exit the method
                return;
            // Get target Spline
            m_manager = (GeNaManager)target;
            //Hide its m_transform
            m_manager.transform.hideFlags = HideFlags.HideInInspector;
            m_manager.Initialize();
            #endregion
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            m_editorUtils.GUINewsHeader();
            #region Panel
            m_editorUtils.Panel("GeneralPanel", ManagerPanel, true);
            m_editorUtils.Panel("SpawnCallPanel", SpawnCallPanel, true);
            #endregion
            m_editorUtils.GUINewsFooter(false);
        }
        private static int SPLINE_QUAD_SIZE = 25;
        private static int SPLINE_STYLE_QUAD_SIZE = 15;
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
        private SpawnCall m_selectedSpawnCall = null;
        public override void OnSceneGUI()
        {
            Initialize();
            if (m_selectedSpawnCall != null)
            {
                switch (Tools.current)
                {
                    case Tool.Rotate:
                    {
                        Vector3 point = m_selectedSpawnCall.Location;
                        var rotation = Quaternion.Euler(m_selectedSpawnCall.Rotation);
                        var result = Handles.RotationHandle(rotation, point);
                        // place a handle on the node and manage m_position change
                        if (result != rotation)
                        {
                            m_selectedSpawnCall.Rotation = result.eulerAngles;
                            m_selectedSpawnCall.UpdateEntities();
                        }
                        break;
                    }
                    default:
                    {
                        var rotation = Quaternion.Euler(m_selectedSpawnCall.Rotation);
                        Vector3 point = m_selectedSpawnCall.Location;
                        Vector3 result = Handles.PositionHandle(point, rotation);
                        // place a handle on the node and manage m_position change
                        if (result != point)
                        {
                            m_selectedSpawnCall.Location = result;
                            m_selectedSpawnCall.UpdateEntities();
                        }
                    }
                        break;
                }
            }
            Handles.BeginGUI();
            var spawnCalls = m_manager.ActiveSpawnCalls;
            foreach (var spawnCall in spawnCalls)
            {
                if (spawnCall == null)
                    continue;
                Vector2 guiPos = HandleUtility.WorldToGUIPoint(spawnCall.Location);
                if (Button(guiPos, Styles.knobTexture2D, Color.green))
                {
                    m_selectedSpawnCall = spawnCall;
                    break;
                }
            }
            Handles.EndGUI();
        }
        #endregion
        #region Panel
        private void ManagerPanel(bool helpEnabled)
        {
            m_editorUtils.Text("WelcomeToGeNaManager");
            if (m_editorUtils.Button("ShowGeNaManager"))
            {
                ShowGeNaManager();
            }
        }
        private void SpawnCallPanel(bool helpEnabled)
        {
            if (m_editorUtils.Button("ClearEmptySpawnCalls"))
            {
                m_manager.ClearEmptySpawnCalls();
            }
            if (m_selectedSpawnCall == null)
                return;
            EditorGUILayout.LabelField(m_selectedSpawnCall.Location.ToString());
            EditorGUI.BeginChangeCheck();
            {
                m_selectedSpawnCall.AlignChildrenToRotation = EditorGUILayout.Toggle("Align Children", m_selectedSpawnCall.AlignChildrenToRotation);
                m_selectedSpawnCall.ConformChildrenToSlope = EditorGUILayout.Toggle("Conform Children", m_selectedSpawnCall.ConformChildrenToSlope);
                m_selectedSpawnCall.SnapChildrenToGround = EditorGUILayout.Toggle("Snap Children", m_selectedSpawnCall.SnapChildrenToGround);
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_selectedSpawnCall.UpdateEntities();
            }
        }
        #endregion
        #region Utilities
        private void ShowGeNaManager()
        {
            GeNaManagerEditor.MenuGeNaMainWindow();
        }
        #endregion
        #endregion
    }
}