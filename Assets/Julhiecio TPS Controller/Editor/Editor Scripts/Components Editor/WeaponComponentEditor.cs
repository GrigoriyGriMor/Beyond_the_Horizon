using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Weapon))]
[CanEditMultipleObjects]
public class WeaponComponentEditor : Editor
{
    //Settings Areas
    public bool WeaponSettings, Precision, Shooting, ProceduralAnimations, IKSettings, Audio;
    //Inspector Draw
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Weapon w = (Weapon)target;

        JUTPS.CustomEditorUtilities.JUTPSTitle("Weapon System");

        WeaponSettings = GUILayout.Toggle(WeaponSettings, "▼ Settings", JUTPS.CustomEditorStyles.Toolbar());
        WeaponSettingsVariables(w);

        Precision = GUILayout.Toggle(Precision, "∿ Precision", JUTPS.CustomEditorStyles.Toolbar());
        PrecisionVariables(w);

        Shooting = GUILayout.Toggle(Shooting, "☼ Shooting", JUTPS.CustomEditorStyles.Toolbar());
        ShootingVariables(w);

        ProceduralAnimations = GUILayout.Toggle(ProceduralAnimations, "☇ Procedural Animations", JUTPS.CustomEditorStyles.Toolbar());
        ProceduralAnimationVariables(w);

        IKSettings = GUILayout.Toggle(IKSettings, "↔ Left Hand IK", JUTPS.CustomEditorStyles.Toolbar());
        IKVariables(w);

        Audio = GUILayout.Toggle(Audio, "♫ Audios", JUTPS.CustomEditorStyles.Toolbar());
        AudioVariables(w);

        serializedObject.ApplyModifiedProperties();

