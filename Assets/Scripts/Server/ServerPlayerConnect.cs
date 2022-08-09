using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ServerPlayerConnect : MonoBehaviour 
{
    [Header("Шаблоны клиентов игроков")]
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private GameObjectData gameObjectData;

    Vector3 point;

    public InputPlayerManager ConnectNewClient(char[] _name, ushort body_type) 
    {
        string name = "";
        for (int i = 0; i < _name.Length; i++)
              name += _name[i];

        try {
            if (SpawnerPlayer.Instance) {
                point = SpawnerPlayer.Instance.GetSpawnPos();
            }
            else
                point = new Vector3(spawnPoint.position.x + (UnityEngine.Random.Range(-3, 3)), spawnPoint.position.y, spawnPoint.position.z + (UnityEngine.Random.Range(-3, 3)));

            GameObject player = Instantiate(gameObjectData.playerObjects[body_type].playerServerPrefab, point, spawnPoint.rotation);
            if (player.GetComponent<PlayerController>()) player.GetComponent<PlayerController>().InitPlayer(name, body_type);
            return player.GetComponent<InputPlayerManager>();
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "New Client connect error: ");
            return null;
        }
        }

    public PlayerController ConnectNewClone(char[] _name, ushort body_type) {
        string name = "";
        for (int i = 0; i < _name.Length; i++)
            name += _name[i];

        try {
            PlayerController player = Instantiate(gameObjectData.playerObjects[body_type].playerCloneForClient, new Vector3(0, -15, 0), Quaternion.identity).GetComponent<PlayerController>();
            if (player != null) player.InitPlayer(name);
            return player;
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "New Client connect error: ");
            return null;
        }
    }

    public EnemyBase ConnectNewWarriorClone(ushort body_type) {
        try {
            EnemyBase warrior = Instantiate(gameObjectData.warriorObjects[body_type].playerCloneForClient, Vector3.zero, Quaternion.identity).GetComponent<EnemyBase>();
            return warrior;
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "New warrior connect error: ");
            return null;
        }
    }
}


