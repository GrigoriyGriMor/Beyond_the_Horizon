using UnityEditor;

namespace GeNa.Core 
{
    [InitializeOnLoad]
    public class GeNaRiverScriptDefine
    {
       
        static GeNaRiverScriptDefine()
        {

            SetupGeNaPipelineDefine();
        }

        public static void SetupGeNaPipelineDefine()
        {
            bool updateDefines = false;
            string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            switch (GeNaUtility.GetActivePipeline())
            {
                case Constants.RenderPipeline.BuiltIn:
                    if (defineSymbols.Contains("GeNa_URP"))
                    {
                        updateDefines = true;
                        defineSymbols = defineSymbols.Replace("GeNa_URP", "");
                    }
                    if (defineSymbols.Contains("GeNa_HDRP"))
                    {
                        updateDefines = true;
                        defineSymbols = defineSymbols.Replace("GeNa_HDRP", "");
                    }
                    break;
                case Constants.RenderPipeline.Universal:
                    if (!defineSymbols.Contains("GeNa_URP"))
                    {
                        updateDefines = true;
                        defineSymbols += ";GeNa_URP";
                    }
                    if (defineSymbols.Contains("GeNa_HDRP"))
                    {
                        updateDefines = true;
                        defineSymbols = defineSymbols.Replace("GeNa_HDRP", "");
                    }
                    break;
                case Constants.RenderPipeline.HighDefinition:
                    if (defineSymbols.Contains("GeNa_URP"))
                    {
                        updateDefines = true;
                        defineSymbols = defineSymbols.Replace("GeNa_URP", "");
                    }
                    if (!defineSymbols.Contains("GeNa_HDRP"))
                    {
                        updateDefines = true;
                        defineSymbols += ";GeNa_HDRP";
                    }
                    break;

            }

            if (updateDefines)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defineSymbols);
            }
        }

    }

}

