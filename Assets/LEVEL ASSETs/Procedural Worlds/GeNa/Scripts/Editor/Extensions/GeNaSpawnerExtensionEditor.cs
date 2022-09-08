﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace GeNa.Core
{
    [CustomEditor(typeof(GeNaSpawnerExtension))]
    public class GeNaSpawnerExtensionEditor : GeNaSplineExtensionEditor
    {
        private static Color SPAWN_TARGET_COLOR = new Color(0.8f, 0.8f, 0.0f, 0.5f);
        protected bool m_modifySpawner = false;
        protected GeNaSpawnerExtension m_spawnerExtension;
        private Vector2 m_lastMousePos = Vector2.zero;
        public AabbTest[,] m_fitnessArray = new AabbTest[1, 1];
        public override void OnSelected()
        {
            m_spawnerExtension = target as GeNaSpawnerExtension;
            m_spawnerExtension.Load();
            m_spawnerExtension.PreExecute();
            Validate();
        }
        protected void OnEnable()
        {
            if (m_editorUtils == null)
                m_editorUtils = PWApp.GetEditorUtils(this, "GeNaSplineExtensionEditor");
            m_spawnerExtension = target as GeNaSpawnerExtension;
        }
        private void Validate()
        {
            if (m_spawnerExtension == null)
                return;
            GeNaSpawner spawner = m_spawnerExtension.Spawner;
            if (spawner == null)
                return;
            spawner.Load();
            GeNaSpawnerData spawnerData = spawner.SpawnerData;
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                SpawnerEditor.ValidateSpawnerPrototypes(spawner, spawnerData, terrain, true);
            }
        }
        public override void OnSceneGUI()
        {
            Handles.color = new Color(0f, 1f, 0f, 0.25f);
            List<SpawnCall> spawnCalls = m_spawnerExtension.SpawnCalls;
            foreach (SpawnCall spawnCall in spawnCalls)
            {
                if (!spawnCall.IsActive)
                    continue;
                if (spawnCall.CanSpawn)
                    Handles.color = new Color(0f, 1f, 0f, 0.25f);
                else
                    Handles.color = new Color(1f, 0f, 0f, 0.25f);
                float radius = Mathf.Clamp(spawnCall.SpawnRange / 6f, 0.25f, 1f);
                Handles.DrawSolidArc(spawnCall.Location, Vector3.up, Vector3.forward, 360f, radius);
                // Handles.color = new Color(0f, 0f, 1f, 0.25f);
                radius = spawnCall.SpawnRange * .5f;
                Handles.DrawWireArc(spawnCall.Location, Vector3.up, Vector3.forward, 360f, radius);
            }
            if (m_modifySpawner)
            {
                GeNaSpawner spawner = m_spawnerExtension.Spawner;
                SpawnerEditor.GeNaSpawnerEditor(spawner);
            }
            Transform groundObject = m_spawnerExtension.GetTarget(out RaycastHit hitInfo);
            if (groundObject != null)
            {
                Vector3 point = hitInfo.point;
                Vector3 normal = hitInfo.normal;
                Vector3 up = Vector3.Cross(normal, Vector3.up);
                float angle = 360f;
                float radius = 2f;
                Handles.color = SPAWN_TARGET_COLOR;
                Handles.DrawSolidArc(point, normal, up, angle, radius);
            }
        }
        /// <summary>
        /// Update the array used for visualisation. Edit mode function that does the hard work for visualisation.
        /// </summary>
        public void UpdateSpawnerVisualisation()
        {
            GeNaSpawnerData spawner = m_spawnerExtension.SpawnerData;
            SpawnerSettings settings = spawner.Settings;
            Vector3 location = Vector3.zero;
            float halfSpawnRange = spawner.SpawnRange * .5f;
            Vector3 spawnOriginLocation = spawner.SpawnOriginLocation;
            // Calculate steps and update array size to handle different dimensions
            int dimensions = (int)spawner.SpawnRange + 1;
            if (dimensions > settings.MaxVisualizationDimensions)
                dimensions = settings.MaxVisualizationDimensions + 1;
            float stepIncrement = spawner.SpawnRange / ((float)dimensions - 1f);
            if (dimensions != m_fitnessArray.GetLength(0))
                m_fitnessArray = new AabbTest[dimensions, dimensions];
            // Build active proto list
            location.x = spawnOriginLocation.x - halfSpawnRange;
            for (int x = 0; x < dimensions; x++)
            {
                location.z = spawnOriginLocation.z - halfSpawnRange;
                for (int z = 0; z < dimensions; z++)
                {
                    AabbTest aabbTest = m_fitnessArray[x, z];
                    GeNaSpawnerInternal.GenerateAabbTest(spawner, out aabbTest, location);
                    m_fitnessArray[x, z] = aabbTest;
                    location.z += stepIncrement;
                }
                location.x += stepIncrement;
            }
            GeNaSpawnerInternal.ProcessAabbTests(spawner, m_fitnessArray, spawner.SpawnCriteria);
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
            GeNaSpawner oldSpawner = m_spawnerExtension.Spawner;
            m_spawnerExtension.Spawner = (GeNaSpawner)m_editorUtils.ObjectField("Spawner", m_spawnerExtension.Spawner, typeof(GeNaSpawner), true, HelpEnabled);
            if (oldSpawner != m_spawnerExtension.Spawner)
            {
                if (m_spawnerExtension.Spawner != null)
                {
                    var spawner = m_spawnerExtension.Spawner;
                    if (spawner != null)
                    {
                        var data = spawner.SpawnerData;
                        if (data != null)
                        {
                            m_spawnerExtension.SpawnRange = data.SpawnRange;
                            m_spawnerExtension.FlowRate = data.SpawnRange;
                        }
                    }
                    Validate();
                }
            }
            GeNaSpawnerData spawnerData = m_spawnerExtension.SpawnerData;
            if (spawnerData != null)
            {
                SpawnerEntry entry = m_spawnerExtension.SpawnerEntry;
                if (entry != null)
                {
                    entry.UseLargeRanges = m_editorUtils.Toggle("Use Large Ranges", entry.UseLargeRanges, HelpEnabled);
                    if (entry.UseLargeRanges)
                    {
                        // Display Spawner Data
                        entry.FlowRate = m_editorUtils.FloatField("Flow Rate", entry.FlowRate, HelpEnabled);
                        entry.SpawnRange = m_editorUtils.FloatField("Spawn Range", entry.SpawnRange, HelpEnabled);
                        entry.ThrowDistance = m_editorUtils.FloatField("Throw Distance", entry.ThrowDistance, HelpEnabled);
                        // Display Offset
                        entry.OffsetPosition = m_editorUtils.Vector3Field("Offset Position", entry.OffsetPosition, HelpEnabled);
                        entry.OffsetRotation = m_editorUtils.Vector3Field("Offset Rotation", entry.OffsetRotation, HelpEnabled);
                    }
                    else
                    {
                        // Display Spawner Data
                        entry.FlowRate = m_editorUtils.FloatField("Flow Rate", entry.FlowRate, HelpEnabled);
                        entry.SpawnRange = m_editorUtils.Slider("Spawn Range", entry.SpawnRange, 1f, 200f, HelpEnabled);
                        entry.ThrowDistance = m_editorUtils.Slider("Throw Distance", entry.ThrowDistance, 0f, entry.SpawnRange, HelpEnabled);
                        // Display Offset
                        Vector3 offsetPosition = entry.OffsetPosition;
                        Vector3 offsetRotation = entry.OffsetRotation;
                        offsetPosition.x = m_editorUtils.Slider("Offset Position X", offsetPosition.x, -20f, 20f, HelpEnabled);
                        offsetPosition.y = m_editorUtils.Slider("Offset Position Y", offsetPosition.y, -2f, 2f, HelpEnabled);
                        offsetRotation.y = m_editorUtils.Slider("Offset Rotation Y", offsetRotation.y, -180f, 180f, HelpEnabled);
                        entry.OffsetPosition = offsetPosition;
                        entry.OffsetRotation = offsetRotation;
                    }

                    entry.FlowRateAssigned = true;
                }
            }
            m_modifySpawner = m_editorUtils.Toggle("Modify Spawner", m_modifySpawner, HelpEnabled);
            m_spawnerExtension.AutoIterate = m_editorUtils.Toggle("Auto Iterate", m_spawnerExtension.AutoIterate, HelpEnabled);
            m_spawnerExtension.AlignToSpline = m_editorUtils.Toggle("Align to Spline", m_spawnerExtension.AlignToSpline, HelpEnabled);
            EditorGUI.indentLevel++;
            m_spawnerExtension.AlignChildrenToSpline = m_editorUtils.Toggle("Align Children", m_spawnerExtension.AlignChildrenToSpline, HelpEnabled);
            EditorGUI.indentLevel--;
            m_spawnerExtension.ConformToSlope = m_editorUtils.Toggle("Conform to Slope", m_spawnerExtension.ConformToSlope, HelpEnabled);
            EditorGUI.indentLevel++;
            m_spawnerExtension.ConformChildrenToSlope = m_editorUtils.Toggle("Conform Children", m_spawnerExtension.ConformChildrenToSlope, HelpEnabled);
            EditorGUI.indentLevel--;
            m_spawnerExtension.SnapToGround = m_editorUtils.Toggle("Snap to Ground", m_spawnerExtension.SnapToGround, HelpEnabled);
            EditorGUI.indentLevel++;
            m_spawnerExtension.SnapChildrenToGround = m_editorUtils.Toggle("Snap Children", m_spawnerExtension.SnapChildrenToGround, HelpEnabled);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Experimental", EditorStyles.boldLabel);
            m_spawnerExtension.CheckIntersectionBounds = m_editorUtils.Toggle("Check Intersection Bounds", m_spawnerExtension.CheckIntersectionBounds, HelpEnabled);
            float size = m_spawnerExtension.IntersectionBoundsSize.x;
            if (m_spawnerExtension.CheckIntersectionBounds)
            {
                EditorGUI.indentLevel++;
                size = m_editorUtils.FloatField("Intersection Bounds Size", size, HelpEnabled);
                EditorGUI.indentLevel--;
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_spawnerExtension.IntersectionBoundsSize = new Vector3(size, size, size);
                m_spawnerExtension.PreExecute();
            }

            GUI.enabled = spawnerData != null;
            bool isProcessing = m_spawnerExtension.IsProcessing;
            if (isProcessing)
            {
                GUI.enabled = true;
                GUIContent cancelContent = new GUIContent("\u00D7 Cancel");
                Color oldColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button(cancelContent, Styles.cancelBtn, GUILayout.MaxHeight(25f)))
                {
                    GeNaGlobalReferences.GeNaManagerInstance.Cancel = true;
                    //GUIUtility.ExitGUI();
                }
                GUI.backgroundColor = oldColor;
            }
            GUI.enabled = !isProcessing;
            EditorGUILayout.BeginHorizontal();
            {
                if (m_editorUtils.Button("Spawn"))
                    m_spawnerExtension.Spawn();
                if (m_editorUtils.Button("Iterate"))
                {
                    // EditorApplication.delayCall += Undo.PerformUndo;
                    m_spawnerExtension.Iterate();
                }
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();
            if (m_editorUtils.Button("Bake"))
            {
                if (EditorUtility.DisplayDialog(m_editorUtils.GetTextValue("BakeTitleSpawner"), m_editorUtils.GetTextValue("BakeMessageSpawner"), "Yes", "No"))
                {
                    m_spawnerExtension.Bake();
                }
            }
            m_editorUtils.InlineHelp("Spawn", HelpEnabled);
            m_editorUtils.InlineHelp("Iterate", HelpEnabled);
            GUI.enabled = true;
        }
    }
}