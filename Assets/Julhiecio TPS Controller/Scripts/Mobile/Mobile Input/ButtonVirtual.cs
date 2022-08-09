using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("JU TPS/Mobile/Button")]
public class ButtonVirtual : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsPressed;
    public bool IsPressedVisual;
    public bool IsPressedDown;
    public bool IsPressedUp;
    public void OnPointerDown(PointerEventData e)
    {
        IsPressed = true;
        IsPressedVisual = true;
        IsPressedUp = false;

        IsPressedDown = true;
        StartCoroutine(DisableIsPressedDownAtEndOfFrame());
    }
    
    public void OnPointerUp(PointerEventData e)
    {
        IsPressed = false;
        IsPressedVisual = false;
        IsPressedDown = false;
        IsPressedUp = true;
        StartCoroutine(DisableIsPressedUpAtEndOfFrame());
    }
    IEnumerator DisableIsPressedDownAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        IsPressedDown = false;
    }

    IEnumerator DisableIsPressedUpAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        IsPressedUp = false;
    }

}
