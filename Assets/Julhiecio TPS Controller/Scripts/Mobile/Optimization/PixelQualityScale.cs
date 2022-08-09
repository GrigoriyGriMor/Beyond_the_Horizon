using UnityEngine;
using UnityEditor;

[AddComponentMenu("JU TPS/Mobile/Optimization/Pixel Quality Scale")]
public class PixelQualityScale : MonoBehaviour
{

    private Resolution start_current_resolution;
    [Space]
    [Header("Is useful for increasing mobile performance")]
    [Header("This will reduce the resolution up to 2 times")]

    [Range(3,1)]
    public float ResolutionQuality;
    void Start()
    {
        //if has data of screen resolution
        if (PlayerPrefs.HasKey("screenresolutionWid") && PlayerPrefs.HasKey("screenresolutionHi"))
        {
            SetRenderResolutionQuality(PlayerPrefs.GetInt("screenresolutionWid"), PlayerPrefs.GetInt("screenresolutionHi"));
        }
        else
        {
            //save current resolution
            PlayerPrefs.SetInt("screenresolutionWid", Screen.currentResolution.width);
            PlayerPrefs.SetInt("screenresolutionHi", Screen.currentResolution.height);
            SetRenderResolutionQuality(Screen.currentResolution.width, Screen.currentResolution.height);
        }
    }
    private void SetRenderResolutionQuality(int width,int height)
    {
        //Load current resolution
        start_current_resolution.width = width;
        start_current_resolution.height = height;

        //divide the resolution
        int w = (int)((float)start_current_resolution.width / ResolutionQuality);
        int h = (int)((float)start_current_resolution.height / ResolutionQuality);

        //Set New Resolution
        Screen.SetResolution(w, h, true);
        print("Resolution: Width: " + w + "Height: " + h);
    }
}
