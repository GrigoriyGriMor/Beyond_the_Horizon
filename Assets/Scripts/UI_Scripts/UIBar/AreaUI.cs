using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaUI : MonoBehaviour
{
    [SerializeField]
    private GameObject imageArea;

    [HideInInspector]
    public bool visible;

    private void Start()
    {
        SetActiveImageArea(false);
    }

    private void OnBecameVisible()
    {
        visible = true;
    }

    private void OnBecameInvisible()
    {
        visible = false;
    }

    public void SetActiveImageArea(bool isActiveImageArea)
    {
       if(imageArea) imageArea.SetActive(isActiveImageArea);
    }

}
