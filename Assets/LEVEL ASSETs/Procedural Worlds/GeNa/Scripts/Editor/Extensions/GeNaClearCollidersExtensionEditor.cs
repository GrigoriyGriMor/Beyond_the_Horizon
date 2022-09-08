using System.Collections.Generic;
using PWCommon5;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace GeNa.Core
{
    [CustomEditor(typeof(GeNaClearCollidersExtension))]
    public class GeNaClearCollidersExtensionEditor : GeNaSplineExtensionEditor
    {
        private GeNaClearCollidersExtension m_clearCollidersExtension;
        private ReorderableList m_ignoredReorderable;
        protected void OnEnable()
        {
            if (m_editorUtils == null)
                m_editorUtils = PWApp.GetEditorUtils(this, "GeNaSplineExtensionEditor");
            m_clearCollidersExtension = target as GeNaClearCollidersExtension;
            CreateColliderList();
        }
        /// <summary>
        /// Handle drop area for new objects
        /// </summary>
        public bool DropCollidersGUI()
        { 
            // Ok - set up for drag and drop
            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            string dropMsg = m_editorUtils.GetTextValue("Drop Colliders");
            GUI.Box(dropArea, dropMsg, Styles.gpanel);
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
                        Collider collider = null;
                        switch (draggedObject)
                        {
                            case GameObject go:
                                collider = go.GetComponent<Collider>();
                                break;
                            case Collider col:
                                collider = col;
                                break;
                        }
                        if (collider != null)
                        {
                            ColliderEntry colliderEntry = new ColliderEntry();
                            colliderEntry.Collider = collider;
                            m_clearCollidersExtension.IgnoredColliders.Add(colliderEntry);
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!GeNaEditorUtility.ValidateComputeShader())
            {
                Color guiColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                EditorGUILayout.BeginVertical(Styles.box);
                m_editorUtils.Text("NoComputeShaderHelp");
                EditorGUILayout.EndVertical();
                GUI.backgroundColor = guiColor;
                GUI.enabled = false;
            }
            GeNaClearCollidersExtension clearCollidersExtension = target as GeNaClearCollidersExtension;
            EditorGUI.BeginChangeCheck();
            {
                clearCollidersExtension.Width = m_editorUtils.FloatField("Width", clearCollidersExtension.Width, HelpEnabled);
                clearCollidersExtension.LayerMask = m_editorUtils.LayerMaskField("Layer Mask", clearCollidersExtension.LayerMask, HelpEnabled);
                DropCollidersGUI();
                DrawExtensionList(m_ignoredReorderable, m_editorUtils);
                m_editorUtils.InlineHelp("Ignored Colliders", HelpEnabled);
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
            if (m_editorUtils.Button("Clear Colliders Btn", HelpEnabled))
                clearCollidersExtension.Clear();
        }
        #region Spline Extension Reorderable
        private void CreateColliderList()
        {
            if (m_clearCollidersExtension == null)
                return;
            m_ignoredReorderable = new ReorderableList(m_clearCollidersExtension.IgnoredColliders, typeof(ColliderEntry), true, true, true, true);
            m_ignoredReorderable.elementHeightCallback = OnElementHeightExtensionListEntry;
            m_ignoredReorderable.drawElementCallback = DrawExtensionListElement;
            m_ignoredReorderable.drawHeaderCallback = DrawExtensionListHeader;
            m_ignoredReorderable.onAddCallback = OnAddExtensionListEntry;
            m_ignoredReorderable.onRemoveCallback = OnRemoveExtensionListEntry;
            m_ignoredReorderable.onReorderCallback = OnReorderExtensionList;
        }
        private void OnReorderExtensionList(ReorderableList reorderableList)
        {
            //Do nothing, changing the order does not immediately affect anything in the stamper
        }
        private void OnRemoveExtensionListEntry(ReorderableList reorderableList)
        {
            int indexToRemove = reorderableList.index;
            m_clearCollidersExtension.IgnoredColliders.RemoveAt(indexToRemove);
            reorderableList.list = m_clearCollidersExtension.IgnoredColliders;
            if (indexToRemove >= reorderableList.list.Count)
                indexToRemove = reorderableList.list.Count - 1;
            reorderableList.index = indexToRemove;
        }
        private void OnAddExtensionListEntry(ReorderableList reorderableList)
        {
            m_clearCollidersExtension.IgnoredColliders.Add(new ColliderEntry());
            reorderableList.index = reorderableList.count - 1;
        }
        private void DrawExtensionListHeader(Rect rect)
        {
            DrawExtensionListHeader(rect, true, m_clearCollidersExtension.IgnoredColliders, m_editorUtils);
        }
        private void DrawExtensionListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            ColliderEntry entry = m_clearCollidersExtension.IgnoredColliders[index];
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
        public void DrawExtensionListHeader(Rect rect, bool currentFoldOutState, List<ColliderEntry> extensionList, EditorUtils editorUtils)
        {
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.LabelField(rect, editorUtils.GetContent("Ignored Colliders"));
            EditorGUI.indentLevel = oldIndent;
        }
        public void DrawExtensionList(ReorderableList list, EditorUtils editorUtils)
        {
            Rect maskRect = EditorGUILayout.GetControlRect(true, list.GetHeight());
            list.DoList(maskRect);
        }
        public void DrawExtensionListElement(Rect rect, ColliderEntry entry, EditorUtils editorUtils, bool isFocused)
        {
            // Spawner Object
            EditorGUI.BeginChangeCheck();
            {
                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                EditorGUI.LabelField(new Rect(rect.x, rect.y + 1f, rect.width * 0.18f, EditorGUIUtility.singleLineHeight), editorUtils.GetContent("ColliderEntryActive"));
                entry.IsActive = EditorGUI.Toggle(new Rect(rect.x + rect.width * 0.18f, rect.y, rect.width * 0.1f, EditorGUIUtility.singleLineHeight), entry.IsActive);
                bool oldEnabled = GUI.enabled;
                GUI.enabled = entry.IsActive;                
                entry.Collider = (Collider)EditorGUI.ObjectField(new Rect(rect.x + rect.width * 0.4f, rect.y + 1f, rect.width * 0.6f, EditorGUIUtility.singleLineHeight), entry.Collider, typeof(Collider), false);
                GUI.enabled = oldEnabled;
                EditorGUI.indentLevel = oldIndent;
            }
            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
        }
        #endregion
    }
}