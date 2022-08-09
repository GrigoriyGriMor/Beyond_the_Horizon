using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public SupportClass.gameState UDPState = SupportClass.gameState.test;
    private uint warrior_ID;
    public int body_type = 0;

    [Header("1 = ближник, 2 = дальник")]
    [SerializeField] private uint type;

    [Header("Ссылка на цель для Rig")]
    public Transform targetGun;

    [HideInInspector] public ushort w_attack = 0;
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


        if (UDPState == SupportClass.gameState.clone && type == 2)
            currentWeapon = GetComponent<AttackEnemy>().GetWeaponRef();
        thisTransform = transform;
    }

    public WarriorData GetWarriorData()
    {
        WarriorData newData = new WarriorData();

        newData.warriorID = warrior_ID;
        newData.w_type = type;

        newData.w_pos_X = transform.position.x;
        newData.w_pos_Y = transform.position.y;
        newData.w_pos_Z = transform.position.z;

        newData.w_rotate_X = transform.eulerAngles.x;
        newData.w_rotate_Y = transform.eulerAngles.y;
        newData.w_rotate_Z = transform.eulerAngles.z;

        newData.w_heal_count = damageModule.GetHeal();
        newData.w_shield_count = damageModule.GetShield();

        if (type == 2)
        {
            newData.w_target_posX = targetGun.position.x;
            newData.w_target_posY = targetGun.position.y;
            newData.w_target_posZ = targetGun.position.z;

            newData.distance_w_attack = w_attack;
        }

        float[] layerW = new float[2];
        for (int i = 0; i < _enemyAnimator.animator.layerCount; i++)
            layerW[i] = _enemyAnimator.animator.GetLayerWeight(i);

        newData.animLayerWeight = layerW;

        AnimatorParamData[] apd = new AnimatorParamData[_enemyAnimator.animator.parameterCount];
        for (int i = 0; i < _enemyAnimator.animator.parameterCount; i++)
        {
            apd[i].indexParam = i;

            switch (_enemyAnimator.animator.parameters[i].type)
            {
                case (AnimatorControllerParameterType.Float):
                    apd[i].type = 0;
                    apd[i].defaultFloat = _enemyAnimator.animator.GetFloat(_enemyAnimator.animator.parameters[i].name);
                    break;
                case (AnimatorControllerParameterType.Bool):
                    apd[i].type = 1;
                    if (_enemyAnimator.animator.GetBool(_enemyAnimator.animator.parameters[i].name))
                        apd[i].defaultBool = 1;
                    else
                        apd[i].defaultBool = 0;
                    break;
                case (AnimatorControllerParameterType.Trigger):
                    apd[i].defaultTrigger = 0;
                    for (int j = 0; j < _enemyAnimator.triggerActive.Count; j++)
                    {
                        if (i == _enemyAnimator.triggerActive[j])
                        {
                            apd[i].defaultTrigger = 1;
                            _enemyAnimator.triggerActive.RemoveAt(j);
                        }
                    }
                    apd[i].type = 2;
                    break;
                case (AnimatorControllerParameterType.Int):
                    apd[i].type = 3;
                    apd[i].defaultInt = _enemyAnimator.animator.GetInteger(_enemyAnimator.animator.parameters[i].name);
                    break;
            }
        }
        newData.animParam = apd;

        return newData;
    }

    public void SetServerParamSinc(Vector3 w_Pos, Vector3 w_Rotation, float w_Heal, float w_Shield, Vector3 w_target_Pos,
        ushort w_attack, float[] _animLayerWeight, AnimatorParamData[] _animParam)
    {

        currentWarriorPosition = w_Pos;
        currentWarriorRotation = w_Rotation;

        damageModule.SetHeal(w_Heal);
        damageModule.SetShield(w_Shield);

        if (type == 2)
        {
            targetGun.position = w_target_Pos;
            currentAttack = w_attack;
        }

        if (_animLayerWeight.Length <= _enemyAnimator.animator.layerCount)
            for (int i = 0; i < _animLayerWeight.Length; i++)
                _enemyAnimator.animator.SetLayerWeight(i, _animLayerWeight[i]);
        else
            for (int i = 0; i < _enemyAnimator.animator.layerCount; i++)
                _enemyAnimator.animator.SetLayerWeight(i, _animLayerWeight[i]);

        for (int j = 0; j < _animParam.Length; j++)
        {
            string name;
            if (_animParam[j].indexParam < _enemyAnimator.animator.parameterCount)
                name = _enemyAnimator.animator.GetParameter(_animParam[j].indexParam).name;
            else
            {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText($"Enemy Animator param count {_enemyAnimator.animator.parameterCount} <= server_anim_param_count {_animParam[j].indexParam}");
                break;
            }

            switch (_animParam[j].type)
            {
                case (0):
                    _enemyAnimator.animator.SetFloat(name, _animParam[j].defaultFloat);
                    break;
                case (1):
                    if (_animParam[j].defaultBool == 1)
                        _enemyAnimator.animator.SetBool(name, true);
                    else
                        _enemyAnimator.animator.SetBool(name, false);
                    break;
                case (2):
                    if (_animParam[j].defaultTrigger == 1)
                        _enemyAnimator.animator.SetTrigger(name);
                    break;
                case (3):
                    _enemyAnimator.animator.SetInteger(name, _animParam[j].defaultInt);
                    break;
            }
        }
    }

    private ushort currentAttack = 0;
    private Vector3 currentWarriorPosition;
    private Vector3 currentWarriorRotation;
    private void Update()
    {
        if (UDPState == SupportClass.gameState.clone)
        {

            transform.position = Vector3.LerpUnclamped(transform.position, currentWarriorPosition, 15 * Time.deltaTime);

            Quaternion tr = Quaternion.Euler(currentWarriorRotation);
            transform.rotation = Quaternion.LerpUnclamped(transform.rotation, tr, 15 * Time.deltaTime);

            if (currentWeapon != null && currentAttack == 1)
                currentWeapon.Fire();
        }
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