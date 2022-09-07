﻿#if EXPANSE_PRESENT
using HDRP.Lightweight;
#endif
using UnityEngine;
#if GeNa_HDRP
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
#if GAIA_PRO_PRESENT
using Gaia;
#endif
namespace GeNa.Core
{
    public enum EmissionRenderType
    {
        GameObject,
        Material,
        Both,
        None
    }
    public enum SkyTypeMode
    {
        Gaia,
        Expanse
    }
    [ExecuteAlways]
    public class GaiaTimeOfDayLightSync : MonoBehaviour
    {
        #region Variables
        #region Public
        public SkyTypeMode m_skyMode = SkyTypeMode.Gaia;
        public bool m_overrideSystemActiveState = false;
        public bool SystemActive
        {
            get => m_systemActive;
            set
            {
                if (m_systemActive != value)
                {
                    m_systemActive = value;
                    if (!m_systemActive)
                    {
                        m_isNight = false;
                    }
                    else
                    {
                        CheckNightStatus();
                    }
                    SetLightStatus(false, false);
                }
            }
        }
        [HideInInspector]
        [SerializeField] private bool m_systemActive = false;
        public LightShadows LightShadowMode
        {
            get => m_lightShadowMode;
            set
            {
                if (m_lightShadowMode != value)
                {
                    m_lightShadowMode = value;
                    if (!m_systemActive)
                    {
                        m_isNight = false;
                    }
                    else
                    {
                        CheckNightStatus();
                    }
                    SetLightStatus(false, false);
                }
            }
        }
        [HideInInspector]
        [SerializeField] private LightShadows m_lightShadowMode = LightShadows.None;
        public Constants.LightSyncCullingMode LightCullingMode
        {
            get => m_lightCullingMode;
            set
            {
                if (m_lightCullingMode != value)
                {
                    m_lightCullingMode = value;
                    SetLightStatus(false, false);
                }
            }
        }
        [HideInInspector]
        [SerializeField] private Constants.LightSyncCullingMode m_lightCullingMode = Constants.LightSyncCullingMode.ShadowOnly;
        public float LightCullingDistance
        {
            get => m_lightCullingDistance;
            set
            {
                if (m_lightCullingDistance != value)
                {
                    m_lightCullingDistance = value;
                    SetLightStatus(false, false);
                }
            }
        }
        [HideInInspector]
        [SerializeField] private float m_lightCullingDistance = 70f;
        public float LightEmissionCullingDistance
        {
            get => m_lightEmissionCullingDistance;
            set
            {
                if (m_lightEmissionCullingDistance != value)
                {
                    m_lightEmissionCullingDistance = value;
                    SetLightStatus(false, false);
                }
            }
        }
        [HideInInspector]
        [SerializeField] private float m_lightEmissionCullingDistance = 250f;
        public int CullingWaitForFrames
        {
            get => m_cullingWaitForFrames;
            set
            {
                if (m_cullingWaitForFrames != value)
                {
                    m_cullingWaitForFrames = value;
                    SetLightStatus(false, false);
                }
            }
        }
        [HideInInspector]
        [SerializeField] private int m_cullingWaitForFrames = 100;
        public Light m_lightComponent;
        public GameObject m_lightEmissionObject;
        public Material m_emissionMaterial;
        public string m_enableEmissioKeyWord = "_EMISSION";
        public EmissionRenderType m_emissionRenderType = EmissionRenderType.GameObject;
        [HideInInspector]
        public bool PreviewSyncLightCullingInEditor = false;
        #endregion
        #region Private
        private Transform m_player;
        private Transform m_sceneCamera;
        private bool m_isNight;
#if GeNa_HDRP
        private HDAdditionalLightData m_lightData;
#endif
        private bool m_lastLightState = false;
        [SerializeField] private bool m_genaManagerExists = false;
        //private bool m_currentStatus = false;
        //private int m_currentCullingWaitForFrames;
        [HideInInspector]
        [SerializeField] private bool m_lightComponentExists = false;
        [HideInInspector]
        [SerializeField] private bool m_lightEmissionObjectExists = false;
        [HideInInspector]
        [SerializeField] private bool m_lightEmissionMaterialExists = false;
        [HideInInspector]
        [SerializeField] private bool m_playerExists = false;
        [HideInInspector]
        [SerializeField] private bool m_gaiaProExists = false;
        #endregion
        #endregion
        #region Unity Functions
        private void OnEnable()
        {
            ValidateComponents();
            LoadPreferences(GeNaGlobalReferences.GeNaManagerInstance);
#if UNITY_EDITOR
            AssemblyReloadEvents.afterAssemblyReload -= SubscribeToEvents;
            AssemblyReloadEvents.afterAssemblyReload += SubscribeToEvents;
            SubscribeToEvents();
            if (Application.isPlaying)
            {
                AssemblyReloadEvents.afterAssemblyReload -= SubscribeToEvents;
                EditorApplication.update -= UpdateLights;
            }
#endif
        }
#if UNITY_EDITOR
        private void SubscribeToEvents()
        {
            EditorApplication.update -= UpdateLights;
            EditorApplication.update += UpdateLights;
        }
#endif
#if UNITY_EDITOR
        private void OnDestroy()
        {
            AssemblyReloadEvents.afterAssemblyReload -= SubscribeToEvents;
            EditorApplication.update -= UpdateLights;
        }
        private void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= SubscribeToEvents;
            EditorApplication.update -= UpdateLights;
        }
#endif
        private void LateUpdate()
        {
            UpdateLights();
        }
        public void UpdateLights()
        {
            if (!Application.isPlaying)
            {
                if (m_sceneCamera == null)
                {
                    m_sceneCamera = GetEditorPlayer();
                }
            }
            if (!m_overrideSystemActiveState)
            {
                if (m_lightComponentExists)
                {
                    if (!SystemActive)
                    {
                        m_isNight = false;
                    }
                    else
                    {
                        CheckNightStatus();
                    }
                    SetLightStatus();
                }
            }
        }
        #endregion
        #region Public Functions
        /// <summary>
        /// Validates the components in this system
        /// </summary>
        public void ValidateComponents(Transform player = null)
        {
            if (m_lightComponent == null)
            {
                m_lightComponent = GetComponent<Light>();
            }
            m_lightComponentExists = m_lightComponent != null;
#if GeNa_HDRP
            if (m_lightComponentExists)
            {
                m_lightData = m_lightComponent.GetComponentsInParent<HDAdditionalLightData>(true)[0];
            }
            m_lightComponentExists = m_lightData != null;
#endif
            m_lightEmissionObjectExists = m_lightEmissionObject != null;
            m_lightEmissionMaterialExists = m_emissionMaterial != null;
            if (player == null)
            {
                if (m_player == null)
                {
                    m_player = GetPlayer();
                }
            }
            else
            {
                m_player = player;
            }
            m_genaManagerExists = GeNaGlobalReferences.GeNaManagerInstance != null;
            m_playerExists = m_player != null;
#if GAIA_PRO_PRESENT
            m_gaiaProExists = ProceduralWorldsGlobalWeather.Instance != null;
#else
            m_gaiaProExists = false;
#endif
#if EXPANSE_PRESENT
            ExpanseController controller = ExpanseController.Instance;
            if (controller != null)
            {
                m_skyMode = SkyTypeMode.Expanse;
            }
#endif
            UpdateLightData();
            if (!SystemActive)
            {
                m_isNight = false;
                m_lastLightState = false;
                SetLightStatus(false, false);
            }
            else
            {
                SetLightStatus(false, false);
            }
        }
        /// <summary>
        /// Sets the light status
        /// </summary>
        /// <param name="light"></param>
        public void SetLightStatus(bool getSettings = false, bool useLightStateCheck = false)
        {
            if (m_genaManagerExists)
            {
                if (getSettings)
                {
                    GeNaGlobalReferences.GeNaManagerInstance.GetTimeOfDayLightSyncSettings(out m_systemActive, out PreviewSyncLightCullingInEditor, out m_lightShadowMode, out m_lightCullingMode, out m_lightCullingDistance, out m_cullingWaitForFrames);
                }
            }
            if (!m_lastLightState && !m_isNight)
            {
                return;
            }
            if (Application.isPlaying)
            {
                if (m_lastLightState != m_isNight && useLightStateCheck)
                {
                    UpdateCulling(m_player);
                }
                else if (!useLightStateCheck)
                {
                    UpdateCulling(m_player);
                }
            }
            else
            {
                if (m_lastLightState != m_isNight && useLightStateCheck)
                {
                    UpdateCulling(m_sceneCamera);
                }
                else if (!useLightStateCheck)
                {
                    UpdateCulling(m_sceneCamera);
                }
            }
        }
        /// <summary>
        /// Update the light settings
        /// </summary>
        /// <param name="getSettings"></param>
        public void UpdateLightSettings(bool getSettings = false)
        {
            if (m_genaManagerExists)
            {
                if (getSettings)
                {
                    GeNaGlobalReferences.GeNaManagerInstance.GetTimeOfDayLightSyncSettings(out m_systemActive, out PreviewSyncLightCullingInEditor, out m_lightShadowMode, out m_lightCullingMode, out m_lightCullingDistance, out m_cullingWaitForFrames);
                }
            }
            if (Application.isPlaying)
            {
                SetCullingMode(true, LightCullingMode, m_player);
            }
            else
            {
                SetCullingMode(true, LightCullingMode, m_sceneCamera);
            }
        }
        /// <summary>
        /// Gets the settings from the GeNa manager
        /// </summary>
        /// <returns></returns>
        public void GetLightRenderSettings()
        {
            if (m_genaManagerExists)
            {
                GeNaGlobalReferences.GeNaManagerInstance.GetTimeOfDayLightSyncSettings(out m_systemActive, out PreviewSyncLightCullingInEditor, out m_lightShadowMode, out m_lightCullingMode, out m_lightCullingDistance, out m_cullingWaitForFrames);
            }
        }
        #endregion
        #region Private Functions
        /// <summary>
        /// Updates the is night bool in the system
        /// </summary>
        private void CheckNightStatus()
        {
            if (m_skyMode == SkyTypeMode.Gaia)
            {
#if GAIA_PRO_PRESENT
                if (m_gaiaProExists)
                {
                    m_isNight = ProceduralWorldsGlobalWeather.Instance.CheckIsNight();
                }
#endif
            }
            else
            {
#if EXPANSE_PRESENT
                m_isNight = ExpanseController.Instance.IsNight;
#else
                Debug.LogWarning("If you're using expanse please add the script define EXPANSE_PRESENT to use expanse features.");
#endif
            }
        }
        /// <summary>
        /// Checks if the light needs to be updated
        /// </summary>
        private void UpdateCulling(Transform player)
        {
            m_lastLightState = m_isNight;
            m_playerExists = player;
            if (!m_playerExists || !m_lightComponentExists)
            {
                return;
            }
            if (!SystemActive)
            {
                SetCullingMode(false, LightCullingMode, player);
                return;
            }
            else if (LightCullingMode == Constants.LightSyncCullingMode.None)
            {
                SetCullingMode(m_isNight, LightCullingMode, player);
                return;
            }
            else if (!m_gaiaProExists && m_skyMode == SkyTypeMode.Gaia)
            {
                m_isNight = false;
                SetCullingMode(false, LightCullingMode, player);
            }
            else if (m_skyMode == SkyTypeMode.Expanse)
            {
                SetCullingMode(m_isNight, LightCullingMode, player);
            }
        }
        /// <summary>
        /// Sets up the culling mode based on the active bool and culling mode
        /// </summary>
        /// <param name="light"></param>
        /// <param name="emissionObject"></param>
        /// <param name="active"></param>
        /// <param name="cullingMode"></param>
        private void SetCullingMode(bool active, Constants.LightSyncCullingMode cullingMode, Transform player)
        {
            switch (m_emissionRenderType)
            {
                case EmissionRenderType.GameObject:
                {
                    if (!m_lightComponentExists || !m_lightEmissionObjectExists)
                    {
                        return;
                    }
                    break;
                }
                case EmissionRenderType.Material:
                {
                    if (!m_lightComponentExists || !m_lightEmissionMaterialExists)
                    {
                        return;
                    }
                    break;
                }
                case EmissionRenderType.Both:
                {
                    if (!m_lightComponentExists || !m_lightEmissionMaterialExists || !m_lightEmissionObjectExists)
                    {
                        return;
                    }
                    break;
                }
                case EmissionRenderType.None:
                {
                    if (!m_lightComponentExists)
                    {
                        return;
                    }
                    break;
                }
            }
            if (active)
            {
                switch (cullingMode)
                {
                    case Constants.LightSyncCullingMode.ShadowOnly:
                    {
                        if (Vector3.Distance(m_lightComponent.transform.position, player.position) <= LightCullingDistance)
                        {
                            m_lightComponent.shadows = m_lightShadowMode;
                        }
                        else
                        {
                            m_lightComponent.shadows = LightShadows.None;
                        }
                        if (!m_overrideSystemActiveState)
                        {
                            if (!m_isNight)
                            {
                                m_lightComponent.enabled = false;
#if GeNa_HDRP
                                m_lightData.enabled = false;
#endif
                            }
                            else
                            {
                                float distance = Vector3.Distance(m_lightComponent.transform.position, player.position);
                                m_lightComponent.enabled = distance <= LightEmissionCullingDistance;
#if GeNa_HDRP
                                m_lightData.enabled = distance <= LightEmissionCullingDistance;
#endif
                            }
                            if (m_emissionRenderType != EmissionRenderType.None)
                            {
                                switch (m_emissionRenderType)
                                {
                                    case EmissionRenderType.GameObject:
                                        m_lightEmissionObject.SetActive(m_isNight);
                                        break;
                                    case EmissionRenderType.Material:
                                        if (m_isNight)
                                        {
                                            m_emissionMaterial.EnableKeyword(m_enableEmissioKeyWord);
                                        }
                                        else
                                        {
                                            m_emissionMaterial.DisableKeyword(m_enableEmissioKeyWord);
                                        }
                                        break;
                                    case EmissionRenderType.Both:
                                        m_lightEmissionObject.SetActive(m_isNight);
                                        if (m_isNight)
                                        {
                                            m_emissionMaterial.EnableKeyword(m_enableEmissioKeyWord);
                                        }
                                        else
                                        {
                                            m_emissionMaterial.DisableKeyword(m_enableEmissioKeyWord);
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    }
                    case Constants.LightSyncCullingMode.LightAndShadow:
                    {
                        if (Vector3.Distance(m_lightComponent.transform.position, player.position) <= LightCullingDistance)
                        {
                            m_lightComponent.shadows = m_lightShadowMode;
                        }
                        else
                        {
                            m_lightComponent.shadows = LightShadows.None;
                        }
                        if (!m_overrideSystemActiveState)
                        {
                            if (!m_isNight)
                            {
                                m_lightComponent.enabled = false;
#if GeNa_HDRP
                                m_lightData.enabled = false;
#endif
                            }
                            else
                            {
                                float distance = Vector3.Distance(m_lightComponent.transform.position, player.position);
                                m_lightComponent.enabled = distance <= LightEmissionCullingDistance;
#if GeNa_HDRP
                                m_lightData.enabled = distance <= LightEmissionCullingDistance;
#endif
                            }
                            if (m_emissionRenderType != EmissionRenderType.None)
                            {
                                switch (m_emissionRenderType)
                                {
                                    case EmissionRenderType.GameObject:
                                        m_lightEmissionObject.SetActive(m_isNight);
                                        break;
                                    case EmissionRenderType.Material:
                                        if (m_isNight)
                                        {
                                            m_emissionMaterial.EnableKeyword(m_enableEmissioKeyWord);
                                        }
                                        else
                                        {
                                            m_emissionMaterial.DisableKeyword(m_enableEmissioKeyWord);
                                        }
                                        break;
                                    case EmissionRenderType.Both:
                                        m_lightEmissionObject.SetActive(m_isNight);
                                        if (m_isNight)
                                        {
                                            m_emissionMaterial.EnableKeyword(m_enableEmissioKeyWord);
                                        }
                                        else
                                        {
                                            m_emissionMaterial.DisableKeyword(m_enableEmissioKeyWord);
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    }
                    case Constants.LightSyncCullingMode.None:
                    {
                        if (Vector3.Distance(m_lightComponent.transform.position, player.position) <= LightCullingDistance)
                        {
                            m_lightComponent.shadows = m_lightShadowMode;
                        }
                        else
                        {
                            m_lightComponent.shadows = LightShadows.None;
                        }
                        if (!m_overrideSystemActiveState)
                        {
                            if (!m_isNight)
                            {
                                m_lightComponent.enabled = false;
#if GeNa_HDRP
                                m_lightData.enabled = false;
#endif
                            }
                            else
                            {
                                float distance = Vector3.Distance(m_lightComponent.transform.position, player.position);
                                m_lightComponent.enabled = distance <= LightEmissionCullingDistance;
#if GeNa_HDRP
                                m_lightData.enabled = distance <= LightEmissionCullingDistance;
#endif
                            }
                            if (m_emissionRenderType != EmissionRenderType.None)
                            {
                                switch (m_emissionRenderType)
                                {
                                    case EmissionRenderType.GameObject:
                                        m_lightEmissionObject.SetActive(m_isNight);
                                        break;
                                    case EmissionRenderType.Material:
                                        if (m_isNight)
                                        {
                                            m_emissionMaterial.EnableKeyword(m_enableEmissioKeyWord);
                                        }
                                        else
                                        {
                                            m_emissionMaterial.DisableKeyword(m_enableEmissioKeyWord);
                                        }
                                        break;
                                    case EmissionRenderType.Both:
                                        m_lightEmissionObject.SetActive(m_isNight);
                                        if (m_isNight)
                                        {
                                            m_emissionMaterial.EnableKeyword(m_enableEmissioKeyWord);
                                        }
                                        else
                                        {
                                            m_emissionMaterial.DisableKeyword(m_enableEmissioKeyWord);
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                switch (cullingMode)
                {
                    case Constants.LightSyncCullingMode.ShadowOnly:
                    {
                        m_lightComponent.shadows = LightShadows.None;
                        if (!m_overrideSystemActiveState)
                        {
                            m_lightComponent.enabled = false;
#if GeNa_HDRP
                            m_lightData.enabled = false;
#endif
                            if (m_emissionRenderType != EmissionRenderType.None)
                            {
                                switch (m_emissionRenderType)
                                {
                                    case EmissionRenderType.GameObject:
                                        m_lightEmissionObject.SetActive(m_isNight);
                                        break;
                                    case EmissionRenderType.Material:
                                        if (m_isNight)
                                        {
                                            m_emissionMaterial.EnableKeyword(m_enableEmissioKeyWord);
                                        }
                                        else
                                        {
                                            m_emissionMaterial.DisableKeyword(m_enableEmissioKeyWord);
                                        }
                                        break;
                                    case EmissionRenderType.Both:
                                        m_lightEmissionObject.SetActive(m_isNight);
                                        if (m_isNight)
                                        {
                                            m_emissionMaterial.EnableKeyword(m_enableEmissioKeyWord);
                                        }
                                        else
                                        {
                                            m_emissionMaterial.DisableKeyword(m_enableEmissioKeyWord);
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    }
                    case Constants.LightSyncCullingMode.LightAndShadow:
                    {
                        m_lightComponent.shadows = LightShadows.None;
                        if (!m_overrideSystemActiveState)
                        {
                            m_lightComponent.enabled = false;
#if GeNa_HDRP
                            m_lightData.enabled = false;
#endif
                            if (m_emissionRenderType != EmissionRenderType.None)
                            {
                                switch (m_emissionRenderType)
                                {
                                    case EmissionRenderType.GameObject:
                                        m_lightEmissionObject.SetActive(false);
                                        break;
                                    case EmissionRenderType.Material:
                                        m_emissionMaterial.DisableKeyword(m_enableEmissioKeyWord);
                                        break;
                                    case EmissionRenderType.Both:
                                        m_lightEmissionObject.SetActive(false);
                                        m_emissionMaterial.DisableKeyword(m_enableEmissioKeyWord);
                                        break;
                                }
                            }
                        }
                        break;
                    }
                    case Constants.LightSyncCullingMode.None:
                    {
                        m_lightComponent.shadows = m_lightShadowMode;
                        if (!m_overrideSystemActiveState)
                        {
                            m_lightComponent.enabled = false;
#if GeNa_HDRP
                            m_lightData.enabled = false;
#endif
                            if (m_emissionRenderType != EmissionRenderType.None)
                            {
                                switch (m_emissionRenderType)
                                {
                                    case EmissionRenderType.GameObject:
                                        m_lightEmissionObject.SetActive(false);
                                        break;
                                    case EmissionRenderType.Material:
                                        if (m_isNight)
                                        {
                                            m_emissionMaterial.EnableKeyword(m_enableEmissioKeyWord);
                                        }
                                        else
                                        {
                                            m_emissionMaterial.DisableKeyword(m_enableEmissioKeyWord);
                                        }
                                        break;
                                    case EmissionRenderType.Both:
                                        m_lightEmissionObject.SetActive(false);
                                        if (m_isNight)
                                        {
                                            m_emissionMaterial.EnableKeyword(m_enableEmissioKeyWord);
                                        }
                                        else
                                        {
                                            m_emissionMaterial.DisableKeyword(m_enableEmissioKeyWord);
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Updates the light settings to the manager settings
        /// </summary>
        private void UpdateLightData()
        {
            if (m_genaManagerExists)
            {
                GeNaGlobalReferences.GeNaManagerInstance.GetTimeOfDayLightSyncSettings(out m_systemActive, out PreviewSyncLightCullingInEditor, out m_lightShadowMode, out m_lightCullingMode, out m_lightCullingDistance, out m_cullingWaitForFrames);
            }
            if (m_lightComponent != null)
            {
                m_lightComponent.shadows = m_lightShadowMode;
                if (!m_overrideSystemActiveState)
                {
                    m_lightComponent.enabled = m_isNight;
                    if (m_emissionRenderType != EmissionRenderType.None)
                    {
                        if (m_lightEmissionObjectExists)
                        {
                            if (m_emissionRenderType != EmissionRenderType.None)
                            {
                                switch (m_emissionRenderType)
                                {
                                    case EmissionRenderType.GameObject:
                                        m_lightEmissionObject.SetActive(m_isNight);
                                        break;
                                    case EmissionRenderType.Material:
                                        m_emissionMaterial.DisableKeyword(m_enableEmissioKeyWord);
                                        break;
                                    case EmissionRenderType.Both:
                                        m_lightEmissionObject.SetActive(m_isNight);
                                        m_emissionMaterial.DisableKeyword(m_enableEmissioKeyWord);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets the player transform
        /// </summary>
        /// <returns></returns>
        private Transform GetPlayer()
        {
            GameObject playerObject = GameObject.Find(Constants.playerThirdPersonName);
            if (playerObject != null)
            {
                return playerObject.transform;
            }
            playerObject = GameObject.Find(Constants.playerFirstPersonName);
            if (playerObject != null)
            {
                return playerObject.transform;
            }
            playerObject = GameObject.Find(Constants.playerFlyCamName);
            if (playerObject != null)
            {
                return playerObject.transform;
            }
            playerObject = GameObject.Find("Player");
            if (playerObject != null)
            {
                return playerObject.transform;
            }
            playerObject = GameObject.Find(Constants.playerXRName);
            if (playerObject != null)
            {
                return playerObject.transform;
            }
            Camera camera = FindObjectOfType<Camera>();
            if (camera != null)
            {
                return camera.gameObject.transform;
            }
            return null;
        }
        /// <summary>
        /// Gets the editor scene view camera to use as the player in scene view
        /// </summary>
        /// <returns></returns>
        private Transform GetEditorPlayer()
        {
#if UNITY_EDITOR
            Camera camera = SceneView.lastActiveSceneView.camera;
            if (camera != null)
            {
                return camera.transform;
            }
#endif
            return null;
        }
        /// <summary>
        /// Removes warning when gaia is not installed
        /// </summary>
        private void RemoveWarning()
        {
            if (m_isNight)
            {
                m_isNight = true;
            }
            if (m_gaiaProExists)
            {
                m_gaiaProExists = true;
            }
        }
        #endregion
        #region Public Static Functions
        /// <summary>
        /// Sets up the light sync with the transform
        /// Player transform required, can be either player or camera transform
        /// Recommended to use the player camera
        /// </summary>
        /// <param name="player"></param>
        public static bool SetupLightSync(Transform player)
        {
            GaiaTimeOfDayLightSync[] allLightSyncs = GetAllInstances();
            if (allLightSyncs.Length > 0)
            {
                foreach (GaiaTimeOfDayLightSync lightSync in allLightSyncs)
                {
                    lightSync.ValidateComponents(player);
                }
            }
            return player != null;
        }
        /// <summary>
        /// Gets all the components in the scene
        /// </summary>
        /// <returns></returns>
        public static GaiaTimeOfDayLightSync[] GetAllInstances()
        {
            return FindObjectsOfType<GaiaTimeOfDayLightSync>();
        }
        /// <summary>
        /// Loads editor prefs
        /// </summary>
        public static void LoadPreferences(GeNaManager manager)
        {
#if UNITY_EDITOR
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
#endif
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
#if UNITY_EDITOR
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
#endif
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
#if UNITY_EDITOR
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
#endif
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
#if UNITY_EDITOR
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
#endif
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
#if UNITY_EDITOR
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
#endif
        }
        #endregion
    }
}