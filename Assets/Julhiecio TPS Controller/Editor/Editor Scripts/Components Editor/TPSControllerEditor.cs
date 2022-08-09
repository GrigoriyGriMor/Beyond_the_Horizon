using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ThirdPersonController))]
public class TPSControllerEditor : Editor
{
    public bool MovementSettings, GroundCheckSettings, StepCorrectionSettings, FootstepSettings, AttackSettings, EventsSettings, WeaponSettings, States;
    public bool AutoFindWeapons;
    private void OnEnable()
    {
        ThirdPersonController pl = (ThirdPersonController)target;

        PLlayerMasksStartup(pl);
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ThirdPersonController pl = (ThirdPersonController)target;

        JUTPS.CustomEditorUtilities.JUTPSTitle("Third Person Controller");

        MovementSettings = GUILayout.Toggle(MovementSettings, "✥ Movement", JUTPS.CustomEditorStyles.Toolbar());
        MovementSettingsVariables(pl);

        GroundCheckSettings = GUILayout.Toggle(GroundCheckSettings, "░  Ground Check", JUTPS.CustomEditorStyles.Toolbar());
        GroundCheckSettingsVariables(pl);

        StepCorrectionSettings = GUILayout.Toggle(StepCorrectionSettings, "↨   Step", JUTPS.CustomEditorStyles.Toolbar());
        StepCorrectionSettingsVariables(pl);

        FootstepSettings = GUILayout.Toggle(FootstepSettings, "♫  Footstep", JUTPS.CustomEditorStyles.Toolbar());
        FootstepSettingsVariables(pl);

        AttackSettings = GUILayout.Toggle(AttackSettings, "☼ Attack", JUTPS.CustomEditorStyles.Toolbar());
        AttackSettingsVariables(pl);

        WeaponSettings = GUILayout.Toggle(WeaponSettings, "◘  Weapons", JUTPS.CustomEditorStyles.Toolbar());
        WeaponSettingsVariables(pl);

        EventsSettings = GUILayout.Toggle(EventsSettings, "▼ Events", JUTPS.CustomEditorStyles.Toolbar());
        EventsSettingsVariables(pl);

        States = GUILayout.Toggle(States, "☻ States", JUTPS.CustomEditorStyles.Toolbar());
        StatesViewVariables(pl);
        serializedObject.ApplyModifiedProperties();
        //DrawDefaultInspector();
    }
    public void ExempleSettingsVariables(ThirdPersonController pl)
    {
        if (MovementSettings)
        {
            PLSettingsView(pl);
        }
    }
    public void MovementSettingsVariables(ThirdPersonController pl)
    {
        if (MovementSettings)
        {
            //MoveOnForwardWhenIsntAiming
            serializedObject.FindProperty("Speed").floatValue = EditorGUILayout.Slider("  Movement Speed", pl.Speed, 0, 8);
            serializedObject.FindProperty("RotationSpeed").floatValue = EditorGUILayout.Slider("  Rotation Speed", pl.RotationSpeed, 0, 30);
            serializedObject.FindProperty("JumpForce").floatValue = EditorGUILayout.Slider("  Jump Force", pl.JumpForce, 1, 10);
            serializedObject.FindProperty("AirInfluenceControll").floatValue = EditorGUILayout.Slider("  In Air Control Force", pl.AirInfluenceControll, 0, 100);

            serializedObject.FindProperty("CurvedMovement").boolValue = EditorGUILayout.ToggleLeft("  Curved Movement", pl.CurvedMovement, JUTPS.CustomEditorStyles.MiniLeftButtonStyle());
            serializedObject.FindProperty("BodyInclination").boolValue = EditorGUILayout.ToggleLeft("  Body Inclination", pl.BodyInclination, JUTPS.CustomEditorStyles.MiniLeftButtonStyle());

            serializedObject.FindProperty("RootMotion").boolValue = EditorGUILayout.ToggleLeft("  Root Motion", pl.RootMotion, JUTPS.CustomEditorStyles.MiniLeftButtonStyle());
            if (pl.RootMotion)
            {
                serializedObject.FindProperty("RootMotionSpeed").floatValue = EditorGUILayout.Slider("  Root Motion Speed", pl.RootMotionSpeed, 0, 10);
                serializedObject.FindProperty("RootMotionRotation").boolValue = EditorGUILayout.Toggle("  Root Motion Rotation", pl.RootMotionRotation);
            }


            PLSettingsView(pl);
        }
    }
    public void GroundCheckSettingsVariables(ThirdPersonController pl)
    {
        if (GroundCheckSettings == true)
        {
            if (pl.WhatIsGround == 0)
                pl.WhatIsGround = JUTPS.LayerMaskUtilities.GroundMask();


            LayerMask tempMask = EditorGUILayout.MaskField("  Ground Layer", UnityEditorInternal.InternalEditorUtility.LayerMaskToConcatenatedLayersMask(pl.WhatIsGround), UnityEditorInternal.InternalEditorUtility.layers);
            serializedObject.FindProperty("WhatIsGround").intValue = UnityEditorInternal.InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);

            serializedObject.FindProperty("GroundCheckRadious").floatValue = EditorGUILayout.Slider("  Radious", pl.GroundCheckRadious, 0.01f, 0.2f);
            serializedObject.FindProperty("GroundCheckSize").floatValue = EditorGUILayout.Slider("  Height", pl.GroundCheckSize, 0.05f, 0.5f);
            serializedObject.FindProperty("GroundCheckHeighOfsset").floatValue = EditorGUILayout.Slider("  Up Ofsset", pl.GroundCheckHeighOfsset, -1f, 1f);
            PLSettingsView(pl);
        }
    }
    public void StepCorrectionSettingsVariables(ThirdPersonController pl)
    {
        if (StepCorrectionSettings)
        {
            serializedObject.FindProperty("EnableStepCorrection").boolValue = EditorGUILayout.Toggle("  Step Correction", pl.EnableStepCorrection);

            LayerMask tempMask = EditorGUILayout.MaskField("  Step Correction Layers", UnityEditorInternal.InternalEditorUtility.LayerMaskToConcatenatedLayersMask(pl.WhatIsGround), UnityEditorInternal.InternalEditorUtility.layers);
            serializedObject.FindProperty("StepCorrectionMask").intValue = UnityEditorInternal.InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);

            serializedObject.FindProperty("FootstepHeight").floatValue = EditorGUILayout.Slider("  Step Raycast Distance", pl.FootstepHeight, 0.1f, 1f);
            serializedObject.FindProperty("ForwardStepOffset").floatValue = EditorGUILayout.Slider("  Forward Offset", pl.ForwardStepOffset, 0f, 1f);
            serializedObject.FindProperty("StepHeight").floatValue = EditorGUILayout.Slider("  Step Height", pl.StepHeight, 0.01f, pl.FootstepHeight);

            PLSettingsView(pl);
        }
    }
    public void FootstepSettingsVariables(ThirdPersonController pl)
    {
        if (FootstepSettings)
        {            
            serializedObject.FindProperty("MaxFootstepTime").floatValue = EditorGUILayout.FloatField("Time to play audio", pl.MaxFootstepTime);

            var footstepaudio = serializedObject.FindProperty("FootstepAudioClips");
            EditorGUILayout.PropertyField(footstepaudio);
        }
    }
    public void AttackSettingsVariables(ThirdPersonController pl)
    {
        if (AttackSettings)
        {
            serializedObject.FindProperty("EnabledPunchAttacks").boolValue = EditorGUILayout.Toggle("  Enable Punch Attacks", pl.EnabledPunchAttacks);
            serializedObject.FindProperty("PunchCollisionParticle").objectReferenceValue = EditorGUILayout.ObjectField("  Punch Hit Particle", pl.PunchCollisionParticle, typeof(GameObject), false) as GameObject;
        }
    }
    public void WeaponSettingsVariables(ThirdPersonController pl)
    {
        if (WeaponSettings)
        {
        if (GUILayout.Button("☼Find and set Weapons", EditorStyles.miniButtonMid))
            {
                SetAllWeapons(pl);
            }
            
            var Weapons = serializedObject.FindProperty("Weapons");
            EditorGUILayout.PropertyField(Weapons);

            serializedObject.FindProperty("PivotWeapons").objectReferenceValue = EditorGUILayout.ObjectField("  Weapons Rotation Center", pl.PivotWeapons, typeof(GameObject), true) as GameObject;

            LayerMask tempMask = EditorGUILayout.MaskField("  Crosshair Layer", UnityEditorInternal.InternalEditorUtility.LayerMaskToConcatenatedLayersMask(pl.CrosshairHitMask), UnityEditorInternal.InternalEditorUtility.layers);
            serializedObject.FindProperty("CrosshairHitMask").intValue = UnityEditorInternal.InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);

            if(pl.HumanoidSpine == null)
            {
                pl.HumanoidSpine = pl.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.UpperChest);
                serializedObject.FindProperty("HumanoidSpine").objectReferenceValue = EditorGUILayout.ObjectField("  Upper Chest Bone", pl.HumanoidSpine, typeof(GameObject), true) as GameObject;
            }
            PLSettingsView(pl);
        }
    }
    public void EventsSettingsVariables(ThirdPersonController pl)
    {
        if (EventsSettings)
        {
            serializedObject.FindProperty("RagdollWhenDie").boolValue = EditorGUILayout.ToggleLeft("Enable Ragdoll When Die", pl.RagdollWhenDie, JUTPS.CustomEditorStyles.MiniLeftButtonStyle());
            serializedObject.FindProperty("SlowmotionWhenDie").boolValue = EditorGUILayout.ToggleLeft("Enable Slow Motion When Die", pl.SlowmotionWhenDie, JUTPS.CustomEditorStyles.MiniLeftButtonStyle());
        }
    }
    public void StatesViewVariables(ThirdPersonController pl)
    {
        if (States)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Toggle(pl.IsDead, "Dead", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.Toggle(pl.IsMoving, "Moving", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.Toggle(pl.IsRunning, "Running", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            GUILayout.BeginHorizontal();

            GUILayout.Toggle(pl.IsRolling, "Rolling", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.Toggle(pl.IsGrounded, "Grounded", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.Toggle(pl.IsJumping, "Jumping", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.EndHorizontal();

            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
            GUILayout.Toggle(pl.IsAttacking, "Attacking", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.Toggle(pl.IsArmed, "Armed", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.Toggle(pl.IsDriving, "Driving", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.EndHorizontal();

            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
            GUILayout.Toggle(pl.ToEnterVehicle, "Can Drive Vehicle", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.Toggle(pl.ToPickupWeapon, "To Pick Up Weapon", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.EndHorizontal();

            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
            GUILayout.Toggle(pl.WallInFront, "Wall Ahead", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.Toggle(pl.InverseKinematics, "Inverse Kinematics", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.EndHorizontal();

            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
            GUILayout.Toggle(pl.Shot, "Shooting", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.Toggle(pl.CanShoot, "Can Shoot", JUTPS.CustomEditorStyles.StateStyle());
            GUILayout.EndHorizontal();

            //[HideInInspector] public bool Shot;
            //[HideInInspector] public bool CanShoot;
            //[HideInInspector] public bool WallInFront;
            //[HideInInspector] public bool InverseKinematics = true;




            GUILayout.BeginHorizontal();

            EditorGUILayout.Space(0.5f, false);

            var lifestyle = new GUIStyle(EditorStyles.objectField);
            lifestyle.normal.textColor = new Color(Mathf.Clamp((1 - pl.Health / 100), 0, 1), Mathf.Clamp((pl.Health / 100), 0, 1), 0, 0.5f);
            int health_int = (int)pl.Health;
            EditorGUILayout.LabelField("  ☻ Health: " + health_int.ToString() + "%", lifestyle, GUILayout.Width(120));

            serializedObject.FindProperty("Health").floatValue = GUILayout.HorizontalSlider(pl.Health, 0, 100);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(3);
        }
    }








    //Utility Functions
    public void SetAllWeapons(ThirdPersonController pl)
    {
        pl.Weapons = pl.GetComponentsInChildren<Weapon>();
        foreach (Weapon wp in pl.Weapons)
        {
            Debug.Log("Added the ''" +  wp.name + "'' to Weapon List");
        }
    }
    public void PLSettingsView(ThirdPersonController pl)
    {
        if (pl.TryGetComponent(out PlayerSettingsDrawer pldrw) == false)
        {
            EditorGUILayout.HelpBox("Attention: there is no Player Settings Viewer on your character", MessageType.Warning);
            if (GUILayout.Button("Add Component ''Player Settings Viewer'' ", EditorStyles.miniButtonMid))
            {
                pl.gameObject.AddComponent<PlayerSettingsDrawer>();
                pl.GetComponent<PlayerSettingsDrawer>().GroundCheck = true;
                pl.GetComponent<PlayerSettingsDrawer>().WeaponsPositions = true;
                pl.GetComponent<PlayerSettingsDrawer>().StepCorrection = true;
                Debug.Log("Added Player Settings View Component for the character");
            }
        }
    }
    public void PLlayerMasksStartup(ThirdPersonController pl)
    {
        if (pl.WhatIsGround == 0)
            pl.WhatIsGround = JUTPS.LayerMaskUtilities.GroundMask();
        if (pl.StepCorrectionMask == 0)
            pl.StepCorrectionMask = JUTPS.LayerMaskUtilities.GroundMask();
        if (pl.CrosshairHitMask == 0)
            pl.CrosshairHitMask = JUTPS.LayerMaskUtilities.CrosshairMask();
    }
}
