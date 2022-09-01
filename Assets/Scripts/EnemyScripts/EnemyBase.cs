using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private uint warrior_ID;
    public int body_type = 0;

    [Header("1 = ближник, 2 = дальник")]
    [SerializeField] private uint type;

    [Header("Ссылка на цель для Rig")]
    public Transform targetGun;

    private WeaponController currentWeapon;
    private EnemyAnimator _enemyAnimator;
    private InDamageModule damageModule;


    [Header("__________________________________________")]
    [Header("Drop Block")]
    [Header("Кол-во предметов которое может выпасть с монстра")]
    [SerializeField]
    private int minCountDropItems;
    [SerializeField]
    private int maxCountDropItems;
    [Header("Радиус спавна лута")]
    [SerializeField]
    private float radius = 1.0f;

    [Header("Массив Loot")]
    [SerializeField]
    private DataLoot[] lootArray;
    [SerializeField]
    private GameObjectData Scr_GameObjectDataTest;

    private Transform thisTransform;

    public Transform currentTargetPlayer;


    [SerializeField]
    private bool isDead;

    [System.Serializable]
    public class DataLoot
    {
        [Header("ID предмета")]
        public int ID;
        [Header("Шанс выпадения предмета")]
        [Range(0, 100)]
        public int percentDrop;
        [Header("Кол-во предметов этого типа которое может выпасть")]
        public int minCountDropThisItems = 1;
        public int maxCountDropThisItems;
        [HideInInspector]
        public bool isSelect = true;
    }

    [HideInInspector]
    public bool isAggression;

    public void SetID(uint _id)
    {
        warrior_ID = _id;
    }

    public uint GetWarriorID()
    {
        return warrior_ID;
    }

    private void Awake()
    {
        type = (uint)(GetComponent<EnemyControllerNear>() ? 1 : 2);
        _enemyAnimator = GetComponent<EnemyAnimator>();
        damageModule = GetComponent<InDamageModule>();
        damageModule.deach.AddListener(DropItem);

        thisTransform = transform;
    }

    public void CreateItem(Vector3 positionItem, Quaternion rotationItem)
    {
        int indexlootDrop = Random.Range(0, lootArray.Length);

        for (int index = 0; index < Scr_GameObjectDataTest.otherObjects.Count; index++)
        {
            if (index == lootArray[indexlootDrop].ID)
            {
                if (lootArray[indexlootDrop].isSelect)
                {
                    break;
                }

                if (Random.Range(0, 101) > lootArray[indexlootDrop].percentDrop)
                {
                    lootArray[indexlootDrop].isSelect = true;
                    break;
                }

                ItemAtSceneController itemAtSceneController = Instantiate(Scr_GameObjectDataTest.otherObjects[index].GetVisualForScene(), positionItem, rotationItem).GetComponent<ItemAtSceneController>();

                if (!itemAtSceneController) break;

                int countDropThisItems = Random.Range(lootArray[indexlootDrop].minCountDropThisItems, lootArray[indexlootDrop].maxCountDropThisItems);
                lootArray[indexlootDrop].isSelect = true;

                itemAtSceneController.SetItemCount(countDropThisItems);
            }
        }
    }

    public void DropItem()
    {
        int countDropItem = Random.Range(minCountDropItems, maxCountDropItems + 1);
        if (countDropItem > lootArray.Length) countDropItem = lootArray.Length;

        for (int i = 0; i <= countDropItem; i++)
        {
            float angle = i * Mathf.PI * 2 / countDropItem;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            Vector3 positionItem = thisTransform.position + new Vector3(x, thisTransform.position.y + 0.5f, z);
            float angleDegrees = -angle * Mathf.Rad2Deg;
            Quaternion rotationItem = Quaternion.Euler(0, angleDegrees, 0);
            CreateItem(positionItem, rotationItem);
        }

        for(int i = 0; i < lootArray.Length; i++)
        {
            lootArray[i].isSelect = false;
        }

    }
}