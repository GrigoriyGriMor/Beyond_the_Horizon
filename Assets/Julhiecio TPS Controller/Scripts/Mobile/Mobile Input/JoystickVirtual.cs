using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("JU TPS/Mobile/Joystick")]
public class JoystickVirtual : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Range(0,1)]
    public float JoystickMaxDistance = 0.45f;
    public Image BackgroundImage;
    public Image JoystickImage;

    private Vector3 _inputVector;

    public bool IsPressed;
    public Vector3 InputVector
    {
        get
        {
            return _inputVector;
        }
    }
    public void OnPointerDown(PointerEventData e)
    {
        IsPressed = true;
        OnDrag(e);
    }

    public void OnDrag(PointerEventData e)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(BackgroundImage.rectTransform,
                                                                    e.position,
                                                                    e.pressEventCamera,
                                                                    out pos))
        {

            pos.x = (pos.x / BackgroundImage.rectTransform.sizeDelta.x);
            pos.y = (pos.y / BackgroundImage.rectTransform.sizeDelta.y);

            _inputVector = new Vector3(pos.x * 2 + 1, 0, pos.y * 2 - 1);
            _inputVector = (_inputVector.magnitude > 1.0f) ? _inputVector.normalized : _inputVector;


            JoystickImage.rectTransform.anchoredPosition = new Vector3(_inputVector.x * (BackgroundImage.rectTransform.sizeDelta.x * JoystickMaxDistance),
                                                                     _inputVector.z * (BackgroundImage.rectTransform.sizeDelta.y * JoystickMaxDistance));
        }
    }

    public void OnPointerUp(PointerEventData e)
    {
        IsPressed = false;
        _inputVector = Vector3.zero;
        JoystickImage.rectTransform.anchoredPosition = Vector3.zero;
    }


    public float Horizontal()
    {
        if (_inputVector.x != 0)
        {
            return _inputVector.x;
        }

        return Input.GetAxis("Horizontal");
    }

    public float Vertical()
    {
        if (_inputVector.z != 0)
        {
            return _inputVector.z;
        }

        return Input.GetAxis("Vertical");
    }

}