using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(PixelQualityScale))]
public class PixelScaleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        PixelQualityScale p = (PixelQualityScale)target;

        JUTPS.CustomEditorUtilities.JUTPSTitle("Pixel Quality Scale");
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("   Scale", JUTPS.CustomEditorStyles.MiniToolbar());
        serializedObject.FindProperty("ResolutionQuality").floatValue = EditorGUILayout.Slider(p.ResolutionQuality, 1, 2);
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        EditorGUILayout.HelpBox("The higher the scale, the lower the resolution", MessageType.Info);


        serializedObject.ApplyModifiedProperties();
    }
}
