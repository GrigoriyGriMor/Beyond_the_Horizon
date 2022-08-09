using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
[AddComponentMenu("JU TPS/Tools/Player Settings Viewer")]
public class PlayerSettingsDrawer : MonoBehaviour
{
    public bool DestroyOnStartGame;
    public ThirdPersonController PlayerController;
    public GizmosSettings VisualizationSettings;
    public bool StepCorrection, GroundCheck, WeaponsPositions;
    void Start()
    {
        if (DestroyOnStartGame)
            Destroy(this);
    }
    private void LoadVisualizationAssets()
    {
        PlayerController = GetComponent<ThirdPersonController>();

        VisualizationSettings.ResourcesPath = Application.dataPath + "/Julhiecio TPS Controller/Editor/Editor Resources/GizmosModels/";
        var path_to_load = VisualizationSettings.ResourcesPath;
        path_to_load = FileUtil.GetProjectRelativePath(path_to_load);

        VisualizationSettings.StepVisualizerWireMesh =
         AssetDatabase.LoadAssetAtPath<MeshFilter>(path_to_load + "Step Visualizer Model Wire.fbx").sharedMesh;

        VisualizationSettings.StepVisualizerMesh =
         AssetDatabase.LoadAssetAtPath<MeshFilter>(path_to_load + "Step Visualizer Model.fbx").sharedMesh;




        VisualizationSettings.HandVisualizerWireMesh =
         AssetDatabase.LoadAssetAtPath<MeshFilter>(path_to_load + "Hand Gun Visualizer Wire Model.fbx").sharedMesh;
        VisualizationSettings.HandVisualizerMesh =
         AssetDatabase.LoadAssetAtPath<MeshFilter>(path_to_load + "Hand Gun Visualizer Model.fbx").sharedMesh;
    }
    private void OnDrawGizmos()
    {
        if (VisualizationSettings.StepVisualizerMesh == null)
            LoadVisualizationAssets();

        //Step Visualizer
        float gizmoscale = 0.1f;

        if (GroundCheck)
        {
            //Ground Check Debug
            if (PlayerController.IsGrounded == false)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(transform.position + transform.up * PlayerController.GroundCheckHeighOfsset, new Vector3(PlayerController.GroundCheckRadious, PlayerController.GroundCheckSize, PlayerController.GroundCheckRadious) * 2);
            }
            else
            {
                Gizmos.color = Color.green;

                Gizmos.DrawWireCube(transform.position + transform.up * PlayerController.GroundCheckHeighOfsset, new Vector3(PlayerController.GroundCheckRadious, PlayerController.GroundCheckSize, PlayerController.GroundCheckRadious) * 2);
            }
        }

