using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawenBotsQuestBrand : MonoBehaviour
{
    [SerializeField] private GameObjectData warrior;
    [SerializeField] private int body_type;
    [SerializeField] private int maxWarriorCountInArea;
    [SerializeField]
    private List<MissionQuestBrandBot> warriorPool = new List<MissionQuestBrandBot>();

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

    private int ID;

    private void Start()
    {

    }

    public void InitSpawner()
    {
        if (maxWarriorCountInArea == 0)
        {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText($"{gameObject.name}: maxWarriorCountInArea == 0");
            return;
        }

        for (int i = 0; i < maxWarriorCountInArea; i++)
        {
            GameObject obj = Instantiate(warrior.warriorObjects[body_type].playerServerPrefab, transform.position, transform.rotation);
            obj.transform.position = new Vector3(transform.position.x + Random.Range(SpawnArea_X, SpawnArea_Y), transform.position.y + 2, transform.position.z + Random.Range(SpawnArea_Z, SpawnArea_W));

            if (obj.GetComponent<MissionQuestBrandBot>())
            {
                warriorPool.Add(obj.GetComponent<MissionQuestBrandBot>());
                warriorPool[i].SetQuestBrandID(ID);
            }
            else
            {
                print(" Not MissionQuestBrandBot");
            }
        }
    }

    public void SetSettingMissionQuiestBrand(int ID)
    {
        this.ID = ID;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
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
