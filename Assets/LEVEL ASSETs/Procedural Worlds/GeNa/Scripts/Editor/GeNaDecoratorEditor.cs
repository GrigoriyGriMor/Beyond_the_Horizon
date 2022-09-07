using System.Collections.Generic;
using System.Linq;
using PWCommon5;
using UnityEditor;
using UnityEngine;
namespace GeNa.Core
{
    public class GeNaDecoratorEditor : GeNaEditor
    {
        protected static bool m_showCommonPanel = false;
        protected static bool m_showSettingsPanel = true;
        public bool m_showCommonPanelLocal = false;
        public bool m_showSettingsPanelLocal = true;
        public bool m_useStatic = false;
        public virtual bool HideInSpawner => false;
    }
    [CanEditMultipleObjects]
    [CustomEditor(typeof(IDecorator))]
    public abstract class GeNaDecoratorEditor<T> : GeNaDecoratorEditor where T : GeNaDecorator
    {
        #region Variables
        protected bool m_hideDestroyAfterSpawnOption = false;
        protected bool m_hideUnpackPrefabOption = false;
        protected bool m_advanced = false;
        protected bool m_help = false;
        protected bool m_dirty = false;
        protected string m_name = "Decorator";
        protected T m_decorator;
        protected bool m_hasOtherComponents;
        protected bool m_isPrefab;
        protected bool DataWillBeLost => m_isPrefab ? false : m_hasOtherComponents;
        #endregion
        #region Properties
        protected T Decorator => m_decorator;
        protected bool Advanced => m_advanced;
        protected EditorUtils EditorUtils => m_editorUtils;
        #endregion
        #region Methods
        protected virtual void OnEnable()
        {
            if (m_editorUtils == null)
                m_editorUtils = PWApp.GetEditorUtils(this, "SpawnerEditor");
            m_decorator = target as T;
            m_name = typeof(T).Name;
            m_name = m_name.Replace("GeNa", "");
            m_name = ObjectNames.NicifyVariableName(m_name);
            if (m_decorator != null)
            {
                // Detect if attached to an object without any components
                Component[] components = m_decorator.GetComponents<Component>();
                m_hasOtherComponents = components.Any(item => !(item is IDecorator) && !(item is Transform));
                m_isPrefab = GeNaEditorUtility.IsPrefab(m_decorator.gameObject);
            }
        }
        private void CommonPanel(bool helpEnabled)
        {
            EditorGUI.BeginChangeCheck();
            {
                if (!m_hideDestroyAfterSpawnOption)
                    m_decorator.DestroyAfterSpawn = m_editorUtils.Toggle("DecoratorDestroyAfterSpawn", m_decorator.DestroyAfterSpawn, helpEnabled);
                if (!m_hideUnpackPrefabOption)
                    m_decorator.UnpackPrefab = m_editorUtils.Toggle("DecoratorUnpackPrefab", m_decorator.UnpackPrefab, helpEnabled);
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_dirty = true;
            }
        }
        protected virtual void SettingsPanel(bool helpEnabled)
        {
        }
        protected virtual void RenderCommonPanel()
        {
            bool showCommonPanel = m_useStatic ? m_showCommonPanel : m_showCommonPanelLocal;
            showCommonPanel = m_editorUtils.Panel("DecoratorCommonPanel", CommonPanel, showCommonPanel);
            if (m_useStatic)
                m_showCommonPanel = showCommonPanel;
            else
                m_showCommonPanelLocal = showCommonPanel;
        }
        protected virtual void RenderSettingsPanel()
        {
            bool showSettingsPanel = m_useStatic ? m_showSettingsPanel : m_showSettingsPanelLocal;
            showSettingsPanel = m_editorUtils.Panel("DecoratorSettingsPanel", SettingsPanel, showSettingsPanel);
            if (m_useStatic)
                m_showSettingsPanel = showSettingsPanel;
            else
                m_showSettingsPanelLocal = showSettingsPanel;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            m_editorUtils.Initialize();
            if (m_decorator != null)
            {
                GUILayout.BeginVertical(Styles.gpanel);
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(m_name);
                        GUILayout.FlexibleSpace();
                        EditorUtils.ToggleButton("Advanced Toggle", ref m_advanced, Styles.advancedToggle, Styles.advancedToggleDown);
                        EditorUtils.HelpToggle(ref m_help);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                m_editorUtils.InlineHelp($"{m_decorator.GetType().Name}Desc", m_help);
                EditorGUI.indentLevel++;
                RenderCommonPanel();
                RenderSettingsPanel();
                EditorGUI.indentLevel--;
                if (m_dirty)
                {
                    EditorUtility.SetDirty(m_decorator);
                }
            }
        }
        /// <summary>
        /// Handy layer mask interface
        /// </summary>
        /// <param name="label"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static LayerMask LayerMaskField(GUIContent label, LayerMask layerMask)
        {
            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "")
                {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }
            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                    maskWithoutEmpty |= (1 << i);
            }
            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());
            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                    mask |= (1 << layerNumbers[i]);
            }
            layerMask.value = mask;
            return layerMask;
        }
        #endregion
    }
}