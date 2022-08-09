using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameObj Data", menuName = "GameObj Data", order = 115)]

public class GameObjectData : ScriptableObject
{
    [Header("префабы персонажей игроков")]
    public List<PlayerPrefabs> playerObjects = new List<PlayerPrefabs>();

    [Header("префабы врагов (ботов)")]
    public List<PlayerPrefabs> warriorObjects = new List<PlayerPrefabs>();

    [Header("префабы NPC")]
    public List<GameObject> NPCObjects = new List<GameObject>();

    [Header("префабы транпорта")]
    public List<GameObject> transportObjects = new List<GameObject>();

    [Header("префабы иных объектов")]
    public List<ItemBaseParametrs> otherObjects = new List<ItemBaseParametrs>();

    [Header("Материалы грейдов")]
    public List<Material> gradeMaterials = new List<Material>();
}

[System.Serializable]
public class PlayerPrefabs {
    public string name;
    public GameObject playerClientPrefab;
    public GameObject playerCloneForClient;
    public GameObject playerServerPrefab;
    public GameObject playerPrefabForUI;
}