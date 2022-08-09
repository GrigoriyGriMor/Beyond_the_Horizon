using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AdvancedRagdollController))]
public class AdvancedRagdollEditor : Editor
{
    public Texture2D ImageBanner = null;
    public bool ViewSettings;
    public bool ViewStates;
    public bool EnableDebugging;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        AdvancedRagdollController rag = (AdvancedRagdollController)target;

        JUTPS.CustomEditorUtilities.JUTPSTitle("Advanced Ragdoll Controller");

        if (rag.RagdollBones == null)
        {
            GUILayout.Label("No Ragdoll Bones found, please create a Ragdoll", JUTPS.CustomEditorStyles.ErrorStyle());
            rag.StartAdvancedRagdollController();
        }
        else if (rag.RagdollBones.Length > 0)
        {
            //SETTINGS
            ViewSettings = GUILayout.Toggle(ViewSettings, "Ragdoll Transition Blending Settings", JUTPS.CustomEditorStyles.Toolbar());
            if (ViewSettings)
            {
                serializedObject.FindProperty("TimeToGetUp").floatValue = EditorGUILayout.Slider("  Time To Get Up", rag.TimeToGetUp, 1f, 4f);
                serializedObject.FindProperty("BlendSpeed").floatValue = EditorGUILayout.Slider("  Blend Speed", rag.BlendSpeed, 0f, 4f);
                serializedObject.FindProperty("RagdollDrag").floatValue = EditorGUILayout.Slider("  Ragdoll Bones Drag", rag.RagdollDrag, 0.001f, 4f);
            }

            //STATE
            ViewStates = GUILayout.Toggle(ViewStates, "Ragdoll States", JUTPS.CustomEditorStyles.Toolbar());
            if (ViewStates)
                GUILayout.Label(rag.State.ToString(), JUTPS.CustomEditorStyles.EnabledStyle());

            //DEBUG
            EnableDebugging = GUILayout.Toggle(EnableDebugging, "Debugging", JUTPS.CustomEditorStyles.Toolbar());
            if (EnableDebugging == true)
            {
                serializedObject.FindProperty("RagdollWhenPressKeyG").boolValue = EditorGUILayout.Toggle("  Ragdoll When Press G", rag.RagdollWhenPressKeyG);
                serializedObject.FindProperty("ViewBodyDirection").boolValue = EditorGUILayout.Toggle("  View Body Direction", rag.ViewBodyDirection);
                serializedObject.FindProperty("ViewBodyPhysics").boolValue = EditorGUILayout.Toggle("  View Body Phyics", rag.ViewBodyPhysics);
                serializedObject.FindProperty("ViewHumanBodyBones").boolValue = EditorGUILayout.Toggle("  View Body Bones", rag.ViewHumanBodyBones);
            }

            //DrawDefaultInspector();
        }
        serializedObject.ApplyModifiedProperties();
    }
}

