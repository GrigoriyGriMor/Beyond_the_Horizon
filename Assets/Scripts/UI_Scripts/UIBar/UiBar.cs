using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiBar : MonoBehaviour
{
    private Transform thisTransform;
    void Start()
    {
        thisTransform = GetComponent<Transform>();   
    }

    void Update()
    {
        if (thisTransform != null && Camera.main) thisTransform.LookAt(Camera.main.transform);
    }
}