        //GUILayout.Space(50);
        //DrawDefaultInspector();
    }

    //Inspector Drawers
    public void WeaponSettingsVariables(Weapon w)
    {
        if (WeaponSettings)
        {
            serializedObject.FindProperty("WeaponName").stringValue = EditorGUILayout.TextField("Weapon Name",w.WeaponName);
            serializedObject.FindProperty("WeaponSwitchID").intValue = EditorGUILayout.IntField("Weapon Switch ID", w.WeaponSwitchID);
            serializedObject.FindProperty("WeaponPositionID").intValue = EditorGUILayout.IntField("Weapon Position ID", w.WeaponPositionID);
            serializedObject.FindProperty("Unlocked").boolValue = EditorGUILayout.Toggle("Unlock Weapon", w.Unlocked);
        }
    }
    public void PrecisionVariables(Weapon w)
    {
        if(Precision == true)
        {
            serializedObject.FindProperty("Precision").floatValue = EditorGUILayout.FloatField("Accuracy", w.Precision);
            serializedObject.FindProperty("LossOfAccuracyPerShot").floatValue = EditorGUILayout.FloatField("Loss of Acurracy per Shot", w.LossOfAccuracyPerShot);
        }
    }
    public void ShootingVariables(Weapon w)
    {
        if(Shooting == true)
        {
            serializedObject.FindProperty("Shoot_Position").objectReferenceValue = EditorGUILayout.ObjectField("Shooting Position", w.Shoot_Position, typeof(Transform), true) as Transform;
            serializedObject.FindProperty("BulletPrefab").objectReferenceValue = EditorGUILayout.ObjectField("Bullet Prefab", w.BulletPrefab, typeof(GameObject), false);
            serializedObject.FindProperty("MuzzleFlashParticlePrefab").objectReferenceValue = EditorGUILayout.ObjectField("Muzzle Flash Prefab", w.MuzzleFlashParticlePrefab, typeof(GameObject), false);

            GUILayout.Space(10);
            var firemode = serializedObject.FindProperty("FireMode");
            EditorGUILayout.PropertyField(firemode);

            var aimmode = serializedObject.FindProperty("AimMode");
            EditorGUILayout.PropertyField(aimmode);

            if (w.AimMode == Weapon.WeaponAimMode.Scope)
            {
                var scopetexture = serializedObject.FindProperty("ScopeTexture");
                EditorGUILayout.PropertyField(scopetexture);

                var cameraposition = serializedObject.FindProperty("CameraAimingPosition");
                EditorGUILayout.PropertyField(cameraposition);

                serializedObject.FindProperty("CameraFOV").floatValue = EditorGUILayout.Slider("Camera FOV", w.CameraFOV,10,75);
            }
            if (w.AimMode == Weapon.WeaponAimMode.CameraApproach)
            {
                var cameraposition = serializedObject.FindProperty("CameraAimingPosition");
                EditorGUILayout.PropertyField(cameraposition);
                serializedObject.FindProperty("CameraFOV").floatValue = EditorGUILayout.Slider("Camera FOV", w.CameraFOV, 10, 75);
            }
            GUILayout.Space(10);

            serializedObject.FindProperty("BulletsAmounts").intValue = EditorGUILayout.IntField("Bullets in the Gun", w.BulletsAmounts);
            serializedObject.FindProperty("TotalBullets").intValue = EditorGUILayout.IntField("Total Amount of Bullets", w.TotalBullets);
            serializedObject.FindProperty("BulletsPerMagazine").intValue = EditorGUILayout.IntField("Bullets For Reload", w.BulletsPerMagazine);

            serializedObject.FindProperty("Fire_Rate").floatValue = EditorGUILayout.FloatField("Fire Rate", w.Fire_Rate);
        }
    }
    public void ProceduralAnimationVariables(Weapon w)
    {
        if (ProceduralAnimations)
        {
            serializedObject.FindProperty("GenerateProceduralAnimation").boolValue = EditorGUILayout.Toggle("Enable", w.GenerateProceduralAnimation);
            serializedObject.FindProperty("RecoilForce").floatValue = EditorGUILayout.Slider("Recoil Force", w.RecoilForce, 0f, 1f);
            serializedObject.FindProperty("RecoilForceRotation").floatValue = EditorGUILayout.Slider("Recoil Rotation Force", w.RecoilForceRotation, 0,60);
            
            GUILayout.Space(10);
            serializedObject.FindProperty("GunSlider").objectReferenceValue = EditorGUILayout.ObjectField("Gun Bolt/Slider", w.GunSlider, typeof(Transform), true) as Transform;
            serializedObject.FindProperty("SliderMovementOffset").floatValue = EditorGUILayout.Slider("Slider/Bolt Movement", w.SliderMovementOffset, 0f, 0.1f);

            GUILayout.Space(10);
            serializedObject.FindProperty("BulletCasingPrefab").objectReferenceValue = EditorGUILayout.ObjectField("Bullet Casing", w.BulletCasingPrefab, typeof(GameObject), false);
        }
    }
    public void IKVariables(Weapon w)
    {
        if (IKSettings)
        {
            serializedObject.FindProperty("IK_Position_LeftHand").objectReferenceValue = EditorGUILayout.ObjectField("Left Hand Position", w.IK_Position_LeftHand, typeof(Transform), true) as Transform;
        }
    }
    public void AudioVariables(Weapon w)
    {
        if(Audio == true)
        {
            var ShotAudio = serializedObject.FindProperty("ShootAudio");
            EditorGUILayout.PropertyField(ShotAudio);

            var ReloadAudio = serializedObject.FindProperty("ReloadAudio");
            EditorGUILayout.PropertyField(ReloadAudio);
        }
    }


    //Scene View Handles
    private void OnSceneGUI()
    {
        Weapon w = (Weapon)target;
        Vector3 RealCameraAimingPosition = w.transform.position + w.transform.right * w.CameraAimingPosition.x + w.transform.up * w.CameraAimingPosition.y + w.transform.forward * w.CameraAimingPosition.z;
        //w.CameraAimingPosition = Handles.PositionHandle(w.transform.position + w.CameraAimingPosition, w.transform.rotation);
        if(w.Shoot_Position != null)
        {
            w.Shoot_Position.position = Handles.PositionHandle(w.Shoot_Position.position, w.transform.rotation);
            Handles.Label(w.Shoot_Position.position,"Shooting Position");
            Handles.DrawWireDisc(w.Shoot_Position.position, w.Shoot_Position.forward, 0.04f);
        }
        else
        {
            //Create Shooting Position
            GameObject shootposition = new GameObject("Shooting Position");
            shootposition.transform.position = w.transform.position + w.transform.forward * 0.15f + w.transform.up * 0.1f;
            shootposition.transform.rotation = w.transform.rotation;
            w.Shoot_Position = shootposition.transform;
            shootposition.transform.SetParent(w.transform);
        }

        if(w.IK_Position_LeftHand != null)
        {
            w.IK_Position_LeftHand.rotation = Handles.RotationHandle(w.IK_Position_LeftHand.rotation, w.IK_Position_LeftHand.position);
            w.IK_Position_LeftHand.position = Handles.PositionHandle(w.IK_Position_LeftHand.position, w.transform.rotation);
        }
        else {
            //Create Left Hand IK Position
            GameObject lefthandikposition = new GameObject("Left Hand IK Position");
            lefthandikposition.transform.position = w.transform.position + w.transform.forward * 0.03f;
            lefthandikposition.transform.rotation = w.transform.rotation;
            w.IK_Position_LeftHand = lefthandikposition.transform;
            lefthandikposition.transform.SetParent(w.transform);
        }

        Handles.Label(w.transform.position + w.transform.up * 0.23f, "Name: " + w.WeaponName);
        Handles.Label(w.transform.position + w.transform.up * 0.2f, "Switch ID: " + w.WeaponSwitchID);
        Handles.Label(w.transform.position + w.transform.up * 0.18f, "Ammunition: " + w.BulletsAmounts +  " / " + w.TotalBullets + " Bullets");
    }
}
