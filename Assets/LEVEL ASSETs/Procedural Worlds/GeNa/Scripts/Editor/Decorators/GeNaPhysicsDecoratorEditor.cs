﻿using UnityEditor;
using UnityEngine;
namespace GeNa.Core
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GeNaPhysicsDecorator))]
    public class GeNaPhysicsDecoratorEditor : GeNaDecoratorEditor<GeNaPhysicsDecorator>
    {
        [MenuItem("GameObject/GeNa/Decorators/Physics Decorator")]
        public static void AddDecorator(MenuCommand command) => GeNaDecoratorEditorUtility.CreateDecorator<GeNaPhysicsDecorator>(command);
        protected override void SettingsPanel(bool helpEnabled)
        {
            PhysicsSimulatorSettings settings = Decorator.Settings;
            EditorGUI.BeginChangeCheck();
            {
                settings.Iterations = EditorUtils.IntField("Iterations", settings.Iterations, helpEnabled);
                settings.StepSize = EditorUtils.Slider("Step Size", settings.StepSize, 0.01f, 0.1f, helpEnabled);
                settings.EmbedOffsetY = EditorUtils.Slider("Embed Offset Y", settings.EmbedOffsetY, -5f, 5f, helpEnabled);
                settings.MinHeightY = EditorUtils.FloatField("Min Height Y", settings.MinHeightY, helpEnabled);
            }
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object @object in targets)
                {
                    if (@object is GeNaPhysicsDecorator decorator)
                    {
                        PhysicsSimulatorSettings otherSettings = decorator.Settings;
                        otherSettings.Iterations = settings.Iterations;
                        otherSettings.StepSize = settings.StepSize;
                        otherSettings.EmbedOffsetY = settings.EmbedOffsetY;
                        otherSettings.MinHeightY = settings.MinHeightY;
                        EditorUtility.SetDirty(@object);
                    }
                }
            }
        }
    }
}