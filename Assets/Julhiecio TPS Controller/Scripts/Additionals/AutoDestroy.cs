using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("JU TPS/Utilities/Auto Destroy")]

public class AutoDestroy : MonoBehaviour
{
    public float SecondsToDestroy;
    void Start()
    {
        Destroy(this.gameObject, SecondsToDestroy);
    }
}
