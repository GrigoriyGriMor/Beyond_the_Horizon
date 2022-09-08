using UnityEditor;
using UnityEngine;
namespace GeNa.Core
{
    [CustomEditor(typeof(GeNaCarveExtension))]
    public class GeNaCarveExtensionEditor : GeNaSplineExtensionEditor
    {
        protected void OnEnable()
        {
            if (m_editorUtils == null)
                m_editorUtils = PWApp.GetEditorUtils(this, "GeNaSplineExtensionEditor");
        }
        public override void OnInspectorGUI()
        {
            Initialize();
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
            GeNaCarveExtension carveExtension = target as GeNaCarveExtension;
            carveExtension.Width = m_editorUtils.FloatField("Width", carveExtension.Width, HelpEnabled);
            if (carveExtension.Width < 0.05f)
            {
                carveExtension.Width = 0.05f;
            }
            carveExtension.HeightOffset = m_editorUtils.FloatField("Height Offset", carveExtension.HeightOffset, HelpEnabled);
            carveExtension.Shoulder = m_editorUtils.FloatField("Shoulder", carveExtension.Shoulder, HelpEnabled);
            carveExtension.ShoulderFalloff = m_editorUtils.CurveField("Shoulder Falloff", carveExtension.ShoulderFalloff, HelpEnabled);
            carveExtension.RoadLike = m_editorUtils.Toggle("Road Like", carveExtension.RoadLike, HelpEnabled);
            m_editorUtils.Fractal(carveExtension.MaskFractal, HelpEnabled);
            carveExtension.ShowPreview = m_editorUtils.Toggle("Preview Btn", carveExtension.ShowPreview, HelpEnabled);
            if (m_editorUtils.Button("Carve Btn", HelpEnabled))
                carveExtension.Carve();
        }
    }
}