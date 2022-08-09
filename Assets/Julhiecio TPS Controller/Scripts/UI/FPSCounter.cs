using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("JU TPS/UI/FPS Counter")]
public class FPSCounter : MonoBehaviour
{
    [SerializeField] private Text FPSText;
    public float RefreshRate;
    void Start()
    {
        InvokeRepeating("UpdateFrameRateOnScreen", 0, RefreshRate);
    }
    void UpdateFrameRateOnScreen()
    {
        if (FPSText == null) return;
        FPSText.text = FrameRate() + "FPS";
        FPSText.color = Color.Lerp(Color.red, Color.green, FrameRate() / 60f);
    }
    public int FrameRate()
    {
        float current = 0;
        current = Time.frameCount / Time.time;
        int fps = (int)(1f / Time.unscaledDeltaTime);
        return fps;
    }
}
