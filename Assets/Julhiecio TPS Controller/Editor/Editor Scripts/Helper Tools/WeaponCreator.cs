using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeaponCreator : EditorWindow
{
    public static GameObject WeaponGameObject;

    public string WeaponName;
    public int WeaponID;

    public float FireRate;

    public GameObject BulletPrefab;
    public GameObject MuzzleFlash;

    public int BulletsPerMagazine;
    public int TotalBulletsInWeapon;

    public Weapon.WeaponFireMode Weapon_Type;



    //Weapon Precision
    public QualityPrecision WeaponPrecision;
    public enum QualityPrecision { VeryBad, Bad, Good, VeryGood, Perfect }


    //Procedural Recoil
    public bool UseProceduralAnimations = true;
    public ProceduralRecoilAnimation ProceduralAnimation;
    public enum ProceduralRecoilAnimation {Pistol, Rifle, Shotgun }

    public AudioClip ShotSound;
    public AudioClip ReloadSound;

    public float ShotPositionForwad = 0.4f, ShotPositionRight, ShotPositionUpward = 0.055f;

    //Standard Rotation Euler Angles = Vector3(31.2799301,47.4751205,168.803284)
    public float IKLeftHandPositionForward = 0.1f, IKLeftHandPositionUpward, IKLeftHandPositionRight;
    public float IKLeftHandRotationX = 31.2799301f, IKLeftHandRotationY = 47.4751205f, IKLeftHandRotationZ = 168.803284f;


    Vector3 _mShotPosition;
    Vector3 _LeftHandIKPosition;
    Vector3 _LeftHandIKRotation;

    Vector2 scrollPosition;

    [MenuItem("JU TPS/Helper Tools/Weapon Creator")]
    public static void ShowWindows()
    {
        GetWindow(typeof(WeaponCreator));
    }
    public void CreateWeapon()
    {
        var startpos = WeaponGameObject.transform.position;
        WeaponGameObject.transform.position = Vector3.zero;

        //Create New Gameobjects
        GameObject shotpos = new GameObject(WeaponName + " Shot Position");
        GameObject lefthandik = new GameObject(WeaponName + " Left Hand IK Target");

        //Set all weapon settings
        var wp = WeaponGameObject;

        wp.AddComponent<Weapon>();
        wp.AddComponent<BoxCollider>();
        wp.GetComponent<BoxCollider>().isTrigger = true;
        wp.layer = 14;

        wp.GetComponent<Weapon>().WeaponName = WeaponName;
        wp.GetComponent<Weapon>().WeaponSwitchID = WeaponID;
        wp.GetComponent<Weapon>().Fire_Rate = FireRate;
        wp.GetComponent<Weapon>().BulletPrefab = BulletPrefab;
        wp.GetComponent<Weapon>().MuzzleFlashParticlePrefab = MuzzleFlash;
        wp.GetComponent<Weapon>().TotalBullets = TotalBulletsInWeapon;
        wp.GetComponent<Weapon>().BulletsPerMagazine = BulletsPerMagazine;
        wp.GetComponent<Weapon>().FireMode = Weapon_Type;
        wp.GetComponent<Weapon>().GenerateProceduralAnimation = UseProceduralAnimations;
        wp.GetComponent<Weapon>().Unlocked = true;

        wp.GetComponent<Weapon>().ShootAudio = ShotSound;
        wp.GetComponent<Weapon>().ReloadAudio = ReloadSound;

        //Set new gameobjests positions 
        shotpos.transform.position = wp.GetComponent<WeaponCreatorSettingsVisualizer>().shotposition;
       
        shotpos.transform.SetParent(wp.transform);
        lefthandik.transform.SetParent(wp.transform);

        var pos = _LeftHandIKPosition + wp.transform.forward * -0.06f + wp.transform.up * 0.03f + wp.transform.right * -0.08f;
        lefthandik.transform.position = wp.transform.position + pos;
        lefthandik.transform.rotation = wp.GetComponent<WeaponCreatorSettingsVisualizer>().handrotation;


        wp.GetComponent<Weapon>().Shoot_Position = shotpos.transform;
        wp.GetComponent<Weapon>().IK_Position_LeftHand = lefthandik.transform;

        //Precision Settings
        switch (WeaponPrecision)
        {
            case QualityPrecision.VeryBad:
                wp.GetComponent<Weapon>().Precision = 10;
                wp.GetComponent<Weapon>().LossOfAccuracyPerShot = 0.25f;
                break;
            case QualityPrecision.Bad:
                wp.GetComponent<Weapon>().Precision = 15;
                wp.GetComponent<Weapon>().LossOfAccuracyPerShot = 0.25f;
                break;
            case QualityPrecision.Good:
                wp.GetComponent<Weapon>().Precision = 15;
                wp.GetComponent<Weapon>().LossOfAccuracyPerShot = 0.16f;
                break;
            case QualityPrecision.VeryGood:
                wp.GetComponent<Weapon>().Precision = 15;
                wp.GetComponent<Weapon>().LossOfAccuracyPerShot = 0.1f;
                break;
            case QualityPrecision.Perfect:
                wp.GetComponent<Weapon>().Precision = 1;
                wp.GetComponent<Weapon>().LossOfAccuracyPerShot = 0;
                break;
        }
        //Procedural Animations
        switch (ProceduralAnimation)
        {
            case ProceduralRecoilAnimation.Pistol:
                wp.GetComponent<Weapon>().RecoilForce = 0.043f;
                wp.GetComponent<Weapon>().RecoilForceRotation = 11.9f;
                break;
            case ProceduralRecoilAnimation.Rifle:
                wp.GetComponent<Weapon>().RecoilForce = 0.054f;
                wp.GetComponent<Weapon>().RecoilForceRotation = 6.3f;
                break;
            case ProceduralRecoilAnimation.Shotgun:
                wp.GetComponent<Weapon>().RecoilForce = 0.1f;
                wp.GetComponent<Weapon>().RecoilForceRotation = 20f;
                break;
        }

        //Close Window
        Debug.Log("Weapon created");
        WeaponGameObject.transform.position = startpos;
    }

    void OnDisable()
    {
        //Delete Visualization Script
        if (WeaponGameObject != null)
            DestroyImmediate(WeaponGameObject.GetComponent<WeaponCreatorSettingsVisualizer>());
            WeaponGameObject = null;
    }
    private void OnInspectorUpdate()
    {
        _LeftHandIKRotation = new Vector3(IKLeftHandRotationX, IKLeftHandRotationY, IKLeftHandRotationZ);
        if (WeaponGameObject != null)
        {
            _mShotPosition = new Vector3(ShotPositionRight, ShotPositionUpward, ShotPositionForwad);
            _LeftHandIKPosition = new Vector3(IKLeftHandPositionRight, IKLeftHandPositionUpward, IKLeftHandPositionForward);
            if (WeaponGameObject.TryGetComponent(out WeaponCreatorSettingsVisualizer Visualizer))
            {
                Visualizer.ShotPosition = _mShotPosition;
                Visualizer.IKLeftHandPosition = _LeftHandIKPosition;
                Visualizer.IKLeftHandRotation = _LeftHandIKRotation;
            }
            else
            {
                WeaponGameObject.AddComponent<WeaponCreatorSettingsVisualizer>();
            }
        }
    }
    private void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height - 20));

        GUIStyle HeaderStyle = JUTPS.CustomEditorStyles.Toolbar();

        GUIStyle ErrorStyle = JUTPS.CustomEditorStyles.ErrorStyle();

        GUIStyle WarningStyle = JUTPS.CustomEditorStyles.WarningStyle();


        //GUILayout.Label("| Weapon Settings", HeaderStyle);
        JUTPS.CustomEditorUtilities.JUTPSTitle("Weapon Creator");
        WeaponGameObject = EditorGUILayout.ObjectField("Weapon GameObject", WeaponGameObject, typeof(GameObject), true) as GameObject;

        if (WeaponGameObject != null)
        {
            if (WeaponGameObject.TryGetComponent(out Weapon WeaponSelected) == false)
            {

                if (WeaponGameObject.TryGetComponent(out WeaponCreatorSettingsVisualizer Visualizer))
                {
                    Visualizer.ShotPosition = _mShotPosition;
                    Visualizer.IKLeftHandPosition = _LeftHandIKPosition;
                    Visualizer.IKLeftHandRotation = _LeftHandIKRotation;
                }
                else
                {
                    WeaponGameObject.AddComponent<WeaponCreatorSettingsVisualizer>();
                }
                if(WeaponGameObject.transform.parent != null)
                {
                    GUILayout.Label("Warning: The Weapon GameObject has a Parent, which can make the generation look strange.\n To make sure, move it from Parent;", WarningStyle);
                }
                if(WeaponGameObject.transform.rotation != Quaternion.identity)
                {
                    GUILayout.Label("Warning: The rotation is not (0,0,0), reset the rotation to make sure it will work.", WarningStyle);
                }
                Weapon_Type = (Weapon.WeaponFireMode)EditorGUILayout.EnumPopup("Weapon Type", Weapon_Type);
                WeaponName = EditorGUILayout.TextField("Weapon Name", WeaponName);
                WeaponID = EditorGUILayout.IntField("Weapon ID", WeaponID);
                FireRate = EditorGUILayout.FloatField("Fire Rate", FireRate);
                WeaponPrecision = (QualityPrecision)EditorGUILayout.EnumPopup("Precision", WeaponPrecision);
                UseProceduralAnimations = EditorGUILayout.Toggle("Use Procedural Animation", UseProceduralAnimations);

                GUILayout.Label("• Ammunition", HeaderStyle);
                BulletPrefab = EditorGUILayout.ObjectField("Bullet Prefab", BulletPrefab, typeof(GameObject), false) as GameObject;
                MuzzleFlash = EditorGUILayout.ObjectField("Muzzle Flash", MuzzleFlash, typeof(GameObject), false) as GameObject;
                BulletsPerMagazine = EditorGUILayout.IntField("Bullets per Magazine", BulletsPerMagazine);
                TotalBulletsInWeapon = EditorGUILayout.IntField("Total Bullets", TotalBulletsInWeapon);

                GUILayout.Label("• Positions", HeaderStyle);
                GUILayout.Label("• IK Left Hand Position", EditorStyles.helpBox);
                IKLeftHandPositionRight = EditorGUILayout.Slider("X", IKLeftHandPositionRight, -2, 2);
                IKLeftHandPositionUpward = EditorGUILayout.Slider("Y", IKLeftHandPositionUpward, -2, 2);
                IKLeftHandPositionForward = EditorGUILayout.Slider("Z", IKLeftHandPositionForward, -2, 2);

                GUILayout.Label("• IK Left Hand Rotation", EditorStyles.helpBox);
                IKLeftHandRotationX = EditorGUILayout.Slider("X", IKLeftHandRotationX, -360, 360);
                IKLeftHandRotationY = EditorGUILayout.Slider("Y", IKLeftHandRotationY, -360, 360);
                IKLeftHandRotationZ = EditorGUILayout.Slider("Z", IKLeftHandRotationZ, -360, 360);

                GUILayout.Label("• Shot Position", EditorStyles.helpBox);
                ShotPositionRight = EditorGUILayout.Slider("X", ShotPositionRight, -2, 2);
                ShotPositionUpward = EditorGUILayout.Slider("Y", ShotPositionUpward, -2, 2);
                ShotPositionForwad = EditorGUILayout.Slider("Z", ShotPositionForwad, -2, 2);

                GUILayout.Label("♫ Weapon Sounds", HeaderStyle);
                ShotSound = EditorGUILayout.ObjectField("Shot Sound", ShotSound, typeof(AudioClip), false) as AudioClip;
                ReloadSound = EditorGUILayout.ObjectField("Reload Sound", ReloadSound, typeof(AudioClip), false) as AudioClip;

                if (UseProceduralAnimations)
                {
                    GUILayout.Label("♦ Procedural Animations", HeaderStyle);
                    ProceduralAnimation = (ProceduralRecoilAnimation)EditorGUILayout.EnumPopup("Procedural Animation", ProceduralAnimation);
                }
                GUILayout.Label("◙ Finishing", HeaderStyle);
                if (GUILayout.Button("Create Weapon"))
                {
                    CreateWeapon();
                }
            }
            else
            {

                GUIStyle DangerButton = new GUIStyle(EditorStyles.miniButtonMid);
                DangerButton.normal.textColor = new Color(1, 0.6f, 0.6f, 1F);
                DangerButton.fontSize = 14;
                DangerButton.fontStyle = FontStyle.Bold;

                GUILayout.Label("Error: There is already a ''Weapon'' component in the Weapon GameObject. \n\nClick on ''Clear Weapon'' to clear all weapon data.", ErrorStyle);
                if (GUILayout.Button("Clear Weapon Data", DangerButton, GUILayout.Width(150)))
                {
                    DestroyImmediate(WeaponGameObject.GetComponent<Weapon>().IK_Position_LeftHand.gameObject);
                    DestroyImmediate(WeaponGameObject.GetComponent<Weapon>().Shoot_Position.gameObject);
                    DestroyImmediate(WeaponGameObject.GetComponent<Weapon>());
                    DestroyImmediate(WeaponGameObject.GetComponent<AudioClip>());
                    DestroyImmediate(WeaponGameObject.GetComponent<Collider>());
                }
            }
        }
        else
        { 
            GUILayout.Label("Warning: WeaponGameobject is empty, select a weapon model in the scene.", WarningStyle);
        }
        GUILayout.EndScrollView();
    }
}
