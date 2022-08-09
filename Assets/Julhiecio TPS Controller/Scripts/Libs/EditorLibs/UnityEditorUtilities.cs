using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
namespace JUTPS
{
    public static class CustomEditorStyles
    {
        public static GUIStyle WarningStyle()
        {
            GUIStyle WarningStyle = new GUIStyle(EditorStyles.helpBox);
            WarningStyle.normal.textColor = Color.yellow;
            WarningStyle.fontSize = 12;
            return WarningStyle;
        }
        public static GUIStyle NormalStateStyle()
        {
            GUIStyle normalStateStyle = new GUIStyle(EditorStyles.objectField);
            //normalStateStyle.normal.textColor = new Color(0.3f, 0.7f, 0.3f, 1F);
            normalStateStyle.fontSize = 12;
            return normalStateStyle;
        }
        public static GUIStyle EnabledStyle()
        {
            GUIStyle enabledStyle = new GUIStyle(EditorStyles.objectField);
            enabledStyle.normal.textColor = new Color(0.3f, 0.7f, 0.3f, 1F);
            enabledStyle.fontSize = 12;
            return enabledStyle;
        }
        public static GUIStyle DisabledStyle()
        {
            GUIStyle disabledStyle = new GUIStyle(EditorStyles.objectField);
            disabledStyle.normal.textColor = new Color(1f, 0.3f, 0.3f, 1F);
            disabledStyle.fontSize = 12;
            return disabledStyle;
        }
        public static GUIStyle MiniButtonStyle()
        {
            GUIStyle minibuttonstyle = new GUIStyle(EditorStyles.miniButtonMid);
            minibuttonstyle.fontSize = 12;
            minibuttonstyle.alignment = TextAnchor.MiddleCenter;
            return minibuttonstyle;
        }
        public static GUIStyle MiniLeftButtonStyle()
        {
            GUIStyle minibuttonleftstyle = new GUIStyle(EditorStyles.miniButtonLeft);
            minibuttonleftstyle.fontSize = 12;
            minibuttonleftstyle.alignment = TextAnchor.MiddleLeft;
            return minibuttonleftstyle;
        }
        public static GUIStyle DangerButtonStyle()
        {
            GUIStyle DangerButton = new GUIStyle(EditorStyles.miniButtonMid);
            DangerButton.normal.textColor = new Color(1, 0.6f, 0.6f, 1F);
            DangerButton.fontSize = 12;
            DangerButton.fontStyle = FontStyle.Bold;
            return DangerButton;
        }
        public static GUIStyle StateStyle()
        {
            GUIStyle DangerButton = new GUIStyle(EditorStyles.miniButtonMid);
            DangerButton.font = JUTPSEditorFont();
            DangerButton.fontSize = 11;
            DangerButton.fontStyle = FontStyle.Normal;
            return DangerButton;
        }
        public static GUIStyle Toolbar()
        {
            GUIStyle toolbar = new GUIStyle(EditorStyles.objectField);
            //HeaderStyle.normal.textColor = new Color(1, 1, 1, 1F);
            toolbar.font = JUTPSEditorFont();
            toolbar.fontSize = 16;
            toolbar.fontStyle = FontStyle.Normal;
            return toolbar;
        }
        public static GUIStyle MiniToolbar()
        {
            GUIStyle toolbar = new GUIStyle(EditorStyles.objectField);
            //HeaderStyle.normal.textColor = new Color(1, 1, 1, 1F);
            toolbar.font = JUTPSEditorFont();
            toolbar.fontSize = 14;
      
            toolbar.fontStyle = FontStyle.Normal;
            return toolbar;
        }
        public static GUIStyle Title()
        {
            GUIStyle titleStyle = new GUIStyle(EditorStyles.label);
            //titleStyle.normal.textColor = new Color(1, 1, 1, 1F);
            titleStyle.font = JUTPSEditorFont();
            titleStyle.fontSize = 18;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleLeft;
            return titleStyle;
        }
        public static GUIStyle Header()
        {
            GUIStyle header = new GUIStyle(EditorStyles.objectFieldThumb);
            //titleStyle.normal.textColor = new Color(1, 1, 1, 1F);
            header.font = JUTPSEditorFont();
            header.fontSize = 18;
            return header;
        }
        public static GUIStyle ErrorStyle()
        {
            GUIStyle ErrorStyle = new GUIStyle(EditorStyles.helpBox);
            ErrorStyle.normal.textColor = new Color(1, 0.4f, 0.4f, 1F);
            ErrorStyle.fontSize = 12;
            ErrorStyle.wordWrap = true;
            return ErrorStyle;
        }
        public static Font JUTPSEditorFont()
        {
            Font font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Julhiecio TPS Controller/Editor/EditorResources/Fonts/Hyperjump Bold.otf");
            return font;
        }
    }
    public static class CustomEditorUtilities
    {
        private static Texture2D logo;
        public static void JUTPSTitle(string TitleName)
        {
            if (logo == null)
            {
                logo = GetImage("Assets/Julhiecio TPS Controller/Textures/Editor/JUTPSLOGO.png");
                return;
            }
            else
            {
                GUILayout.BeginHorizontal();
                RenderImage(logo);
                GUILayout.Label(TitleName, JUTPS.CustomEditorStyles.Title());
                GUILayout.EndHorizontal();
            }
        }
        public static Texture2D GetImage(string filepath)
        {
            //filepath = FileUtil.GetProjectRelativePath(filepath);
            Texture2D img = AssetDatabase.LoadAssetAtPath<Texture2D>(filepath);
            //Debug.Log(img);
            //Debug.Log(filepath);
            return img;
        }
        public static void RenderImage(Texture2D image)
        {
            GUILayout.Label(image, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
        }
        public static void RenderImageWithResize(Texture2D image, Vector2 Size)
        {
            GUILayout.Label(image, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        }
    }
    public static class LayerMaskUtilities
    {
        public static LayerMask GroundMask()
        {
            LayerMask groundmask = LayerMask.GetMask("Default", "Terrain", "Walls", "VehicleMeshCollider");
            return groundmask;
        }
        public static LayerMask CrosshairMask()
        {
            LayerMask crosshair = LayerMask.GetMask("Default", "Terrain", "Walls", "VehicleMeshCollider");
            return crosshair;
        }
        public static LayerMask CameraCollisionMask()
        {
            LayerMask crosshair = LayerMask.GetMask("Default", "Terrain", "Walls", "VehicleMeshCollider");
            return crosshair;
        }
        public static LayerMask DrivingCameraCollisionMask()
        {
            LayerMask crosshair = LayerMask.GetMask("Default", "Terrain", "Walls");
            return crosshair;
        }
    }
}
#endif
