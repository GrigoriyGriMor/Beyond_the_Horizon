﻿using System.Linq;
using UnityEditor;
using UnityEngine;
namespace GeNa.Core
{
    [CustomEditor(typeof(GeNaBuilderDecorator))]
    public class GeNaBuilderDecoratorEditor : GeNaDecoratorEditor<GeNaBuilderDecorator>
    {
        [MenuItem("GameObject/GeNa/Decorators/Builder Decorator")]
        public static void AddDecorator(MenuCommand command) => GeNaDecoratorEditorUtility.CreateDecorator<GeNaBuilderDecorator>(command);
        /// <summary>
        /// Currently selected builder - snapping only works if this is selected
        /// </summary>
        public GeNaSubSpawnerDecorator[] m_subSpawnerDecorators = null;
        public GeNaTerrainDecorator[] m_terrainDecorators = null;
        public GeNaBuilderDecorator m_selectedDecorator = null;
        // private KeyCode m_keyPrevElement;
        // private KeyCode m_keyPrevElementTAB;
        // private KeyCode m_keyNextElement;
        // private KeyCode m_keyNextElementTAB;
        private void ProcessUnityEditorEvent()
        {
            // Event e = Event.current;
            // if (e == null)
            //     return;
            //
            // //Nothing to do if this is not an origami related structure
            // if (m_selectedDecorator == null)
            //     return;
            // //Grab selected transform
            // Transform selectedTr = m_selectedDecorator.GetSelectedTransform();
            // if (e.type == EventType.KeyDown)
            // {
            //     //Make sure scene window seleted
            //     //EditorApplication.ExecuteMenuItem("Window/Scene");
            //
            //     //Element selection
            //     if (!e.control && !e.shift && e.keyCode == m_keyPrevElement)
            //     {
            //         if (selectedTr.parent == null)
            //         {
            //         }
            //         else
            //         {
            //             int idx = selectedTr.GetSiblingIndex();
            //             if (idx > 0)
            //             {
            //                 Selection.activeGameObject = selectedTr.parent.GetChild(idx - 1).gameObject;
            //             }
            //             else
            //             {
            //                 Selection.activeGameObject = selectedTr.parent.gameObject;
            //             }
            //         }
            //         Event.current.Use();
            //         return;
            //     }
            //     if (!e.control && e.shift && e.keyCode == m_keyPrevElementTAB)
            //     {
            //         if (selectedTr.parent == null)
            //         {
            //         }
            //         else
            //         {
            //             int idx = selectedTr.GetSiblingIndex();
            //             if (idx > 0)
            //             {
            //                 Selection.activeGameObject = selectedTr.parent.GetChild(idx - 1).gameObject;
            //             }
            //             else
            //             {
            //                 Selection.activeGameObject = selectedTr.parent.gameObject;
            //             }
            //         }
            //         Event.current.Use();
            //         return;
            //     }
            //     if (!e.control && !e.shift && e.keyCode == m_keyNextElement)
            //     {
            //         if (selectedTr.parent == null)
            //         {
            //             if (selectedTr.childCount > 0)
            //             {
            //                 Selection.activeGameObject = selectedTr.GetChild(0).gameObject;
            //             }
            //         }
            //         else
            //         {
            //             int idx = selectedTr.GetSiblingIndex();
            //             if (idx < selectedTr.parent.childCount - 1)
            //             {
            //                 Selection.activeGameObject = selectedTr.parent.GetChild(idx + 1).gameObject;
            //             }
            //             else
            //             {
            //                 Selection.activeGameObject = selectedTr.parent.gameObject;
            //             }
            //         }
            //         Event.current.Use();
            //         return;
            //     }
            //     if (!e.control && !e.shift && e.keyCode == m_keyNextElementTAB)
            //     {
            //         if (selectedTr.parent == null)
            //         {
            //             if (selectedTr.childCount > 0)
            //             {
            //                 Selection.activeGameObject = selectedTr.GetChild(0).gameObject;
            //             }
            //         }
            //         else
            //         {
            //             int idx = selectedTr.GetSiblingIndex();
            //             if (idx < selectedTr.parent.childCount - 1)
            //             {
            //                 Selection.activeGameObject = selectedTr.parent.GetChild(idx + 1).gameObject;
            //             }
            //             else
            //             {
            //                 Selection.activeGameObject = selectedTr.parent.gameObject;
            //             }
            //         }
            //         Event.current.Use();
            //         return;
            //     }
            // }
            //
            //     //Increase snap distance
            //     if (e.control && e.keyCode == m_keyIncreaseSnapDistance)
            //     {
            //         SnapXZ *= 2f;
            //         SnapY *= 2f;
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Decrease snap distance
            //     if (e.control && e.keyCode == m_keyDecreaseSnapDistance)
            //     {
            //         SnapXZ /= 2f;
            //         SnapY /= 2f;
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Increase snap rotation
            //     if (e.shift && e.keyCode == m_keyIncreaseSnapRotation)
            //     {
            //         SnapRotation *= 2f;
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Decrease snap rotation
            //     if (e.shift && e.keyCode == m_keyDecreaseSnapRotation)
            //     {
            //         SnapRotation /= 2f;
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Move Left
            //     if (e.control && e.keyCode == m_keyMoveLeft)
            //     {
            //         Vector3 movement = Quaternion.Euler(0F, SceneView.lastActiveSceneView.rotation.eulerAngles.y, 0f) * Vector3.left;
            //         selectedTr.position += (movement * SnapXZ);
            //         if (SnapEnabled)
            //         {
            //             selectedTr.position = SnapTransformPosition(selectedTr.position);
            //         }
            //         SelectedBuilder.UpdateSelectedBounds();
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Move right
            //     if (e.control && e.keyCode == m_keyMoveRight)
            //     {
            //         Vector3 movement = Quaternion.Euler(0F, SceneView.lastActiveSceneView.rotation.eulerAngles.y, 0f) * Vector3.right;
            //         selectedTr.position += (movement * SnapXZ);
            //         if (SnapEnabled)
            //         {
            //             selectedTr.position = SnapTransformPosition(selectedTr.position);
            //         }
            //         SelectedBuilder.UpdateSelectedBounds();
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Move Forward
            //     if (e.control && e.keyCode == m_keyMoveForward)
            //     {
            //         Vector3 movement = Quaternion.Euler(0F, SceneView.lastActiveSceneView.rotation.eulerAngles.y, 0f) * Vector3.forward;
            //         selectedTr.position += (movement * SnapXZ);
            //         if (SnapEnabled)
            //         {
            //             selectedTr.position = SnapTransformPosition(selectedTr.position);
            //         }
            //         SelectedBuilder.UpdateSelectedBounds();
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Move Backward
            //     if (e.control && e.keyCode == m_keyMoveBackward)
            //     {
            //         Vector3 movement = Quaternion.Euler(0F, SceneView.lastActiveSceneView.rotation.eulerAngles.y, 0f) * Vector3.back;
            //         selectedTr.position += (movement * SnapXZ);
            //         if (SnapEnabled)
            //         {
            //             selectedTr.position = SnapTransformPosition(selectedTr.position);
            //         }
            //         SelectedBuilder.UpdateSelectedBounds();
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Move Up
            //     if (e.control && e.keyCode == m_keyMoveUp)
            //     {
            //         Vector3 movement = Quaternion.Euler(0F, SceneView.lastActiveSceneView.rotation.eulerAngles.y, 0f) * Vector3.up;
            //         selectedTr.position += (movement * SnapY);
            //         if (SnapEnabled)
            //         {
            //             selectedTr.position = SnapTransformPosition(selectedTr.position);
            //         }
            //         SelectedBuilder.UpdateSelectedBounds();
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Move Down
            //     if (e.control && e.keyCode == m_keyMoveDown)
            //     {
            //         Vector3 movement = Quaternion.Euler(0F, SceneView.lastActiveSceneView.rotation.eulerAngles.y, 0f) * Vector3.down;
            //         selectedTr.position += (movement * SnapY);
            //         if (SnapEnabled)
            //         {
            //             selectedTr.position = SnapTransformPosition(selectedTr.position);
            //         }
            //         SelectedBuilder.UpdateSelectedBounds();
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Rotate X Down
            //     if (e.shift && e.keyCode == m_keyRotateXDown)
            //     {
            //         selectedTr.Rotate(Vector3.right, -SnapRotation);
            //         if (SnapEnabled)
            //         {
            //             selectedTr.localEulerAngles = SnapTransformRotation(selectedTr.localEulerAngles);
            //         }
            //         SelectedBuilder.UpdateSelectedBounds();
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Rotate X Up
            //     if (e.shift && e.keyCode == m_keyRotateXUp)
            //     {
            //         selectedTr.Rotate(Vector3.right, SnapRotation);
            //         if (SnapEnabled)
            //         {
            //             selectedTr.localEulerAngles = SnapTransformRotation(selectedTr.localEulerAngles);
            //         }
            //         SelectedBuilder.UpdateSelectedBounds();
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Rotate Y Down
            //     if (e.shift && e.keyCode == m_keyRotateYDown)
            //     {
            //         selectedTr.Rotate(Vector3.up, SnapRotation);
            //         if (SnapEnabled)
            //         {
            //             selectedTr.localEulerAngles = SnapTransformRotation(selectedTr.localEulerAngles);
            //         }
            //         SelectedBuilder.UpdateSelectedBounds();
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Rotate Y Up
            //     if (e.shift && e.keyCode == m_keyRotateYUp)
            //     {
            //         selectedTr.Rotate(Vector3.up, -SnapRotation);
            //         if (SnapEnabled)
            //         {
            //             selectedTr.localEulerAngles = SnapTransformRotation(selectedTr.localEulerAngles);
            //         }
            //         SelectedBuilder.UpdateSelectedBounds();
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Rotate Z Down
            //     if (e.shift &&  e.keyCode == m_keyRotateZDown)
            //     {
            //         selectedTr.Rotate(Vector3.forward, SnapRotation);
            //         if (SnapEnabled)
            //         {
            //             selectedTr.localEulerAngles = SnapTransformRotation(selectedTr.localEulerAngles);
            //         }
            //         SelectedBuilder.UpdateSelectedBounds();
            //         Event.current.Use();
            //         return;
            //     }
            //
            //     //Rotate Z Up
            //     if (e.shift && e.keyCode == m_keyRotateZUp)
            //     {
            //         selectedTr.Rotate(Vector3.forward, -SnapRotation);
            //         if (SnapEnabled)
            //         {
            //             selectedTr.localEulerAngles = SnapTransformRotation(selectedTr.localEulerAngles);
            //         }
            //         SelectedBuilder.UpdateSelectedBounds();
            //         Event.current.Use();
            //         return;
            //     }
            // }

            // //Snapping - eg mouse movement
            // if (SnapEnabled)
            // {
            //     if (!Utils.Math_ApproximatelyEqual(m_lastSnapPosition, selectedTr.position))
            //     {
            //         selectedTr.position = SnapTransformPosition(selectedTr.position);
            //         m_lastSnapPosition = selectedTr.position;
            //         SelectedBuilder.UpdateSelectedBounds();
            //     }
            //     if (!Utils.Math_ApproximatelyEqual(m_lastSnapRotation, selectedTr.rotation.eulerAngles))
            //     {
            //         selectedTr.rotation = Quaternion.Euler(SnapTransformRotation(selectedTr.rotation.eulerAngles));
            //         m_lastSnapRotation = selectedTr.rotation.eulerAngles;
            //         SelectedBuilder.UpdateSelectedBounds();
            //     }
            // }

            //if (e.type == EventType.MouseDrag || e.type == EventType.MouseUp)
            //{
            //}
        }
        public void OnHierarchyHotkeys(int instanceID, Rect selectionRect)
        {
            ProcessUnityEditorEvent();
        }
        public void HierarchyWindowChanged()
        {
            ProcessUnityEditorEvent();
        }
        /// <summary>
        /// See if this is an origami structure with a valid builder
        /// </summary>
        private void OnSelectionChange()
        {
            m_subSpawnerDecorators = null;
            m_terrainDecorators = null;
            foreach (GameObject go in Selection.gameObjects)
            {
                GeNaBuilderDecorator builder = go.GetComponentInParent<GeNaBuilderDecorator>();
                if (builder != null)
                {
                    m_selectedDecorator = builder;
                    //m_selectedDecorator.DrawGizmos = true;
                    m_selectedDecorator.SetSelectedTransform(go.transform);
                    m_subSpawnerDecorators = m_selectedDecorator.GetComponentsInChildren<GeNaSubSpawnerDecorator>();
                    m_terrainDecorators = m_selectedDecorator.GetComponentsInChildren<GeNaTerrainDecorator>();
                    break;
                }
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            SceneView.duringSceneGui -= OnSceneViewGUI;
            SceneView.duringSceneGui += OnSceneViewGUI;
            EditorApplication.hierarchyChanged -= HierarchyWindowChanged;
            EditorApplication.hierarchyChanged += HierarchyWindowChanged;
            Selection.selectionChanged -= OnSelectionChange;
            Selection.selectionChanged += OnSelectionChange;
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyHotkeys;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyHotkeys;
            OnSelectionChange();
            m_selectedDecorator = target as GeNaBuilderDecorator;
        }
        public void OnSceneViewGUI(SceneView sceneView)
        {
            if (m_selectedDecorator == null)
                return;
            ProcessUnityEditorEvent();
            if (m_subSpawnerDecorators != null)
            {
                using (new Handles.DrawingScope())
                {
                    int count = 0;
                    foreach (GeNaSubSpawnerDecorator decorator in m_subSpawnerDecorators)
                    {
                        GeNaSpawner spawner = decorator.SubSpawner;
                        if (spawner == null || !decorator.gameObject.activeInHierarchy)
                            continue;
                        GeNaSpawnerData spawnerData = spawner.SpawnerData;
                        Transform transform = decorator.transform;
                        Vector3 position = transform.position;
                        if (Vector3.Distance(sceneView.camera.transform.position, decorator.transform.position) < m_selectedDecorator.CullingRange)
                        {
                            if (count >= m_selectedDecorator.RenderLimit)
                            {
                                break;
                            }
                            count++;
                            GeNaEditorUtility.RenderSpawnRange(spawnerData, position, transform.eulerAngles.y);
                            if (m_selectedDecorator.RenderLabels)
                            {
                                Vector3 labelPosition = transform.position;
                                labelPosition.y += m_selectedDecorator.LabelOffset;
                                GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
                                {
                                    normal =
                                    {
                                        textColor = Color.red
                                    }
                                };
                                Handles.Label(labelPosition, decorator.name, style);
                            }
                        }
                    }
                }
            }
            if (m_terrainDecorators != null)
            {
                foreach (GeNaTerrainDecorator decorator in m_terrainDecorators)
                {
                    if (decorator == null)
                        continue;
                    Transform transform = decorator.transform;
                    TerrainModifier modifier = decorator.TerrainModifier;
                    GeNaEditorUtility.RenderTerrainModifier(transform, modifier);
                }
            }
        }
        protected override void SettingsPanel(bool helpEnabled)
        {
            if (m_selectedDecorator == null)
                return;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Optimizations", EditorStyles.boldLabel);
            m_selectedDecorator.CullingRange = EditorUtils.FloatField("Culling Distance", m_selectedDecorator.CullingRange, helpEnabled);
            m_selectedDecorator.RenderLimit = EditorUtils.IntField("Render Limit", m_selectedDecorator.RenderLimit, helpEnabled);
            m_selectedDecorator.RenderLabels = EditorUtils.Toggle("Render Labels", m_selectedDecorator.RenderLabels, helpEnabled);
            if (m_selectedDecorator.RenderLabels)
            {
                EditorGUI.indentLevel++;
                m_selectedDecorator.LabelOffset = EditorUtils.FloatField("Label Offset", m_selectedDecorator.LabelOffset, helpEnabled);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Utilities", EditorStyles.boldLabel);
            m_selectedDecorator.RandomizationValue = EditorUtils.Vector2Field("Rotation Min/Max", m_selectedDecorator.RandomizationValue, helpEnabled);
            if (m_selectedDecorator.Spawners.Count < 1)
            {
                GUI.enabled = false;
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(m_selectedDecorator);
            }
            if (EditorUtils.Button("Randomize Rotation"))
            {
                m_selectedDecorator.RandomizeYRotation();
            }
            GUI.enabled = true;
        }
    }
}