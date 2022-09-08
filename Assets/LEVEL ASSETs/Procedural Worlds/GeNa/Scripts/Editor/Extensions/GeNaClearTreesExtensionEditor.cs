using UnityEditor;
using UnityEngine;
namespace GeNa.Core
{
    [CustomEditor(typeof(GeNaClearTreesExtension))]
    public class GeNaClearTreesExtensionEditor : GeNaSplineExtensionEditor
    {
        protected void OnEnable()
        {
            if (m_editorUtils == null)
                m_editorUtils = PWApp.GetEditorUtils(this, "GeNaSplineExtensionEditor");
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
            GeNaClearTreesExtension clearTreesExtension = target as GeNaClearTreesExtension;
            clearTreesExtension.Width = m_editorUtils.FloatField("Width", clearTreesExtension.Width, HelpEnabled);
            clearTreesExtension.Shoulder = m_editorUtils.FloatField("Shoulder", clearTreesExtension.Shoulder, HelpEnabled);
            clearTreesExtension.ShoulderFalloff = m_editorUtils.CurveField("Shoulder Falloff", clearTreesExtension.ShoulderFalloff, HelpEnabled);
            m_editorUtils.Fractal(clearTreesExtension.MaskFractal, HelpEnabled);
            if (m_editorUtils.Button("Clear Trees Btn", HelpEnabled))
                clearTreesExtension.Clear();
        }
    }
}