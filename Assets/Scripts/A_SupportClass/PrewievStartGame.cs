using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrewievStartGame : MonoBehaviour
{
    [SerializeField] private GameObject screenDemo;

    private void Update() {
        if (Input.anyKeyDown)
            Destroy(screenDemo);
    }

}
