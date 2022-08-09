using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CamPivotController))]
public class TPSCameraControllerEditor : Editor
{
    public bool CameraSettings, CameraShake, CameraCollision, CameraPositionAdjustment, DrivingCamera, SlowMotion, FPSLimit;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CamPivotController c = (CamPivotController)target;

        JUTPS.CustomEditorUtilities.JUTPSTitle("Camera Controller");



        CameraSettings = GUILayout.Toggle(CameraSettings, "☇ Camera Follow", JUTPS.CustomEditorStyles.Toolbar());
        CameraSettingsVariables(c);

        CameraPositionAdjustment = GUILayout.Toggle(CameraPositionAdjustment, "✥ Camera Position", JUTPS.CustomEditorStyles.Toolbar());
        CameraPositionAdjustmentVariables(c);

        DrivingCamera = GUILayout.Toggle(DrivingCamera, "⦿ Driving Camera Position", JUTPS.CustomEditorStyles.Toolbar());
        CameraDrivingPositionAdjustmentVariables(c);

        CameraCollision = GUILayout.Toggle(CameraCollision, "⊕  Camera Collision", JUTPS.CustomEditorStyles.Toolbar());
        CameraCollisionVariables();

        CameraShake = GUILayout.Toggle(CameraShake, " ∿  Camera Shake", JUTPS.CustomEditorStyles.Toolbar());
        CameraShakeVariables(c);
        
        SlowMotion = GUILayout.Toggle(SlowMotion, "➠ Slowmotion", JUTPS.CustomEditorStyles.Toolbar());
        SlowmotionVariables(c);

        FPSLimit = GUILayout.Toggle(FPSLimit, "▦ FPS Limit", JUTPS.CustomEditorStyles.Toolbar());
        FPSLimitVariables(c);

        serializedObject.ApplyModifiedProperties();
    }
    public void CameraSettingsVariables(CamPivotController target)
    {
        if (CameraSettings)
        {
            serializedObject.FindProperty("PlayerTarget").objectReferenceValue =
                EditorGUILayout.ObjectField("  TPS Character Target", target.PlayerTarget, typeof(ThirdPersonController), true) as ThirdPersonController;

            serializedObject.FindProperty("MovementSpeed").floatValue =
                EditorGUILayout.Slider("  Movement Speed", target.MovementSpeed, 15,50);
            
            serializedObject.FindProperty("RotationSpeed").floatValue =
                EditorGUILayout.Slider("  Rotation Speed", target.RotationSpeed, 1, 30);

            serializedObject.FindProperty("Distance").floatValue =
                EditorGUILayout.Slider("  Normal Distance", target.Distance, 1, 8);

            serializedObject.FindProperty("IsArmedDistance").floatValue =
                EditorGUILayout.Slider("  Armed Distance", target.IsArmedDistance, 1, 8);

        }
    }
    public void CameraShakeVariables(CamPivotController target)
    {
        if (CameraShake)
        {
            serializedObject.FindProperty("CameraShakeWhenShooting").boolValue = EditorGUILayout.ToggleLeft("  Shake Camera When Player Shoot", target.CameraShakeWhenShooting, JUTPS.CustomEditorStyles.MiniLeftButtonStyle());

            serializedObject.FindProperty("CameraShakeSensibility").floatValue =
                EditorGUILayout.Slider("  Shaking Force", target.CameraShakeSensibility, 0, 2);
        }
    }
    public void CameraCollisionVariables()
    {
        if (CameraCollision)
        {
            var CamCollisionLayer = serializedObject.FindProperty("CameraCollisionLayer");
            EditorGUILayout.PropertyField(CamCollisionLayer);

            var DrivingCamCollisionLayer = serializedObject.FindProperty("CameraCollisionDrivingLayer");
            EditorGUILayout.PropertyField(DrivingCamCollisionLayer);
        }
    }

    public void CameraPositionAdjustmentVariables(CamPivotController target)
    {
        if (CameraPositionAdjustment)
        {
            serializedObject.FindProperty("TargetHeight").floatValue = EditorGUILayout.Slider("  Height", target.TargetHeight, -1,1);
            serializedObject.FindProperty("X_adjust").floatValue = EditorGUILayout.Slider("  X Position Adjustment", target.X_adjust, -1, 1);
            serializedObject.FindProperty("Y_adjust").floatValue = EditorGUILayout.Slider("  Y Position Adjustment", target.Y_adjust, -1, 1);

        }
    }
    public void CameraDrivingPositionAdjustmentVariables(CamPivotController target)
    {
        if (DrivingCamera)
        {
            serializedObject.FindProperty("DistanceCarDriving").floatValue = EditorGUILayout.Slider("  Car Distance", target.DistanceCarDriving, 1, 10);
            serializedObject.FindProperty("DistanceMotocycleDriving").floatValue = EditorGUILayout.Slider("  Motorcycle Distance", target.DistanceMotocycleDriving, 1, 10);

            serializedObject.FindProperty("TargetHeightDriving").floatValue = EditorGUILayout.Slider("  Height", target.TargetHeightDriving, -3, 3);

            serializedObject.FindProperty("X_adjustDriving").floatValue = EditorGUILayout.Slider("  X Position Adjustment", target.X_adjustDriving, -1, 1);
            serializedObject.FindProperty("Y_adjustDriving").floatValue = EditorGUILayout.Slider("  Y Position Adjustment", target.Y_adjustDriving, -1, 1);
            serializedObject.FindProperty("Z_adjustDriving").floatValue = EditorGUILayout.Slider("  Z Position Adjustment", target.Z_adjustDriving, -1, 1);
        }
    }

    public void SlowmotionVariables(CamPivotController target)
    {
        if (SlowMotion)
        {
            serializedObject.FindProperty("EnableSlowmotion").boolValue = EditorGUILayout.ToggleLeft("  Slowmotion", target.EnableSlowmotion, JUTPS.CustomEditorStyles.MiniLeftButtonStyle());
        }
    }
    public void FPSLimitVariables(CamPivotController target)
    {
        if (FPSLimit)
        {
            serializedObject.FindProperty("FPS_Limit").intValue = EditorGUILayout.IntSlider("  FPS Limit",target.FPS_Limit, 1,144);
        }
    }
}
