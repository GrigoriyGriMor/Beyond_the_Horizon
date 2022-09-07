//Copyright(c)2020 Procedural Worlds Pty Limited 
using UnityEngine;
using UnityEditor;
namespace GeNa.Core
{
    [CustomEditor(typeof(GeNaRiverExtension))]
    public class GeNaRiverExtensionEditor : GeNaSplineExtensionEditor
    {
        protected Editor m_riverProfileEditor;
        protected GeNaRiverExtension m_riverExtension;
        protected void OnEnable()
        {
            if (m_editorUtils == null)
                m_editorUtils = PWApp.GetEditorUtils(this, "GeNaSplineExtensionEditor");
            m_riverExtension = target as GeNaRiverExtension;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GlobalPanel();
        }
        private void GlobalPanel()
        {
            bool defaultGUIEnabled = GUI.enabled;
            EditorGUILayout.BeginHorizontal();
            m_editorUtils.LabelField("Tag", GUILayout.MaxWidth(40));
            m_riverExtension.Tag = EditorGUILayout.TagField(m_riverExtension.Tag);
            m_editorUtils.LabelField("Layer", GUILayout.MaxWidth(40));
            m_riverExtension.Layer = EditorGUILayout.LayerField(m_riverExtension.Layer);
            EditorGUILayout.EndHorizontal();
            m_editorUtils.InlineHelp("TagAndLayerHelp", HelpEnabled);
            Constants.RenderPipeline pipeline = GeNaUtility.GetActivePipeline();
            if (pipeline != Constants.RenderPipeline.BuiltIn)
            {
                EditorGUILayout.BeginHorizontal();
                m_riverExtension.CastShadows = m_editorUtils.Toggle("CastShadows", m_riverExtension.CastShadows);
                m_riverExtension.ReceiveShadows = m_editorUtils.Toggle("ReceiveShadows", m_riverExtension.ReceiveShadows);
                m_editorUtils.InlineHelp("ShadowsHelp", HelpEnabled);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
            m_editorUtils.Heading("RiverMeshSettings");
            m_editorUtils.InlineHelp("RiverMeshSettings", HelpEnabled);
            EditorGUI.indentLevel++;
            m_riverExtension.StartFlow = m_editorUtils.FloatField("StartDepth", m_riverExtension.StartFlow, HelpEnabled);
            m_riverExtension.CapDistance = m_editorUtils.FloatField("StartCapDistance", m_riverExtension.CapDistance, HelpEnabled);
            m_riverExtension.EndCapDistance = m_editorUtils.FloatField("EndCapDistance", m_riverExtension.EndCapDistance, HelpEnabled);
            m_riverExtension.RiverWidth = m_editorUtils.FloatField("RiverWidth", m_riverExtension.RiverWidth, HelpEnabled);
            m_riverExtension.VertexDistance = m_editorUtils.Slider("VertexDistance", m_riverExtension.VertexDistance, 1.5f, 8.0f, HelpEnabled);
            m_riverExtension.BankOverstep = m_editorUtils.FloatField("BankOverstep", m_riverExtension.BankOverstep, HelpEnabled);
            if (m_riverExtension.RiverProfile.RiverParameters.m_renderMode == Constants.ProfileRenderMode.PWShader)
            {
                m_riverExtension.UseWorldspaceTextureWidth = m_editorUtils.Toggle("Use Worldspace Width Texturing", m_riverExtension.UseWorldspaceTextureWidth, HelpEnabled);
                GUI.enabled = m_riverExtension.UseWorldspaceTextureWidth;
                EditorGUI.indentLevel++;
                m_riverExtension.WorldspaceWidthRepeat = m_editorUtils.Slider("Worldspace Width Repeat", m_riverExtension.WorldspaceWidthRepeat, 0.5f, 50.0f, HelpEnabled);
                EditorGUI.indentLevel--;
                GUI.enabled = true;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            m_editorUtils.Heading("RiverBehaviourSettings");
            m_editorUtils.InlineHelp("RiverBehaviourSettings", HelpEnabled);
            EditorGUI.indentLevel++;
            if (GeNaUtility.Gaia2Present)
            {
                m_riverExtension.UseGaiaSeaLevel = m_editorUtils.Toggle("UseSeaLevel", m_riverExtension.UseGaiaSeaLevel, HelpEnabled);
                GUI.enabled = !m_riverExtension.UseGaiaSeaLevel;
                EditorGUI.indentLevel++;
                m_riverExtension.SeaLevel = m_editorUtils.FloatField("SeaLevel", m_riverExtension.SeaLevel, HelpEnabled);
                EditorGUI.indentLevel--;
                GUI.enabled = true;
            }
            else
            {
                m_riverExtension.SeaLevel = m_editorUtils.FloatField("SeaLevel", m_riverExtension.SeaLevel, HelpEnabled);
            }
            m_riverExtension.UpdateOnTerrainChange = m_editorUtils.Toggle("Auto-Update On Terrain Change", m_riverExtension.UpdateOnTerrainChange, HelpEnabled);
            m_riverExtension.RaycastTerrainOnly = m_editorUtils.Toggle("RaycastTerrainOnly", m_riverExtension.RaycastTerrainOnly, HelpEnabled);
            m_riverExtension.AddCollider = m_editorUtils.Toggle("AddCollider", m_riverExtension.AddCollider, HelpEnabled);
            m_riverExtension.SplitAtTerrains = m_editorUtils.Toggle("SplitMeshesAtTerrains", m_riverExtension.SplitAtTerrains, HelpEnabled);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            GUI.enabled = m_riverExtension.Spline.gameObject.activeInHierarchy;
            {
                m_editorUtils.Heading("RiverRenderingSettings");
                m_editorUtils.InlineHelp("RiverRenderingSettings", HelpEnabled);
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                m_riverExtension.RiverProfile = (GeNaRiverProfile)m_editorUtils.ObjectField("RiverProfile", m_riverExtension.RiverProfile, typeof(GeNaRiverProfile), false, HelpEnabled);
                if (EditorGUI.EndChangeCheck())
                    m_riverExtension.UpdateMaterial();
                if (m_riverExtension.RiverProfile != null)
                {
                    if (m_riverProfileEditor == null)
                        m_riverProfileEditor = CreateEditor(m_riverExtension.RiverProfile);
                    GeNaRiverProfileEditor.SetProfile(m_riverExtension.RiverProfile, (GeNaRiverProfileEditor)m_riverProfileEditor);
                    EditorGUI.BeginChangeCheck();
                    m_riverProfileEditor.OnInspectorGUI();
                    if (EditorGUI.EndChangeCheck())
                        m_riverExtension.UpdateMaterial();
                }
                if (m_riverExtension.RiverProfile != null && m_riverExtension.RiverProfile.RiverParameters.m_renderMode == Constants.ProfileRenderMode.RiverFlow)
                {
                    if (m_editorUtils.Button("Save Flow Texture"))
                        m_riverExtension.CaptureRiverFlowTexture(true);
                }
                EditorGUI.indentLevel--;
            }
            GUI.enabled = defaultGUIEnabled;
            if (m_editorUtils.Button("MakeSplineDownhill", HelpEnabled))
                m_riverExtension.SetSplineToDownhill();
            if (m_editorUtils.Button("BakeRiver", HelpEnabled))
            {
                if (EditorUtility.DisplayDialog(m_editorUtils.GetTextValue("BakeTitleRiver"), m_editorUtils.GetTextValue("BakeMessageRiver"), "Ok"))
                {
                    m_riverExtension.Bake(true);
                }
                GUIUtility.ExitGUI();
            }
            if (m_riverExtension.HasBakedRivers())
            {
                EditorGUILayout.Space(3);
                if (m_editorUtils.Button("DeleteBakedRiver", HelpEnabled))
                {
                    if (EditorUtility.DisplayDialog(m_editorUtils.GetTextValue("DeleteBakeTitleRiver"), m_editorUtils.GetTextValue("DeleteBakeMessageRiver"), "Ok", "Cancel"))
                    {
                        m_riverExtension.DeleteBakedRiver(true);
                    }
                    GUIUtility.ExitGUI();
                }
            }
        }
    }
}