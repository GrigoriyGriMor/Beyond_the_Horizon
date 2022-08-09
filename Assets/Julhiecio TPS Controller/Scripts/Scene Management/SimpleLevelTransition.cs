﻿using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("JU TPS/Scene Management/Trigger Load Level")]
public class SimpleLevelTransition : MonoBehaviour
{
    [SerializeField]string DesiredLevelName = "Hub";
    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            SceneManager.LoadScene(DesiredLevelName);
        }
    }
}
