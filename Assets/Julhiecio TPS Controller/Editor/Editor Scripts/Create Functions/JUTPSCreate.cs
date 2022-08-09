using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
public class JUTPSCreate
{
    static AudioClip clip = null;

    //>>>> CONTROLLERS
    [MenuItem("GameObject/JUTPS Create/Controllers.../Game Manager and UI", false, 0)]
    public static void CreateGameManager()
    {
        var gamemanagerprefab = AssetDatabase.LoadAssetAtPath<Object>("Assets/Julhiecio TPS Controller/Prefabs/Game/GameManager.prefab");
        PrefabUtility.InstantiatePrefab(gamemanagerprefab);
    }
    [MenuItem("GameObject/JUTPS Create/Controllers.../Third Person Controller", false, 0)]
    public static void CreateNewThirdPersonCharacter()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("No character selected. Please select a Humanoid Character and try again");
        }
        else
        {
            var pl = Selection.gameObjects[0];

            var anim = pl.GetComponent<Animator>();
            if (anim != null)
            {
                pl.tag = "Player";
                pl.layer = 9;

                var JUTPSAnimator = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Julhiecio TPS Controller/Animations/Animator/AnimatorTPS Controller.controller");
                anim.runtimeAnimatorController = JUTPSAnimator;
                anim.applyRootMotion = false;

                pl.AddComponent<ThirdPersonController>();
                pl.AddComponent<JUFootPlacement>();
                CreateNewWeaponRotationCenter();
                pl.GetComponent<ThirdPersonController>().PivotWeapons = pl.GetComponentInChildren<WeaponAimRotationCenter>().gameObject;

                for (int i = 0; i < 4; i++)
                {
                    pl.GetComponent<ThirdPersonController>().FootstepAudioClips.Add(new ThirdPersonController.FootstepAudios());
                    for (int ii = 0; ii < 4; ii++)
                    {
                        pl.GetComponent<ThirdPersonController>().FootstepAudioClips[i].AudioClips.Add(clip);
                    }
                }

                //Load Footstep Audios
                pl.GetComponent<ThirdPersonController>().FootstepAudioClips[0].SurfaceTag = "Untagged";
                for (int i = 0; i < 4; i++)
                {
                    pl.GetComponent<ThirdPersonController>().FootstepAudioClips[0].AudioClips[i] =
                        AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Julhiecio TPS Controller/Audio/Footstep/CC0 Sounds/Concrete/Footstep on Concrete 0" + (i+1) + ".ogg");
                }

                pl.GetComponent<ThirdPersonController>().FootstepAudioClips[1].SurfaceTag = "Stone";
                for (int i = 0; i < 4; i++)
                {
                    //Assets/Julhiecio TPS Controller/Audio/Footstep/CC0 Sounds/Stones/Footsteps-on-stone01.ogg
                    pl.GetComponent<ThirdPersonController>().FootstepAudioClips[1].AudioClips[i] =
                        AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Julhiecio TPS Controller/Audio/Footstep/CC0 Sounds/Stones/Footsteps-on-stone0" + (i+1) + ".ogg");
                }

                pl.GetComponent<ThirdPersonController>().FootstepAudioClips[2].SurfaceTag = "Grass";
                for (int i = 0; i < 4; i++)
                {
                    pl.GetComponent<ThirdPersonController>().FootstepAudioClips[2].AudioClips[i] =
                        AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Julhiecio TPS Controller/Audio/Footstep/CC0 Sounds/Grass/Footsteps-on-grass0" + (i+1) + ".ogg");
                }

                pl.GetComponent<ThirdPersonController>().FootstepAudioClips[3].SurfaceTag = "Tiles";
                for (int i = 0; i < 4; i++)
                {
                    pl.GetComponent<ThirdPersonController>().FootstepAudioClips[3].AudioClips[i] =
                        AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Julhiecio TPS Controller/Audio/Footstep/CC0 Sounds/Tiles/Footstep-on-tiles0" + (i+1) + ".ogg");
                }
                /*
                pl.GetComponent<ThirdPersonController>().FootstepAudioClips[1] =
                    AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Julhiecio TPS Controller/Audio/Footstep/Footstep on Concrete 02.ogg");
                pl.GetComponent<ThirdPersonController>().FootstepAudioClips[2] =
                    AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Julhiecio TPS Controller/Audio/Footstep/Footstep on Concrete 03.ogg");
                pl.GetComponent<ThirdPersonController>().FootstepAudioClips[3] =
                    AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Julhiecio TPS Controller/Audio/Footstep/Footstep on Concrete 04.ogg");
                */

                pl.GetComponent<Rigidbody>().mass = 85;
                pl.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
                pl.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

                pl.GetComponent<CapsuleCollider>().height = 2;
                pl.GetComponent<CapsuleCollider>().center = new Vector3(0, 1, 0);
                pl.GetComponent<CapsuleCollider>().radius = 0.3f;
            }
            else
            {
                Debug.LogWarning("The selected GameObject does not have an animator, are you sure it is a humanoid rig model?");
            }
        }
    }
    public void SetupAudios()
    {

    }
    [MenuItem("GameObject/JUTPS Create/Controllers.../Camera Controller", false, 0)]
    public static void CreateNewCamera()
    {
        var campivot = new GameObject("Camera Controller");
        campivot.AddComponent<CamPivotController>();
        campivot.transform.position = SceneViewInstantiatePosition();

        var camera = new GameObject("Main Camera");
        camera.AddComponent<Camera>();
        camera.AddComponent<AudioListener>();
        camera.transform.tag = "MainCamera";
        camera.transform.position = campivot.transform.position - campivot.transform.forward * 1f;
        camera.transform.parent = campivot.transform;

        CamPivotController cameracontroller = campivot.GetComponent<CamPivotController>();
        cameracontroller.CameraCollisionLayer = JUTPS.LayerMaskUtilities.CameraCollisionMask();
        cameracontroller.CameraCollisionDrivingLayer = JUTPS.LayerMaskUtilities.DrivingCameraCollisionMask();
    }

    [MenuItem("GameObject/JUTPS Create/Controllers.../Quick Scene Setup", false, 0)]
    public static void QuickSceneSetup()
    {
        CreateGameManager();
        CreateNewCamera();
    }



    //>>>> WEAPONS

    [MenuItem("GameObject/JUTPS Create/Weapons.../Weapon Aim Rotation Center", false, 0)]
    public static void CreateNewWeaponRotationCenter()
    {
        var WeaponPivot = new GameObject("Weapon Aim Rotation Center");
        WeaponPivot.AddComponent<WeaponAimRotationCenter>();
        if (Selection.activeGameObject != null)
        {
            WeaponPivot.transform.position = Selection.transforms[0].position + WeaponPivot.transform.up * 1.2f;
            WeaponPivot.transform.SetParent(Selection.transforms[0]);
        }
        else
        {
            WeaponPivot.transform.position = SceneViewInstantiatePosition();
        }
        var WeaponPositionsParent = new GameObject("Weapons Reference Positions");
        WeaponPositionsParent.transform.position = WeaponPivot.transform.position;
        WeaponPositionsParent.transform.parent = WeaponPivot.transform;

        //Weapon Position Reference
        var SmallWeaponPosition = new GameObject("Small Weapon Position Reference");
        SmallWeaponPosition.transform.parent = WeaponPositionsParent.transform;
        SmallWeaponPosition.transform.localPosition = new Vector3(0.212f, 0.065f, 0.361f);
        SmallWeaponPosition.transform.localRotation = Quaternion.Euler(0, 11.383f, -94.913f);
        WeaponPivot.GetComponent<WeaponAimRotationCenter>().CreateWeaponPositionReference(SmallWeaponPosition.name);
        WeaponPivot.GetComponent<WeaponAimRotationCenter>().WeaponPositionTransform[0] = SmallWeaponPosition.transform;
        WeaponPivot.GetComponent<WeaponAimRotationCenter>().StoreLocalTransform();

        var BigWeaponPosition = new GameObject("Big Weapon Position Reference");
        BigWeaponPosition.transform.parent = WeaponPositionsParent.transform;
        BigWeaponPosition.transform.localPosition = new Vector3(0.138f, -0.013f, 0.24f);
        BigWeaponPosition.transform.localRotation = Quaternion.Euler(0, 11.383f, -94.913f);
        WeaponPivot.GetComponent<WeaponAimRotationCenter>().CreateWeaponPositionReference(BigWeaponPosition.name);
        WeaponPivot.GetComponent<WeaponAimRotationCenter>().WeaponPositionTransform[1] = BigWeaponPosition.transform;
        WeaponPivot.GetComponent<WeaponAimRotationCenter>().StoreLocalTransform();
    }



    //>>>> UTILITIES
    [MenuItem("GameObject/JUTPS Create/Utilities/Trigger Message", false, 0)]
    public static void CreateTriggerMessage()
    {
        var triggermessage = new GameObject("Trigger Message");
        triggermessage.AddComponent<BoxCollider>();
        triggermessage.GetComponent<BoxCollider>().isTrigger = true;
        triggermessage.AddComponent<Rigidbody>();
        triggermessage.GetComponent<Rigidbody>().isKinematic = true;
        triggermessage.AddComponent<UIMenssengerTrigger>();

        triggermessage.gameObject.layer = 2;
        triggermessage.transform.position = SceneViewInstantiatePosition();
    }
    public static Vector3 SceneViewInstantiatePosition()
    {
        var view = SceneView.lastActiveSceneView.camera;
        if (view != null)
        {
            Vector3 pos = view.transform.position + view.transform.forward * 2;
            return pos;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
