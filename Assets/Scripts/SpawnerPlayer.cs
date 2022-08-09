using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPlayer : MonoBehaviour
{
    private static SpawnerPlayer instance;
    public static SpawnerPlayer Instance => instance;

    [SerializeField] private Transform[] spawnPos = new Transform[3];

    private void Awake() {
        instance = this;
    }

    public Vector3 GetSpawnPos() {
        return spawnPos[Random.Range(0, spawnPos.Length)].position;
    }
}
