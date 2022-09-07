﻿using UnityEditor;
using UnityEngine;
namespace GeNa.Core
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GeNaSpawnFlagsDecorator))]
    public class GeNaSpawnFlagsDecoratorEditor : GeNaDecoratorEditor<GeNaSpawnFlagsDecorator>
    {
        [MenuItem("GameObject/GeNa/Decorators/Spawn Flags Decorator")]
        public static void AddDecorator(MenuCommand command) => GeNaDecoratorEditorUtility.CreateDecorator<GeNaSpawnFlagsDecorator>(command);
        protected override void SettingsPanel(bool helpEnabled)
        {
            EditorGUI.BeginChangeCheck();
            {
                EditorUtils.SpawnFlags(Decorator.SpawnFlags, true, helpEnabled);
            }
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object @object in targets)
                {
                    if (@object is GeNaSpawnFlagsDecorator decorator)
                        decorator.SpawnFlags.Copy(Decorator.SpawnFlags);

                    EditorUtility.SetDirty(@object);
                }
            }
        }
    }
}