using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("JU TPS/Mobile/Mobile HUD Animator")]
public class MobileHUDAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public float SimulationSpeed = 3f;
    public Color NormalColor = new Color(1, 1, 1, 0.5f), PressedColor = new Color(1, 1, 1, 1f);
    public float NormalScale = 1f, PressedScale = 1.15f;

    public JoystickVirtual Joystick;
    public Image RunButton;
    public Image RunButtonRight;
    public Image PickupButton;

    private Image _runbuttonimage;

    private Image _joystickimage;

    private ButtonVirtual[] MobileButtons;
    private Image[] ImageButtons;

    void Start()
    {
        if (MobileButtons == null)
        {
            MobileButtons = GetComponentsInChildren<ButtonVirtual>();
        }
        ImageButtons = new Image[MobileButtons.Length];
        _joystickimage = Joystick.GetComponent<Image>();
       
        RunButtonRight = RunButtonRight.GetComponent<Image>();
        _runbuttonimage = RunButton.GetComponent<Image>();

        for (int i = 0; i < MobileButtons.Length; i++)
        {
            ImageButtons[i] = MobileButtons[i].GetComponent<Image>();
        }
    }
    void Update()
    {
        for (int i = 0; i < MobileButtons.Length; i++)
        {
            if (MobileButtons[i].IsPressedVisual)
            {
                // >>> COLOR ANIMATION
                if (ImageButtons[i].gameObject != PickupButton)
                {
                    ImageButtons[i].color = Color.Lerp(ImageButtons[i].color, PressedColor, SimulationSpeed * Time.unscaledDeltaTime);
                }


                // >>> SCALE ANIMATION
                if (ImageButtons[i].gameObject != PickupButton)
                {
                    ImageButtons[i].transform.localScale = Vector3.Lerp(ImageButtons[i].transform.localScale,
                        new Vector3(PressedScale, PressedScale, PressedScale), SimulationSpeed * Time.unscaledDeltaTime);
                }
            }
            else
            {
                // >>> COLOR ANIMATION
                if (ImageButtons[i].gameObject != PickupButton)
                {
                    ImageButtons[i].color = Color.Lerp(ImageButtons[i].color, NormalColor, SimulationSpeed * Time.unscaledDeltaTime);
                }

                // COLOR ANIMATION
                if (ImageButtons[i].gameObject != RunButton && RunButtonRight && ImageButtons[i].gameObject != PickupButton)
                {
                    ImageButtons[i].transform.localScale = Vector3.Lerp(ImageButtons[i].transform.localScale,
                        new Vector3(NormalScale, NormalScale, NormalScale), SimulationSpeed * Time.unscaledDeltaTime);
                }
            }
        }

        if (Joystick.IsPressed)
        {
            _joystickimage.color = Color.Lerp(_joystickimage.color, PressedColor, SimulationSpeed * Time.unscaledDeltaTime);

            _joystickimage.transform.localScale = Vector3.Lerp(_joystickimage.transform.localScale,
                    new Vector3(PressedScale, PressedScale, PressedScale), SimulationSpeed * Time.unscaledDeltaTime);
        }
        else
        {
            _joystickimage.color = Color.Lerp(_joystickimage.color, NormalColor, SimulationSpeed * Time.unscaledDeltaTime);

            _joystickimage.transform.localScale = Vector3.Lerp(_joystickimage.transform.localScale,
                    new Vector3(NormalScale, NormalScale, NormalScale), SimulationSpeed * Time.unscaledDeltaTime);
        }
    }
}
