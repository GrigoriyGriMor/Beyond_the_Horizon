using System;
using UnityEditor;
using UnityEngine;
namespace GeNa.Core
{
    [InitializeOnLoad]
    public static class GeNaEditorEvents
    {
        #region Variables
        public static Action<string> onImportPackageCompleted;
        public static Action<string> onImportPackageCancelled;
        public static Action<string, string> onImportPackageFailed;
        public static Action onHeierarchyChanged;
        public static Action onEditorUpdate;
        public static Action onBeforeAssemblyReloads;
        public static Action onAfterAssemblyReloads;
        #endregion
        #region Constructors
        static GeNaEditorEvents()
        {
            // On Import Package Completed
            AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
            // On Import Package Cancelled
            AssetDatabase.importPackageCancelled -= OnImportPackageCancelled;
            AssetDatabase.importPackageCancelled += OnImportPackageCancelled;
            // On Import Package Failed
            AssetDatabase.importPackageFailed -= OnImportPackageFailed;
            AssetDatabase.importPackageFailed += OnImportPackageFailed;
            // On Before Assembly Reloads
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReloads;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReloads;
            // On After Assembly Reloads
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReloads;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReloads;
            // On Editor Update
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
            // On Hierarchy Changed
            EditorApplication.hierarchyChanged -= OnHeierarchyChanged;
            EditorApplication.hierarchyChanged += OnHeierarchyChanged;
            Initialize();
        }
        #endregion
        #region Methods
        /// <summary>
        /// Adds TerrainEvents Component on an Active Terrain 
        /// </summary>
        private static void AddTerrainEvents()
        {
            GeNaManager geNaManager = GeNaGlobalReferences.GeNaManagerInstance;
            if (geNaManager == null)
                return;
            Terrain[] terrains = Terrain.activeTerrains;
            if (terrains != null)
            {
                foreach (Terrain terrain in terrains)
                {
                    GameObject gameObject = terrain.gameObject;
                    GeNaTerrainEvents terrainEvents = gameObject.GetComponent<GeNaTerrainEvents>();
                    if (terrainEvents == null)
                        gameObject.AddComponent<GeNaTerrainEvents>();
                }
            }
        }
        // Sets up Default Events
        private static void Initialize()
        {
            // When the Hierarchy Changes, add the Terrain Events Script to an Active Terrain
            onHeierarchyChanged -= AddTerrainEvents;
            onHeierarchyChanged += AddTerrainEvents;
            // Call it once!
            AddTerrainEvents();
            // Setup GeNaEvents
            GeNaEditorUtility.SubscribeEvents();
        }
        private static void OnImportPackageCompleted(string packageName) => onImportPackageCompleted?.Invoke(packageName);
        /// <summary>
        /// Called when a package import is Cancelled.
        /// </summary>
        private static void OnImportPackageCancelled(string packageName) => onImportPackageCancelled?.Invoke(packageName);
        /// <summary>
        /// Called when a package import fails.
        /// </summary>
        private static void OnImportPackageFailed(string packageName, string error) => onImportPackageFailed?.Invoke(packageName, error);
        /// <summary>
        /// Called Before Assembly Reloads
        /// </summary>
        private static void OnBeforeAssemblyReloads()
        {
            onBeforeAssemblyReloads?.Invoke();
            GeNaFactory.Dispose();
        }
        /// <summary>
        /// Called After Assembly Reloads
        /// </summary> 
        private static void OnAfterAssemblyReloads() => onAfterAssemblyReloads?.Invoke();
        /// <summary>
        /// Called when Editor Updates
        /// </summary>
        private static void OnEditorUpdate() => onEditorUpdate?.Invoke();
        /// <summary>
        /// Event that is raised when an object or group of objects in the hierarchy changes.
        /// </summary>
        private static void OnHeierarchyChanged() => onHeierarchyChanged?.Invoke();
        #endregion
    }
}