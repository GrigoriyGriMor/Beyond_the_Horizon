using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class StartThankyouMessage
{
    static StartThankyouMessage()
    {
        if (System.IO.File.Exists(Application.dataPath + "/Julhiecio TPS Controller/Editor/DontShowThankYouMessage.jutps") == false)
        {
            ThankYouWindow.ShowWindow();
            System.IO.File.Create(Application.dataPath + "/Julhiecio TPS Controller/Editor/DontShowThankYouMessage.jutps");
        }
    }
}

public class ThankYouWindow : EditorWindow
{
    [MenuItem("JU TPS/Thank you!")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ThankYouWindow));
        GetWindow(typeof(ThankYouWindow)).titleContent.text = "Thanks!";
        const int width = 275;
        const int height = 511;

        var x = (Screen.currentResolution.width - width) / 2;
        var y = (Screen.currentResolution.height - height) / 2;

        GetWindow<ThankYouWindow>().position = new Rect(x, y, width, height);
    }

    [MenuItem("JU TPS/Help/Open Documentation")]
    public static void OpenDocumentation()
    {
        Application.OpenURL(Application.dataPath + "/Julhiecio TPS Controller/Documentation JU TPS.pdf");
    }
    [MenuItem("JU TPS/Help/Open Tutorials Playlist")]
    public static void OpenTutorialPlaylists()
    {
        Application.OpenURL("https://youtube.com/playlist?list=PLznOHnSwmVcGcbDpXtElYKFVFYE9DvCgz");
    }
    [MenuItem("JU TPS/Help/Support Email")]
    public static void OpenSupportEmail()
    {
        Application.OpenURL("mailto:julhieciogames1@gmail.com");
    }

    public Texture2D Banner;
    private void OnGUI()
    {
        if (Banner == null)
        {
            Banner = JUTPS.CustomEditorUtilities.GetImage("Assets/Julhiecio TPS Controller/Textures/Editor/JU TPS BANNER.png");
        }
        if (Banner != null)
        {
            JUTPS.CustomEditorUtilities.RenderImageWithResize(Banner, new Vector2(265, 70));
        }

        var style = new GUIStyle(EditorStyles.label);
        style.font = JUTPS.CustomEditorStyles.JUTPSEditorFont();
        style.fontSize = 16;
        style.wordWrap = true;

        GUILayout.Label("Thanks!", JUTPS.CustomEditorStyles.Header());
        GUILayout.Label("Thank you very much for the purchase, you don't know how much is helping me!" +
            "\r\n\n I hope you enjoy my work, I am always updating and bringing new things and improvements." +
            "\r\n\n  if you have any suggestions or need help with something you can send me an email:", style);


        if (GUILayout.Button(" ✎ Open Support Email", JUTPS.CustomEditorStyles.MiniToolbar(), GUILayout.Width(180)))
        {
            OpenSupportEmail();
        }

        GUILayout.Space(15);

        GUILayout.Label("How to Start ?", JUTPS.CustomEditorStyles.Header());
        if (GUILayout.Button(" ▷ Tutorials Playlist", JUTPS.CustomEditorStyles.MiniToolbar(), GUILayout.Width(180)))
        {
            OpenTutorialPlaylists();
        }
        if (GUILayout.Button(" ▓ Open Documentation", JUTPS.CustomEditorStyles.MiniToolbar(), GUILayout.Width(180)))
        {
            OpenDocumentation();
        }

        GUILayout.Space(15);

        GUILayout.Label("My Youtube Channel:", JUTPS.CustomEditorStyles.Header());
        if (GUILayout.Button(" Julhiecio GameDev", JUTPS.CustomEditorStyles.MiniToolbar()))
        {
            Application.OpenURL("https://www.youtube.com/c/JulhiecioGameDev");
        }
    }
}
