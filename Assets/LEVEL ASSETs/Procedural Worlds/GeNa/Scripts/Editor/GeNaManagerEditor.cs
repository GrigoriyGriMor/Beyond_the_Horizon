﻿// Copyright © 2020 Procedural Worlds Pty Limited. All Rights Reserved.
using UnityEngine;
using UnityEditor;
using PWCommon5;
namespace GeNa.Core
{
    /// <summary>
    /// Main Workflow Editor Window
    /// </summary>
    public class GeNaManagerEditor : EditorWindow, IPWEditor
    {
        #region Variables
        #region Static
        private static GeNaStyles m_styles;
        #endregion
        #region GUI
        private EditorUtils m_editorUtils;
        private Vector2 m_scrollPosition;
        #endregion
        #region Preferences
        private bool showNews;
        private bool enableTODLightSync;
        private bool previewInEditor;
        private LightShadows lightShadowType;
        private Constants.LightSyncCullingMode lightCullingMode;
        private float cullingDitance;
        private int cullingWaitTime;
        #endregion
        // Core
        private GeNaManager m_manager;
        private GeNaManagerSettings m_settings;
        private TabSet m_mainTabs;
        public bool PositionChecked { get; set; }
        #endregion
        #region Custom Menu Items
        /// <summary>
        /// 
        /// </summary>
        [MenuItem("Window/" + PWConst.COMMON_MENU + "/GeNa/GeNa Manager...", false, 40)]
        public static void MenuGeNaMainWindow()
        {
            GeNaManagerEditor window = EditorWindow.GetWindow<GeNaManagerEditor>(false, "GeNa Manager");
            window.Show();
        }
        #endregion
        #region Constructors destructors and related delegates
        private void OnDestroy()
        {
            m_editorUtils?.Dispose();
        }
        private void OnEnable()
        {
            minSize = new Vector2(400f, 500f);
            // If there isn't any Editor Utils Initialized
            if (m_editorUtils == null)
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this, null, null, null);
            // If there is no target associated with Editor Script
            // Get target Spline
            m_manager = GeNaManager.GetInstance();
            if (m_manager == null)
                return;
            m_settings = m_manager.Settings;
            //Hide its m_transform
            m_manager.transform.hideFlags = HideFlags.HideInInspector;
            m_manager.Initialize();
            Tab[] tabs =
            {
                new Tab("ManagerTab", ManagerTab),
                new Tab("PreferencesTab", PreferencesTab),
            };
            m_mainTabs = new TabSet(m_editorUtils, tabs);
            SetupNewsPref();
            LoadPreferences();
        }
        #endregion
        #region GUI main
        /// <summary>
        /// OnGUI
        /// </summary>
        private void OnGUI()
        {
            m_editorUtils.Initialize(); // Do not remove this!
            m_editorUtils.GUIHeader();
            m_editorUtils.GUINewsHeader();
            InitializeGUI();
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, false);
            {
                // Add content here
                m_editorUtils.Tabs(m_mainTabs);
            }
            GUILayout.EndScrollView();
            m_editorUtils.GUINewsFooter();
        }
        #endregion
        #region Preferences Settings
        /// <summary>
        /// General Settings Tab
        /// </summary>
        private void PreferencesTab()
        {
            m_editorUtils.Panel("Prefrences Panel", PreferencesPanel, true);
        }
        /// <summary>
        /// Preferences Panel
        /// </summary>
        /// <param name="helpEnabled"></param>
        private void PreferencesPanel(bool helpEnabled)
        {
            if (!EditorPrefs.HasKey(Constants.PW_SHOW_NEWS + PWApp.CONF.NameSpace))
                EditorPrefs.SetBool(Constants.PW_SHOW_NEWS + PWApp.CONF.NameSpace, false);
            else
            {
                showNews = EditorPrefs.GetBool(Constants.PW_SHOW_NEWS + PWApp.CONF.NameSpace);
                EditorGUI.BeginChangeCheck();
                m_editorUtils.Text("NewsInfo");
                EditorGUILayout.BeginHorizontal();
                showNews = m_editorUtils.Toggle("ShowNews", showNews);
                if (m_editorUtils.Button("UpdateNewsNow"))
                {
                    m_editorUtils.ForceNewsUpdate();
                }
                EditorGUILayout.EndHorizontal();
                m_editorUtils.InlineHelp("ShowNews", helpEnabled);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool(Constants.PW_SHOW_NEWS + PWApp.CONF.NameSpace, showNews);
                }
            }
            m_editorUtils.Panel("Adv Spawn Panel", SpawnPanel, true);
            m_editorUtils.Panel("ComponentExtensionsPanel", ExtensionSystems, true);
            if (m_editorUtils.Button("RevertToDefaults"))
            {
                if (EditorUtility.DisplayDialog("Reverting Preferences", "You are about to revert your preferences this will reset them back there default value. Are you sure you want to proceed?", "Yes", "No"))
                {
                    RevertPrefsToDefaults();
                }
            }
        }
        /// <summary>
        /// Spawning Panel
        /// </summary>
        private void SpawnPanel(bool helpActive)
        {
            bool spawnToTarget = Preferences.DefaultSpawnToTarget;
            EditorGUI.BeginChangeCheck();
            {
                float lw = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 220f;
                spawnToTarget = m_editorUtils.Toggle("Default Spawn To Target", spawnToTarget, helpActive);
                EditorGUIUtility.labelWidth = lw;
            }
            if (EditorGUI.EndChangeCheck())
            {
                Preferences.DefaultSpawnToTarget = spawnToTarget;
            }
        }
        /// <summary>
        /// Extension Component System Panel
        /// </summary>
        /// <param name="helpEnabled"></param>
        private void ExtensionSystems(bool helpEnabled)
        {
            if (m_manager == null)
                m_manager = GeNaManager.GetInstance();
            enableTODLightSync = m_manager.EnableTimeOfDayLightSync;
            previewInEditor = m_manager.PreviewSyncLightCullingInEditor;
            lightShadowType = m_manager.TimeOfDayLightSyncShadowMode;
            lightCullingMode = m_manager.LightCullingMode;
            cullingDitance = m_manager.LightCullingDistance;
            cullingWaitTime = m_manager.CullingWaitForFrames;
            EditorGUI.BeginChangeCheck();
            {
                enableTODLightSync = m_editorUtils.Toggle("EnableTimeOfDayLightSync", enableTODLightSync, helpEnabled);
                if (enableTODLightSync)
                {
                    EditorGUI.indentLevel++;
                    m_editorUtils.Heading("Setup");
                    lightShadowType = (LightShadows)m_editorUtils.EnumPopup("TimeOfDayLightSyncShadowType", lightShadowType, helpEnabled);
                    previewInEditor = m_editorUtils.Toggle("PreviewInEditor", previewInEditor, helpEnabled);
                    if (previewInEditor)
                    {
                        m_editorUtils.Text("PreviewInEditorText");
                    }
                    m_editorUtils.Heading("Culling");
                    lightCullingMode = (Constants.LightSyncCullingMode)m_editorUtils.EnumPopup("TimeOfDayLightSyncCullingMode", lightCullingMode, helpEnabled);
                    cullingDitance = m_editorUtils.FloatField("TimeOfDayLightSyncCullingDistance", cullingDitance, helpEnabled);
                    if (cullingDitance < 0.1f)
                    {
                        cullingDitance = 0.1f;
                    }
                    // cullingWaitTime = m_editorUtils.IntField("TimeOfDayLightSyncCullingWaitTime", cullingWaitTime, helpEnabled);
                    // if (cullingWaitTime < 1)
                    // {
                    //     cullingWaitTime = 1;
                    // }
                    EditorGUI.indentLevel--;
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                //Setup
                EditorPrefs.SetBool(Constants.ENABLE_GAIA_TIME_OF_DAY_LIGHT_SYNC, enableTODLightSync);
                m_manager.EnableTimeOfDayLightSync = enableTODLightSync;
                EditorPrefs.SetInt(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_SHADOWS_MODE, (int)lightShadowType);
                m_manager.TimeOfDayLightSyncShadowMode = lightShadowType;
                EditorPrefs.SetBool(Constants.SET_GAIA_TIME_OF_DAY_PREVIEW_IN_EDITOR, m_manager.PreviewSyncLightCullingInEditor);
                m_manager.PreviewSyncLightCullingInEditor = previewInEditor;

                //Culling
                EditorPrefs.SetInt(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_CULLING_MODE, (int)lightCullingMode);
                m_manager.LightCullingMode = lightCullingMode;
                EditorPrefs.SetFloat(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_CULLING_DISTANCE, cullingDitance);
                m_manager.LightCullingDistance = cullingDitance;
                EditorPrefs.SetInt(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_CULLING_WAIT_FOR_FRAMES, cullingWaitTime);
                m_manager.CullingWaitForFrames = cullingWaitTime;
                GeNaEvents.UpdateTimeOfDaySyncCulling();
            }
        }
        /// <summary>
        /// Reverts all the prefs to there default value
        /// </summary>
        public static void RevertPrefsToDefaults()
        {
            //Spawning
            Preferences.DefaultSpawnToTarget = true;

            //Undo
            GeNaManager manager = GeNaManager.GetInstance();
            //Component Extension
            EditorPrefs.SetBool(Constants.ENABLE_GAIA_TIME_OF_DAY_LIGHT_SYNC, false);
            manager.EnableTimeOfDayLightSync = false;
            EditorPrefs.SetInt(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_SHADOWS_MODE, 0);
            manager.TimeOfDayLightSyncShadowMode = LightShadows.None;
            EditorPrefs.SetInt(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_CULLING_MODE, 0);
            manager.LightCullingMode = Constants.LightSyncCullingMode.ShadowOnly;
            EditorPrefs.SetFloat(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_CULLING_DISTANCE, 70F);
            manager.LightCullingDistance = 70f;
            EditorPrefs.SetInt(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_CULLING_WAIT_FOR_FRAMES, 100);
            manager.CullingWaitForFrames = 100;

            //News
            EditorPrefs.SetBool(Constants.PW_SHOW_NEWS + PWApp.CONF.NameSpace, false);
            EditorPrefs.SetBool(Constants.ENABLE_NEWS_EDITOR_PREFS, false);
        }
        #endregion
        #region Manager Settings
        /// <summary>
        /// Advanced Settings Tab
        /// </summary>
        private void ManagerTab()
        {
            TutorialPanel();
            m_editorUtils.Panel("ShowCreateButtons", ShowCreateButtons, true);
        }
        /// <summary>
        /// Tutorial panel
        /// </summary>
        private void TutorialPanel()
        {
            // if (ActiveEditorTracker.sharedTracker.isLocked)
            // {
            //     EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("Inspector locked warning"), MessageType.Warning);
            // }
            GUILayout.BeginVertical(m_editorUtils.Styles.panelFrame);
            {
                GUILayout.BeginHorizontal();
                {
                    m_editorUtils.Label("Quick Start", m_styles.helpNoWrap);
                    GUILayout.FlexibleSpace();
                    bool show = m_settings.ShowQuickStart;
                    m_editorUtils.HelpToggle(ref show);
                    if (show != m_settings.ShowQuickStart)
                    {
                        EditorPrefs.SetBool("GeNa_DoShowQuickStart", show);
                        m_settings.ShowQuickStart = show;
                    }
                }
                GUILayout.EndHorizontal();
                if (m_settings.ShowQuickStart)
                {
                    if (m_settings.ShowDetailedHelp)
                    {
                        m_editorUtils.Label("Spline Help", m_editorUtils.Styles.help);
                    }
                    else
                    {
                        EditorGUILayout.LabelField(
                            "Spawn Nodes: Ctrl + Left click.\nSingle Spawn: Ctrl + Left click.\nGlobal Spawn: Ctrl + Shift + Left click.",
                            m_styles.wrappedText);
                    }
                    if (m_editorUtils.Button("View Tutorials Btn"))
                    {
                        Application.OpenURL(PWApp.CONF.TutorialsLink);
                    }
                }
            }
            GUILayout.EndVertical();
        }
        /// <summary>
        /// Shows the gena create buttons
        /// </summary>
        private void ShowCreateButtons(bool helpEnabled)
        {
            m_editorUtils.InlineHelp("GettingSpartedSpawnerButtons", helpEnabled);
            if (m_editorUtils.Button("CreateSpawner"))
                GeNaEditorUtility.AddSpawner();
            if (m_editorUtils.Button("CreateSpline"))
                GeNaEditorUtility.AddSpline();
            if (m_editorUtils.Button("CreateRoadSpline"))
                GeNaRoadExtensionEditor.AddRoadSpline(null);
            if (m_editorUtils.Button("CreateRiverFlow"))
                GeNaRiverFlow2.AddRiverFlow(null);
            EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("UpgradeVisualizationShaderHelp"), MessageType.Info);
            if (m_editorUtils.Button("UpgradeVisualizationShader"))
                GeNaEditorUtility.GetVisulizationMaterial(true);
            m_editorUtils.Panel("Debugging", OverviewPanel, false);
            if (GUILayout.Button("Perform Maintainence"))
                GeNaEditorUpgrader.PerformUpgradeOperation();
        }
        #endregion
        #region Utilities
        /// <summary>
        /// Makes the Scene View Focus on a Given Point and Size
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        public void FocusPosition(Vector3 pos, Vector3 size)
        {
            SceneView.lastActiveSceneView.Frame(new Bounds(pos, size), false);
        }
        /// <summary>
        /// Function used for first time setup to enable or disable news
        /// </summary>
        private void SetupNewsPref()
        {
            if (!EditorPrefs.HasKey(Constants.ENABLE_NEWS_EDITOR_PREFS))
                EditorPrefs.SetBool(Constants.ENABLE_NEWS_EDITOR_PREFS, false);
            if (!EditorPrefs.GetBool(Constants.ENABLE_NEWS_EDITOR_PREFS))
            {
                //if (EditorUtility.DisplayDialog("GeNa Pro News", "Would you like GeNa pro to display the latest news and updates in GeNa systems?", "Yes", "No"))
                if (EditorUtility.DisplayDialog(m_editorUtils.GetTextValue("GeNaProNews"), m_editorUtils.GetTextValue("GeNaProNewsText"), "Yes", "No"))
                    EditorPrefs.SetBool(Constants.PW_SHOW_NEWS + PWApp.CONF.NameSpace, true);
                else
                    EditorPrefs.SetBool(Constants.PW_SHOW_NEWS + PWApp.CONF.NameSpace, false);
                m_editorUtils = PWApp.GetEditorUtils(this, null, null, null);
                EditorPrefs.SetBool(Constants.ENABLE_NEWS_EDITOR_PREFS, true);
            }
        }
        #endregion
        #region Panels
        private void OverviewPanel(bool helpEnabled)
        {
            EditorGUI.BeginChangeCheck();
            {
                m_manager.DebugEnabled = m_editorUtils.Toggle("Debug Enabled", m_manager.DebugEnabled, helpEnabled);
                if (m_manager.DebugEnabled)
                {
                    m_manager.DebugLabel = m_editorUtils.Toggle("Debug Label", m_manager.DebugLabel, helpEnabled);
                    m_manager.RenderType = (AabbManager.RenderType)m_editorUtils.EnumPopup("Render Type", m_manager.RenderType, helpEnabled);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
        }
        #endregion
        #region PhysicsSimulator
        #endregion
        #region Static
        /// <summary>
        /// Loads editor prefs
        /// </summary>
        public static void LoadPreferences()
        {
            //Spawning
            ValidateEditorPref(Defaults.DEF_SPAWN_TO_TARGET_KEY, Constants.EditorPrefsType.Bool, true);
            Preferences.DefaultSpawnToTarget = EditorPrefs.GetBool(Defaults.DEF_SPAWN_TO_TARGET_KEY);

            //Undo
            GeNaManager manager = GeNaGlobalReferences.GeNaManagerInstance;
            if (manager != null)
            {
                //Extensions
                ValidateEditorPref(Constants.ENABLE_GAIA_TIME_OF_DAY_LIGHT_SYNC, Constants.EditorPrefsType.Bool, false);
                manager.EnableTimeOfDayLightSync = EditorPrefs.GetBool(Constants.ENABLE_GAIA_TIME_OF_DAY_LIGHT_SYNC);
                ValidateEditorPref(Constants.SET_GAIA_TIME_OF_DAY_PREVIEW_IN_EDITOR, Constants.EditorPrefsType.Bool, false);
                manager.PreviewSyncLightCullingInEditor = EditorPrefs.GetBool(Constants.SET_GAIA_TIME_OF_DAY_PREVIEW_IN_EDITOR);
                ValidateEditorPref(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_SHADOWS_MODE, Constants.EditorPrefsType.Int, 0);
                manager.TimeOfDayLightSyncShadowMode = (LightShadows)EditorPrefs.GetInt(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_SHADOWS_MODE);
                ValidateEditorPref(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_CULLING_MODE, Constants.EditorPrefsType.Int, 0);
                manager.LightCullingMode = (Constants.LightSyncCullingMode)EditorPrefs.GetInt(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_CULLING_MODE);
                ValidateEditorPref(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_CULLING_DISTANCE, Constants.EditorPrefsType.Float, 70f);
                manager.LightCullingDistance = EditorPrefs.GetFloat(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_CULLING_DISTANCE);
                ValidateEditorPref(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_CULLING_WAIT_FOR_FRAMES, Constants.EditorPrefsType.Int, 100);
                manager.CullingWaitForFrames = EditorPrefs.GetInt(Constants.SET_GAIA_TIME_OF_DAY_LIGHT_CULLING_WAIT_FOR_FRAMES);
            }

            //News
            ValidateEditorPref(Constants.PW_SHOW_NEWS, Constants.EditorPrefsType.Bool, false);
            ValidateEditorPref(Constants.ENABLE_NEWS_EDITOR_PREFS, Constants.EditorPrefsType.Bool, false);
        }
        /// <summary>
        /// Used to check if the editor prefs key exists
        /// If not it will create one
        /// </summary>
        /// <param name="prefName"></param>
        /// <param name="prefsType"></param>
        /// <param name="prefsBool"></param>
        /// <param name="prefsFloat"></param>
        /// <param name="prefsInt"></param>
        /// <param name="prefsString"></param>
        public static void ValidateEditorPref(string prefName, Constants.EditorPrefsType prefsType, bool prefsBool = false, float prefsFloat = 0f, int prefsInt = 0, string prefsString = "")
        {
            if (!EditorPrefs.HasKey(prefName))
            {
                switch (prefsType)
                {
                    case Constants.EditorPrefsType.Bool:
                        EditorPrefs.SetBool(prefName, prefsBool);
                        break;
                    case Constants.EditorPrefsType.Float:
                        EditorPrefs.SetFloat(prefName, prefsFloat);
                        break;
                    case Constants.EditorPrefsType.Int:
                        EditorPrefs.SetInt(prefName, prefsInt);
                        break;
                    case Constants.EditorPrefsType.String:
                        EditorPrefs.SetString(prefName, prefsString);
                        break;
                }
            }
        }
        /// <summary>
        /// Used to check if the editor prefs key exists
        /// If not it will create one
        /// </summary>
        /// <param name="prefName"></param>
        /// <param name="prefsType"></param>
        /// <param name="prefsFloat"></param>
        /// <param name="prefsBool"></param>
        /// <param name="prefsInt"></param>
        /// <param name="prefsString"></param>
        public static void ValidateEditorPref(string prefName, Constants.EditorPrefsType prefsType, float prefsFloat = 0f, bool prefsBool = false, int prefsInt = 0, string prefsString = "")
        {
            if (!EditorPrefs.HasKey(prefName))
            {
                switch (prefsType)
                {
                    case Constants.EditorPrefsType.Bool:
                        EditorPrefs.SetBool(prefName, prefsBool);
                        break;
                    case Constants.EditorPrefsType.Float:
                        EditorPrefs.SetFloat(prefName, prefsFloat);
                        break;
                    case Constants.EditorPrefsType.Int:
                        EditorPrefs.SetInt(prefName, prefsInt);
                        break;
                    case Constants.EditorPrefsType.String:
                        EditorPrefs.SetString(prefName, prefsString);
                        break;
                }
            }
        }
        /// <summary>
        /// Used to check if the editor prefs key exists
        /// If not it will create one
        /// </summary>
        /// <param name="prefName"></param>
        /// <param name="prefsType"></param>
        /// <param name="prefsInt"></param>
        /// <param name="prefsBool"></param>
        /// <param name="prefsFloat"></param>
        /// <param name="prefsString"></param>
        public static void ValidateEditorPref(string prefName, Constants.EditorPrefsType prefsType, int prefsInt = 0, bool prefsBool = false, float prefsFloat = 0, string prefsString = "")
        {
            if (!EditorPrefs.HasKey(prefName))
            {
                switch (prefsType)
                {
                    case Constants.EditorPrefsType.Bool:
                        EditorPrefs.SetBool(prefName, prefsBool);
                        break;
                    case Constants.EditorPrefsType.Float:
                        EditorPrefs.SetFloat(prefName, prefsFloat);
                        break;
                    case Constants.EditorPrefsType.Int:
                        EditorPrefs.SetInt(prefName, prefsInt);
                        break;
                    case Constants.EditorPrefsType.String:
                        EditorPrefs.SetString(prefName, prefsString);
                        break;
                }
            }
        }
        /// <summary>
        /// Used to check if the editor prefs key exists
        /// If not it will create one
        /// </summary>
        /// <param name="prefName"></param>
        /// <param name="prefsType"></param>
        /// <param name="prefsString"></param>
        /// <param name="prefsBool"></param>
        /// <param name="prefsFloat"></param>
        /// <param name="prefsInt"></param>
        public static void ValidateEditorPref(string prefName, Constants.EditorPrefsType prefsType, string prefsString = "", bool prefsBool = false, float prefsFloat = 0f, int prefsInt = 0)
        {
            if (!EditorPrefs.HasKey(prefName))
            {
                switch (prefsType)
                {
                    case Constants.EditorPrefsType.Bool:
                        EditorPrefs.SetBool(prefName, prefsBool);
                        break;
                    case Constants.EditorPrefsType.Float:
                        EditorPrefs.SetFloat(prefName, prefsFloat);
                        break;
                    case Constants.EditorPrefsType.Int:
                        EditorPrefs.SetInt(prefName, prefsInt);
                        break;
                    case Constants.EditorPrefsType.String:
                        EditorPrefs.SetString(prefName, prefsString);
                        break;
                }
            }
        }
        /// <summary>
        /// Editor UX
        /// </summary>
        public static void InitializeGUI()
        {
            if (m_styles == null || m_styles.Inited == false)
            {
                m_styles?.Dispose();
                m_styles = new GeNaStyles();
            }
        }
        #endregion
    }
}