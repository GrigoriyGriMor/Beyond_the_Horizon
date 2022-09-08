using UnityEditor;
using UnityEngine;
namespace GeNa.Core
{
    [CustomEditor(typeof(GeNaClearDetailsExtension))]
    public class GeNaClearDetailsExtensionEditor : GeNaSplineExtensionEditor
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
            GeNaClearDetailsExtension clearDetailsExtension = target as GeNaClearDetailsExtension;
            clearDetailsExtension.Width = m_editorUtils.FloatField("Width", clearDetailsExtension.Width, HelpEnabled);
            clearDetailsExtension.Shoulder = m_editorUtils.FloatField("Shoulder", clearDetailsExtension.Shoulder, HelpEnabled);
            clearDetailsExtension.ShoulderFalloff = m_editorUtils.CurveField("Shoulder Falloff", clearDetailsExtension.ShoulderFalloff, HelpEnabled);
            m_editorUtils.Fractal(clearDetailsExtension.MaskFractal, HelpEnabled);
            if (m_editorUtils.Button("Clear Details Btn", HelpEnabled))
                clearDetailsExtension.Clear();
        }
    }
}