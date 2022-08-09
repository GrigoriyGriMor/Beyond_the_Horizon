using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeaponCreatorSettingsVisualizer : MonoBehaviour
{
    [HideInInspector] public Vector3 ShotPosition;
    [HideInInspector] public Vector3 IKLeftHandPosition;
    [HideInInspector] public Vector3 IKLeftHandRotation;
    [HideInInspector] public GizmosSettings GizmosVisualizationSettings;
    [HideInInspector] public Vector3 shotposition;
    [HideInInspector] public Vector3 handposition;
    [HideInInspector] public Vector3 handpositionvisual;
    [HideInInspector] public Quaternion handrotation;
    private void Awake()
    {
        Destroy(this);
    }
#if UNITY_EDITOR
    private void LoadVisualizationAssets()
    {
        GizmosVisualizationSettings.ResourcesPath = Application.dataPath + "/Julhiecio TPS Controller/Editor/EditorResources/GizmosModels/";
        var path_to_load = GizmosVisualizationSettings.ResourcesPath;
        path_to_load = FileUtil.GetProjectRelativePath(path_to_load);

        GizmosVisualizationSettings.HandVisualizerWireMesh =
         AssetDatabase.LoadAssetAtPath<MeshFilter>(path_to_load + "Hand Visualizer Wire Model.fbx").sharedMesh;
        GizmosVisualizationSettings.HandVisualizerMesh =
         AssetDatabase.LoadAssetAtPath<MeshFilter>(path_to_load + "Hand Visualizer Model.fbx").sharedMesh;
    }

    private void OnDrawGizmosSelected()
    {
        if (GizmosVisualizationSettings.HandVisualizerMesh == null)
        {
            LoadVisualizationAssets();
        }
        else
        {
            shotposition = transform.position + transform.right * ShotPosition.x + transform.up * ShotPosition.y + transform.forward * ShotPosition.z;
            Handles.color = new Color(0.9F, 0.6F, 0.3F, 1F);
            Handles.DrawWireDisc(shotposition, transform.forward, 0.03f);
            Handles.Label(shotposition, "Shot Position");
            Handles.ArrowHandleCap(0, shotposition, transform.rotation, 0.1f, EventType.Repaint);

            var scl = new Vector3(-0.1f, 0.1f, 0.1f);

            handposition = transform.position + transform.right * IKLeftHandPosition.x + transform.up * IKLeftHandPosition.y + transform.forward * IKLeftHandPosition.z;
            handrotation = transform.rotation * Quaternion.Euler(IKLeftHandRotation);

            Gizmos.color = new Color(0.9F, 0.6F, 0.3F, 1F);
            Gizmos.DrawWireMesh(GizmosVisualizationSettings.HandVisualizerWireMesh, 0, handposition, handrotation, scl);
            Gizmos.color = new Color(0, 0, 0, .3F);
            Gizmos.DrawMesh(GizmosVisualizationSettings.HandVisualizerMesh, 0, handposition, handrotation, scl);
        }
        HandleUtility.Repaint();
    }
#endif
}
