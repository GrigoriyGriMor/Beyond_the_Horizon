using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorSpawner : MonoBehaviour
{
    [SerializeField] private GameObjectData warrior;
    [SerializeField] private int body_type;
    [SerializeField] private int maxWarriorCountInArea;
    private List<GameObject> warriorPool = new List<GameObject>();

    [Header("Для разработчиков")]
    [SerializeField] private Color visualAreaColor = Color.blue;

    [Range(-1000, 0)]
    [SerializeField] private float SpawnArea_X;

    [Range(0, 1000)]
    [SerializeField] private float SpawnArea_Y;

    [Range(-1000, 0)]
    [SerializeField] private float SpawnArea_Z;

    [Range(0, 1000)]
    [SerializeField] private float SpawnArea_W;


    [SerializeField] private float spawnTime = 3;

    void Start()
    {
        if (maxWarriorCountInArea == 0) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText($"{gameObject.name}: maxWarriorCountInArea == 0");
            return;
        }

        for (int i = 0; i < maxWarriorCountInArea; i++) {
            GameObject obj = Instantiate(warrior.warriorObjects[body_type].playerServerPrefab, transform.position, transform.rotation);
            warriorPool.Add(obj);
            obj.SetActive(false);
        }

        StartCoroutine(SpawnWarrior());
    }

    private IEnumerator SpawnWarrior() {
        yield return new WaitForSeconds(spawnTime);

        GameObject go = GetFree();

        if (go == null) {
            StartCoroutine(SpawnWarrior());
            yield break;
        }

        go.SetActive(true);
        go.transform.position = new Vector3(transform.position.x + Random.Range(SpawnArea_X, SpawnArea_Y), transform.position.y + 2, transform.position.z + Random.Range(SpawnArea_Z, SpawnArea_W));

        yield return new WaitForFixedUpdate();

        StartCoroutine(SpawnWarrior());
    }

    private GameObject GetFree() {
        List<GameObject> freeObj = new List<GameObject>();

        for (int i = 0; i < warriorPool.Count; i++) {
            if (!warriorPool[i].activeInHierarchy)
                freeObj.Add(warriorPool[i]);
        }

        if (freeObj.Count > 0)
            return freeObj[Random.Range(0, freeObj.Count)];
        else
            return null;
    }


    #if UNITY_EDITOR
        private void OnDrawGizmos() {
            Gizmos.color = visualAreaColor;

            Gizmos.DrawCube(new Vector3(transform.position.x + SpawnArea_X, transform.position.y, transform.position.z), 
                new Vector3(1, 5, 1)); //Рисуем куб

            Gizmos.DrawCube(new Vector3(transform.position.x + SpawnArea_Y, transform.position.y, transform.position.z),
                new Vector3(1, 5, 1));

            Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y, transform.position.z + SpawnArea_Z),
                new Vector3(1, 5, 1));

            Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y, transform.position.z + SpawnArea_W),
                new Vector3(1, 5, 1));

        }
    #endif
}
