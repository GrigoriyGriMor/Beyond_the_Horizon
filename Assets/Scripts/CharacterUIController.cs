using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIController : MonoBehaviour
{
    [SerializeField] private Transform cameraObj;
    [SerializeField] private RectTransform playerArrowRect;

    [SerializeField] private Transform playerVisualObj;
    [SerializeField] private RectTransform playerArrow;

    private void Update()
    {
        if (playerArrowRect != null) playerArrowRect.localEulerAngles = new Vector3(playerArrowRect.localEulerAngles.x, playerArrowRect.localEulerAngles.y, cameraObj.eulerAngles.y);
        if (playerArrow != null) playerArrow.localEulerAngles = new Vector3(playerArrow.localEulerAngles.x, playerArrow.localEulerAngles.y, 360 - playerVisualObj.localEulerAngles.y);
    }
}
