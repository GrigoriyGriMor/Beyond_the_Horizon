using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionWindowDialog : MonoBehaviour
{
    [SerializeField]
    private Transform pointWindowInfo;

    private RectTransform rectTransform;

    private void OnEnable()
    {
        Init();    
    }

    private void Init()
    {
        rectTransform = GetComponent<RectTransform>();
       // gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
    }

    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 parentObjectPosition = pointWindowInfo.position;

        rectTransform.position = Camera.main.WorldToScreenPoint(parentObjectPosition);
    }
}
