using UnityEditor;
using UnityEngine;
namespace GeNa.Core
{
    [CustomEditor(typeof(GeNaTerrainExtension))]
    public class GeNaTerrainExtensionEditor : GeNaSplineExtensionEditor
    {
        protected void OnEnable()
        {
            if (m_editorUtils == null)
            {
                m_editorUtils = PWApp.GetEditorUtils(this, "GeNaSplineExtensionEditor");
            }
        }
        public void RenderPanel()
        {
            GeNaTerrainExtension terrainExtension = target as GeNaTerrainExtension;
            terrainExtension.Width = m_editorUtils.FloatField("Width", terrainExtension.Width, HelpEnabled);
            switch (terrainExtension.EffectType)
            {
                case EffectType.Raise:
                case EffectType.Lower:
                case EffectType.Flatten:
                    terrainExtension.HeightOffset = m_editorUtils.FloatField("Height Offset", terrainExtension.HeightOffset, HelpEnabled);
                    break;
            }
            terrainExtension.Strength = m_editorUtils.Slider("Strength", terrainExtension.Strength, 0f, 1f, HelpEnabled);
            terrainExtension.Shoulder = m_editorUtils.FloatField("Shoulder", terrainExtension.Shoulder, HelpEnabled);
            terrainExtension.ShoulderFalloff = m_editorUtils.CurveField("Shoulder Falloff", terrainExtension.ShoulderFalloff, HelpEnabled);
            m_editorUtils.Fractal(terrainExtension.MaskFractal, HelpEnabled);
            if (GUILayout.Button(terrainExtension.EffectType.ToString()))
                terrainExtension.Clear();
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
            Terrain terrain = Terrain.activeTerrain;
            if (terrain != null)
            {
                TerrainData terrainData = terrain.terrainData;
                if (terrainData != null)
                {
                    GeNaTerrainExtension terrainExtension = target as GeNaTerrainExtension;
                    terrainExtension.EffectType = (EffectType)m_editorUtils.EnumPopup("Effect Type", terrainExtension.EffectType, HelpEnabled);
                    switch (terrainExtension.EffectType)
                    {
                        case EffectType.Raise:
                        case EffectType.Lower:
                        case EffectType.Flatten:
                        case EffectType.ClearTrees:
                        case EffectType.ClearDetails:
                            RenderPanel();
                            break;
                        case EffectType.Texture:
                            TerrainLayer[] terrainLayers = terrainData.terrainLayers;
                            if (terrainLayers.Length > 0)
                            {
                                int alphamapLayers = terrainData.alphamapLayers;
                                GUIContent[] choices = new GUIContent[alphamapLayers];
                                for (int assetIdx = 0; assetIdx < choices.Length; assetIdx++)
                                {
                                    TerrainLayer terrainLayer = terrainLayers[assetIdx];
                                    var diffuseTexture = terrainLayer.diffuseTexture;
                                    var normalMapTexture = terrainLayer.normalMapTexture;
                                    var maskMapTexture = terrainLayer.maskMapTexture;
                                    string name = "Unknown Asset";
                                    if (diffuseTexture != null)
                                        name = diffuseTexture.name;
                                    else if (normalMapTexture != null)
                                        name = normalMapTexture.name;
                                    else if (maskMapTexture != null)
                                        name = maskMapTexture.name;
                                    if (terrainLayer.diffuseTexture != null)
                                        name = terrainLayer.diffuseTexture.name;
                                    choices[assetIdx] = new GUIContent(name);
                                }
                                terrainExtension.TextureProtoIndex = m_editorUtils.Popup("Texture", terrainExtension.TextureProtoIndex, choices, HelpEnabled);
                                RenderPanel();
                            }
                            else
                            {
                                m_editorUtils.Label("Missing Terrain Layers", HelpEnabled);
                            }
                            break;
                        case EffectType.Detail:
                            DetailPrototype[] detailPrototypes = terrainData.detailPrototypes;
                            if (detailPrototypes.Length > 0)
                            {
                                GUIContent[] choices = new GUIContent[detailPrototypes.Length];
                                for (int assetIdx = 0; assetIdx < choices.Length; assetIdx++)
                                {
                                    DetailPrototype detailProto = detailPrototypes[assetIdx];
                                    var prefab = detailProto.prototype;
                                    var texture = detailProto.prototypeTexture;
                                    string name = "Unknown Asset";
                                    if (prefab != null)
                                        name = prefab.name;
                                    else if (texture != null)
                                        name = texture.name;
                                    choices[assetIdx] = new GUIContent(name);
                                }
                                terrainExtension.DetailProtoIndex = m_editorUtils.Popup("Details", terrainExtension.DetailProtoIndex, choices, HelpEnabled);
                                RenderPanel();
                            }
                            else
                            {
                                m_editorUtils.Label("Missing Terrain Details", HelpEnabled);
                            }
                            break;
                    }
                }
                else
                {
                    m_editorUtils.Label("Missing TerrainData", HelpEnabled);
                }
            }
            else
            {
                m_editorUtils.Label("Missing Terrain", HelpEnabled);
            }
        }
    }
}