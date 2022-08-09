using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaponAimRotationCenter))]
public class WeaponAimRotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        WeaponAimRotationCenter w = (WeaponAimRotationCenter)target;

        JUTPS.CustomEditorUtilities.JUTPSTitle("⊙ Weapon Aim Rotation Center");
        EditorGUILayout.Space(10);

        if (w.WeaponPositionsLengh == 0)
        {
            EditorGUILayout.HelpBox("You still have no weapon position, you will need one to adjust the position of a weapon type. For example: ''Pistol Weapon Position Reference''.", MessageType.Warning);
            EditorGUILayout.Space(10);
        }

        for (int i = 0; i < w.WeaponPositionsLengh; i++)
        {
            DrawWeaponPositionSettings(w, i);
        }

        if (GUILayout.Button("Add Weapon Position Reference", JUTPS.CustomEditorStyles.MiniButtonStyle(), GUILayout.Width(200)))
        {
            w.CreateWeaponPositionReference("New Weapon Position Reference");
        }
        serializedObject.ApplyModifiedProperties();
    }

    public void DrawWeaponPositionSettings(WeaponAimRotationCenter w, int index)
    {
        GUILayout.BeginHorizontal();
        //NAME
        serializedObject.FindProperty("WeaponPositionName").GetArrayElementAtIndex(index).stringValue = EditorGUILayout.TextField(w.WeaponPositionName[index], JUTPS.CustomEditorStyles.MiniToolbar());

        //ID
        string stringid = "↔ SWITCH ID:" + w.ID[index].ToString();
        EditorGUILayout.LabelField(stringid, JUTPS.CustomEditorStyles.MiniToolbar(), GUILayout.Width(130));
        //DELETE BUTTON
        if (GUILayout.Button("X", JUTPS.CustomEditorStyles.DangerButtonStyle(), GUILayout.Width(20)))
        {
            w.RemoveWeaponPositionReference(index);
        }
        GUILayout.EndHorizontal();

        //TRANSFORM REFERENCE
        serializedObject.FindProperty("WeaponPositionTransform").GetArrayElementAtIndex(index).objectReferenceValue = EditorGUILayout.ObjectField("  Transform Reference", w.WeaponPositionTransform[index], typeof(Transform), true) as Transform;


        //property = EditorGUILayout.ObjectField("  Transform Reference", w.WeaponPositionTransform[index], typeof(Transform), true) as Transform;


        //serializedObject.FindProperty("PunchCollisionParticle").objectReferenceValue = EditorGUILayout.ObjectField("  Punch Hit Particle", pl.PunchCollisionParticle, typeof(GameObject), false) as GameObject;

        EditorGUILayout.Space(5);
    }
}
