using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoInstantiate : MonoBehaviour
{
    [JUHeader("Auto Instantiate Prefab")]
    public GameObject Prefab;
    public float TimeToSpawn = 2;
    public bool Repeat;
    public float RepeatingTime = 1;

    private void Start()
    {
        if(Repeat == true)
        {
            InvokeRepeating("InstantiatePrefab", TimeToSpawn, RepeatingTime);
        }
        else
        {
            Invoke("InstantiatePrefab", TimeToSpawn);
        }
    }
    public void InstantiatePrefab()
    {
        Instantiate(Prefab, transform.position, transform.rotation);
    }
}