        //Step Correction Settings
        if (StepCorrection)
        {
            if (PlayerController.DirectionTransform != null)
            {
                var step_pos = PlayerController.transform.position + PlayerController.DirectionTransform.transform.forward * PlayerController.ForwardStepOffset + transform.up * PlayerController.StepHeight;
                var step_pos_height = PlayerController.transform.position + PlayerController.DirectionTransform.transform.forward * PlayerController.ForwardStepOffset + transform.up * PlayerController.FootstepHeight;
                if (PlayerController.Step_Hit.point != Vector3.zero
                && PlayerController.Step_Hit.point.y > PlayerController.transform.position.y + PlayerController.StepHeight)
                {
                    step_pos = PlayerController.Step_Hit.point;
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(PlayerController.Step_Hit.point, 0.05f);
                }
                Gizmos.color = VisualizationSettings.GizmosColor;
                Gizmos.DrawMesh(VisualizationSettings.StepVisualizerMesh, 0, step_pos, PlayerController.DirectionTransform.transform.rotation, new Vector3(gizmoscale, gizmoscale, gizmoscale));

                Gizmos.color = VisualizationSettings.WireGizmosColor;
                Gizmos.DrawWireMesh(VisualizationSettings.StepVisualizerWireMesh, 0, step_pos, PlayerController.DirectionTransform.transform.rotation, new Vector3(gizmoscale, gizmoscale, gizmoscale));

                Gizmos.color = VisualizationSettings.WireGizmosColor;
                Gizmos.DrawWireMesh(VisualizationSettings.StepVisualizerWireMesh, 0, step_pos_height, PlayerController.DirectionTransform.transform.rotation, new Vector3(gizmoscale, gizmoscale, gizmoscale));

                Gizmos.DrawLine(step_pos, step_pos_height);
            }
            else
            {
                var step_pos = PlayerController.transform.position + PlayerController.transform.forward * PlayerController.ForwardStepOffset + transform.up * PlayerController.StepHeight;
                var step_pos_height = PlayerController.transform.position + PlayerController.transform.forward * PlayerController.ForwardStepOffset + transform.up * PlayerController.FootstepHeight;
                if (PlayerController.Step_Hit.point != Vector3.zero
                && PlayerController.Step_Hit.point.y > PlayerController.transform.position.y + PlayerController.StepHeight)
                {
                    step_pos = PlayerController.Step_Hit.point;
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(PlayerController.Step_Hit.point, 0.05f);
                }
                Gizmos.color = VisualizationSettings.GizmosColor;
                Gizmos.DrawMesh(VisualizationSettings.StepVisualizerMesh, 0, step_pos, PlayerController.transform.rotation, new Vector3(gizmoscale, gizmoscale, gizmoscale));

                Gizmos.color = VisualizationSettings.WireGizmosColor;
                Gizmos.DrawWireMesh(VisualizationSettings.StepVisualizerWireMesh, 0, step_pos, PlayerController.transform.rotation, new Vector3(gizmoscale, gizmoscale, gizmoscale));

                Gizmos.color = VisualizationSettings.WireGizmosColor;
                Gizmos.DrawWireMesh(VisualizationSettings.StepVisualizerWireMesh, 0, step_pos_height, PlayerController.transform.rotation, new Vector3(gizmoscale, gizmoscale, gizmoscale));

                Gizmos.DrawLine(step_pos, step_pos_height);
            }
        }


        /*/Hand Visualizer
        if (WeaponsPositions && PlayerController.HandGunRightHandPosition != null)
        {
            //Pistol Type
            Gizmos.color = VisualizationSettings.GizmosColor;
            Gizmos.DrawMesh(VisualizationSettings.HandVisualizerMesh, 0, PlayerController.HandGunRightHandPosition.position, PlayerController.HandGunRightHandPosition.rotation, new Vector3(gizmoscale, gizmoscale, gizmoscale));
            Gizmos.color = VisualizationSettings.WireGizmosColor;
            Gizmos.DrawWireMesh(VisualizationSettings.HandVisualizerWireMesh, 0, PlayerController.HandGunRightHandPosition.position, PlayerController.HandGunRightHandPosition.rotation, new Vector3(gizmoscale, gizmoscale, gizmoscale));

            //Rifle Type
            Gizmos.color = VisualizationSettings.GizmosColor;
            Gizmos.DrawMesh(VisualizationSettings.HandVisualizerMesh, 0, PlayerController.RifleRightHandPosition.position, PlayerController.RifleRightHandPosition.rotation, new Vector3(gizmoscale, gizmoscale, gizmoscale));
            Gizmos.color = VisualizationSettings.WireGizmosColor;
            Gizmos.DrawWireMesh(VisualizationSettings.HandVisualizerWireMesh, 0, PlayerController.RifleRightHandPosition.position, PlayerController.RifleRightHandPosition.rotation, new Vector3(gizmoscale, gizmoscale, gizmoscale));
        }*/
    }
}
#endif
[System.Serializable]
public class GizmosSettings
{
    [HideInInspector]public string ResourcesPath;
    public Color GizmosColor = new Color(0, 0, 0, .5F);
    public Color WireGizmosColor = new Color(0.9F, 0.4F, 0.2F, .5F);

    public Mesh StepVisualizerMesh;
    public Mesh StepVisualizerWireMesh;

    public Mesh HandVisualizerMesh;
    public Mesh HandVisualizerWireMesh;
}