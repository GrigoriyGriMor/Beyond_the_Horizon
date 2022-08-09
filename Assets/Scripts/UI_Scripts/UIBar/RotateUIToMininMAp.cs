using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RotateUIToMininMAp : MonoBehaviour
{
    private RectTransform rectTransform;

    [SerializeField]
    private Transform cameraMiniMap;


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

    }

    void FixedUpdate()
    {
        if(cameraMiniMap) rectTransform.rotation = Quaternion.Euler(rectTransform.eulerAngles.x, cameraMiniMap.eulerAngles.y, rectTransform.eulerAngles.z);
    }
}
