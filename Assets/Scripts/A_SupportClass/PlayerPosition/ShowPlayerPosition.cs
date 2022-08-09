using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShowPlayerPosition : MonoBehaviour
{
    Transform thisTransform;
    [SerializeField]
    private Text textFieldXZ;


    private void Start()
    {
        thisTransform = GetComponentInParent<PlayerController>().transform;
    }

    void FixedUpdate()
    {
        textFieldXZ.text = "X: " + Mathf.Round(thisTransform.position.x).ToString() + "   Z: " + Mathf.Round(thisTransform.position.z).ToString();
    }
}
