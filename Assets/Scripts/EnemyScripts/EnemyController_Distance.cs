using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public enum StateEnemy
{
    idle,
    patrol,
    follow,
    attack,
    retreat,
    death,
    followAfterRetreat
}

public class EnemyController_Distance : EnemyBase
{
    [Header(" Name bota")]
    [SerializeField]
    private string nameEnemy;

    [Header(" ������� ������� ")]
    [SerializeField]
    public int levelEnemy = 1;

    [Header("Trigger Name")]
    public string walk;
    public string run;
    public string attack;
    public string death;
    public string reload;
    public string speed;
    public string jump;
    public string callForHelp;
    public string animAttentionAttack = "AttentionAttack";


    public EnemyAnimator enemyAnimator;
    public NavMeshAgent navMeshAgent;
    public InDamageModule inDamageModule;
    public AttackEnemy attackEnemy;
    public MoveEnemy MoveEnemy;
    public Collider colliderEnemy;
    public Transform UiHealthBar;
    private Rig rigObject;

    [SerializeField]
    private Text textNameEnemy;

    [SerializeField]
    private Text textLevelEnemy;

    [SerializeField] private int bodyUpLayer = 1;

    /// <summary>
    /// ��� - �� ����� 
    /// </summary>
    [Header("��� - �� ����� ")]
    [SerializeField]
    public int countExpirience = 1;

    /// <summary>
    /// ��� - �� ������
    /// </summary>
    [Header("��� - �� ������ ")]
    public int countMoney = 10;

    /// <summary>
    /// Max ��������
    /// </summary>
    [Header("��������")]
    [SerializeField]
    private float maxHealth = 100;

    /// <summary>
    /// ������� ��������
    /// </summary>
    private float currentHeal;

    /// <summary>
    /// Max ������
    /// </summary>
    [Header("Max ������")]
    [SerializeField]
    private float maxShield = 20;

    /// <summary>
    /// ���� �����
    /// </summary>
    [Header("���� �����")]
    public float damageAttack = 10;

    /// <summary>
    /// �������� �����
    /// </summary>
    [Header("�������� �����")]
    [SerializeField]
    private float speedAttack = 1;

    /// <summary>
    /// ���-�� ��������
    /// </summary>
    [Header("���-�� ��������")]
    [SerializeField]
    private int countBullet = 30;

    /// <summary>
    /// �������� �����������
    /// </summary>
    [Header("�������� �����������")]
    [SerializeField]
    private float speedReload = 1.5f;

    /// <summary>
    /// �������������
    /// </summary>
    [Header("�������������")]
    [SerializeField]
    private bool aggression = false;

    private bool currentAggression;

    /// <summary>
    /// ����� �������������
    /// </summary>
    [Header("����� �������������")]
    [SerializeField]
    private float timerFollow = 5;

    /// <summary>
    /// ����� �������������� ����� �������������
    /// </summary>
    [SerializeField]
    private float timerPatrol = 2.0f;

    /// <summary>
    /// ������������ ��������� ��������� ����������
    /// </summary>
    [SerializeField]
    [Header("������������ ��������� ��������� ����������  ")]
    private float maxDistanceVisibleEnemy = 7;

    /// <summary>
    /// �������������� �� �����
    /// </summary>
    [Header("�������������� �� �����")]
    [SerializeField]
    private bool attentionAttack;

    [Header("����� ����")]
    [SerializeField]
    private AudioClip audioAttentionAttack;

    /// <summary>
    /// ������ �������������� 
    /// </summary>
    [SerializeField]
    [Header("������ �������������� ")]
    private float radiusPatrol = 10;

    /// <summary>
    /// start ����� spawn
    /// </summary>
    private Vector3 startPosition;

    /// <summary>
    /// �������� ��������������
    /// </summary>
    [SerializeField]
    [Header("�������� ��������������  ")]
    private float speedPatrol = 1.0f;

    /// <summary>
    /// �������� �������������
    /// </summary>
    [SerializeField]
    [Header("�������� �������������")]
    private float speedRun = 3.5f;

    /// <summary>
    /// ����� ����
    /// </summary>
    [Header("����� ����")]
    [SerializeField]
    private float chargeShield = 20;

    /// <summary>
    /// ��� ��������� ����� �� ������
    /// </summary>
    [SerializeField]
    [Header("��� ���������� ����� �� ������")]
    private float maxDistanceOffSpawn = 12;

    /// <summary>
    /// ��������� ����������� �����
    /// </summary>
    [SerializeField]
    [Header("��������� ����������� �����  ")]
    private float distanceAttack = 3;

    /// <summary>
    /// ������� �� ������ 
    /// </summary>
    [Header("������� �� ������ ")]
    [SerializeField]
    private bool needHelp;

    [Header("������ ���������")]
    public itemsEnemy[] arrayItem;

    /// <summary>
    ///����������� ��������� �����
    /// </summary>
    [SerializeField]
    [Header("����������� ��������� ����� ")]
    private float minDistanceAttack = 1f;

    /// <summary>
    /// ��������� �� ������� ����� ����������� � �����
    /// </summary>
    [Header("��������� �� ������� ����� ����������� � ����� ")]
    [SerializeField]
    private float distanceToChangeTarget = 1.5f;

    /// <summary>
    /// ������ ����� ����
    /// </summary>
    [SerializeField]
    [Header("������ ����� ���� ")]
    private float timerChangeTarget = 2;

    /// <summary>
    /// �������� �� ����������
    /// </summary>
    [Header("�������� �� ����������")]
    [SerializeField]
    private bool keepDistance;

    /// <summary>
    /// ������ ������
    /// </summary>
    [Header("����� ������ ��� ���������")]
    [SerializeField]
    private float maxTimeRetreat = 3;

    /// <summary>
    /// ������ ����� ������� ��� �����
    /// </summary>
    [Header("����� ����� ������� ��� �����")]
    [SerializeField]
    private float maxTimeChangePos = 2;

    private float distToSpawn;

    /// <summary>
    /// ������� ����
    /// </summary>
    [HideInInspector]
    [Header("������� ����")]
    public Vector3 currentTarget;

    /// <summary>
    /// ������� ����
    /// </summary>
    [Header("������� ����")]
    private Vector3 navMeshTarget;

    private float distToTarget;

    /// <summary>
    /// vector ���������� ��� ������ ��� ����� �������
    /// </summary>
    private Vector3 vectorDirection;

    /// <summary>
    /// ������ �� ������
    /// </summary>
    //[HideInInspector]
    // [Header("������ �� ������ ")]
    //public Transform currentTargetPlayer;

    /// <summary>
    /// ������ �� ��������
    /// </summary>
    private Coroutine refTimerSelectTargetCoroutine;
    private Coroutine refTimerRetreatCoroutine;
    private Coroutine refTimerAttackCoroutine;
    private Coroutine refTimerFollowCoroutine;
    private Coroutine refTimerPatrolCoroutine;
    private Coroutine refTimerCheckAllEnemyCoroutine;
    private Coroutine checkEnemyCoroutine;

    [SerializeField]
    private StateEnemy stateEnemy;

    private float speedIdle = 0.0f;

    private float speedAttackNavMesh = 0;

    /// <summary>
    /// ��������� �� ����
    /// </summary>
    private float distToEnemy;

    /// <summary>
    /// ������� ��������
    /// </summary>
    private float currentSpeed;

    private float currentLerpSpeed;

    private float timeRetreat;                           // ����� ��� ������ ��� ����� �������

    private InDamageModule inDamageModuleEnemy;

    private bool isRetreat;

    [Header(" ������ ����� ������")]
    [SerializeField]
    private float timerDeath = 3.0f;

    private ControlQuestBase[] questAction = new ControlQuestBase[1];
    private MissionQuestBrandBase[] missionQuestBrandBases = new MissionQuestBrandBase[1];

    private void Start()
    {
        questAction = GetComponents<ControlQuestBase>();
        if (questAction == null)
            questAction = GetComponentsInChildren<ControlQuestBase>();

        missionQuestBrandBases = GetComponents<MissionQuestBrandBase>();                         //solo
        if (missionQuestBrandBases == null)                                                      //solo
            missionQuestBrandBases = GetComponentsInChildren<MissionQuestBrandBase>();           //solo


        UpdateUIEnemy();

    }

    private void OnEnable()
    {
            Init();
    }
    private void Update()
    {
        SetState();
    }

    private void Init()
    {
        refTimerSelectTargetCoroutine = null;
        refTimerAttackCoroutine = null;
        refTimerFollowCoroutine = null;
        refTimerPatrolCoroutine = null;
        refTimerCheckAllEnemyCoroutine = null;
        checkEnemyCoroutine = null;

        if (enemyAnimator == null) enemyAnimator = GetComponent<EnemyAnimator>();
        if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();
        if (inDamageModule == null) inDamageModule = GetComponent<InDamageModule>();
        if (attackEnemy == null) attackEnemy = GetComponent<AttackEnemy>();
        if (rigObject == null) rigObject = GetComponentInChildren<Rig>();

        startPosition = transform.position;
        UiHealthBar.gameObject.SetActive(true);
        inDamageModule.ReloadParam();
        currentHeal = inDamageModule.GetHeal();
        colliderEnemy.enabled = true;
        currentAggression = false;
        isAggression = aggression;
        currentTargetPlayer = null;
        navMeshAgent.isStopped = false;
        stateEnemy = StateEnemy.idle;
        currentTarget = Vector3.zero;
        navMeshTarget = Vector3.zero;
        navMeshTarget = Vector3.zero;
    }


    /// <summary>
    /// ����� ��������� ����
    /// </summary>
    private void SetState()
    {
        distToSpawn = Vector3.Distance(transform.position, startPosition);                   // ��������� �� spawn

        if (stateEnemy != StateEnemy.death)
        {
            CheckInDamage();
        }

        switch (stateEnemy)
        {
            case StateEnemy.idle:
                Idle();
                break;

            case StateEnemy.patrol:
                Patrol();
                break;

            case StateEnemy.follow:
                Follow();
                break;

            case StateEnemy.attack:
                Attack();

                break;

            case StateEnemy.retreat:
                Retreat();
                break;

            case StateEnemy.followAfterRetreat:
                FollowAfterRetreat();
                break;


            case StateEnemy.death:

                break;

            default:
                break;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, currentLerpSpeed, Time.fixedDeltaTime * 5.0f);

        navMeshAgent.SetDestination(navMeshTarget);
    }


    /// <summary>
    /// �����
    /// </summary>
    private void Idle()
    {
        if (refTimerSelectTargetCoroutine == null)
        {
            refTimerSelectTargetCoroutine = StartCoroutine(TimerSelectTarget());
            currentSpeed = speedIdle;
            navMeshAgent.speed = currentSpeed;
            enemyAnimator.SetStateAnimator(speed, currentSpeed);
        }

        // ���� ��� ���������� � ��� ����������
        if ((aggression && !currentTargetPlayer) || currentAggression)
        {
            if (checkEnemyCoroutine == null)
                checkEnemyCoroutine = StartCoroutine(CheckAllEnemy(maxDistanceVisibleEnemy));   // ���� �����������
        }

        // ���� ���� ��������� �� ��� �������������
        if (currentTargetPlayer)
        {
            stateEnemy = StateEnemy.follow;
        }
    }


    /// <summary>
    /// ��������������
    /// </summary>
    private void Patrol()
    {
        isAggression = aggression;

        if (enemyAnimator.animator.GetLayerWeight(1) != 0)
            enemyAnimator.SetAnimatorLayerWeight(1, 0);

        if (!currentTargetPlayer)          // ���� ���� Aiming �� ���������
        {
            enemyAnimator.SetStateAnimator(attackEnemy.aiming, false);
        }

        distToTarget = navMeshAgent.remainingDistance;   // ��������� �� ������� ����

        if (distToTarget <= distanceToChangeTarget)  // ���� ������� � ����
        {
            stateEnemy = StateEnemy.idle;
        }
        else
        {
            if (distToSpawn > radiusPatrol)
            {
                currentLerpSpeed = speedRun;
            }
            else
            {
                currentLerpSpeed = speedPatrol;
            }

            navMeshAgent.speed = currentSpeed;
            enemyAnimator.SetStateAnimator(speed, currentSpeed);
        }

        // ���� ��� ���������� � ��� ����������
        if ((aggression && !currentTargetPlayer) || currentAggression)
        {
            if (checkEnemyCoroutine == null)
                checkEnemyCoroutine = StartCoroutine(CheckAllEnemy(maxDistanceVisibleEnemy)); // ���� �����������
        }

        // ���� ���� ��������� �� ��� �������������
        if (currentTargetPlayer)// && distToSpawn < maxDistanceOffSpawn)
        {
            stateEnemy = StateEnemy.follow;
        }

    }

    /// <summary>
    /// �������������
    /// </summary>
    private void Follow()
    {
        if (currentTargetPlayer == null)
        {
            stateEnemy = StateEnemy.idle;
            return;
        }

        isAggression = true;

        if (refTimerSelectTargetCoroutine != null)
        {
            StopCoroutine(refTimerSelectTargetCoroutine);
            refTimerSelectTargetCoroutine = null;
        }

        navMeshTarget = currentTargetPlayer.position;

        distToEnemy = Vector3.Distance(transform.position, currentTargetPlayer.position);    // ��������� �� ����
        distToSpawn = Vector3.Distance(transform.position, startPosition);                   // ��������� �� spawn

        if (currentSpeed != speedRun)
        {
            currentLerpSpeed = speedRun;
            navMeshAgent.speed = currentSpeed;
            enemyAnimator.SetStateAnimator(speed, currentSpeed);
        }

        // if (/*  ���� ��������� �� ������ ������ �������� �����, �� ������������� � ��������� �����*/)
        // ���� ��������� ��� �����
        if (distToEnemy < distanceAttack)
        {
            stateEnemy = StateEnemy.attack;
        }

        // if (/*  ���� ��������� �� ������ ������ ��������� ������� ��� ��������� �� ����� ������ ������ ������������ ���������,
        // �� �������� �������� ������, �� ��������� �������, ������������� � ��������� ������ (�������) */)
        if (distToEnemy > maxDistanceVisibleEnemy || distToSpawn > maxDistanceOffSpawn)
        {
            if (refTimerFollowCoroutine == null)
            {
                refTimerFollowCoroutine = StartCoroutine(TimerFollow());
            }
        }
    }



    /// <summary>
    /// Attack
    /// </summary>
    private void Attack()
    {
        if (refTimerSelectTargetCoroutine != null)
        {
            StopCoroutine(refTimerSelectTargetCoroutine);
            refTimerSelectTargetCoroutine = null;
        }

        if (currentTargetPlayer)                                                                // ���� ��� ���� ��������� � �������
        {
            transform.LookAt(currentTargetPlayer);
            distToEnemy = Vector3.Distance(transform.position, currentTargetPlayer.position);    // ��������� �� ����
        }
        else
        {
            stateEnemy = StateEnemy.patrol;
            return;
        }

        if (currentTargetPlayer && targetGun)
        {
            float offsetY = 1.3f;
            Vector3 tempTargetGun = currentTargetPlayer.position;
            tempTargetGun.y += offsetY;
            targetGun.position = tempTargetGun;
        }

        if (distToEnemy > distanceAttack && refTimerAttackCoroutine == null)
        {
            stateEnemy = StateEnemy.follow;
        }
        else
        {
            if (currentSpeed != speedAttackNavMesh)
            {
                currentSpeed = speedAttackNavMesh;
                navMeshAgent.speed = currentSpeed;
                enemyAnimator.SetStateAnimator(speed, 0);
            }

            if ((distToEnemy < minDistanceAttack && refTimerAttackCoroutine == null) && attWhenPWW == null)
            {

                timeRetreat = maxTimeRetreat;
                vectorDirection = transform.position + transform.forward * (-20);
                stateEnemy = StateEnemy.retreat;
                return;
            }

            if (CheckObstacleToEnemy())        //���� ���� �����������
            {
                if (!isRetreat)
                {
                    timeRetreat = 5.0f;
                    int sign = (Random.Range(0, 2) == 0 ? -1 : 1);
                    vectorDirection = transform.position + transform.right * (sign * 20);                  // ������� ������
                }
                else
                {
                    if (attWhenPWW == null)
                    {
                        attWhenPWW = StartCoroutine(AttackWhenPlayerWithWarrior());

                    }
                    vectorDirection = currentTargetPlayer.position;
                }

                stateEnemy = StateEnemy.retreat;
                return;
            }
            else
            {
                isRetreat = false;
            }



            if (refTimerAttackCoroutine == null)
            {
                navMeshTarget = currentTargetPlayer.position;
                speedAttack = attackEnemy.UpdateAttack(distToEnemy);                          // �����
                refTimerAttackCoroutine = StartCoroutine(TimerAttack(speedAttack));
            }
        }
    }

    private Coroutine attWhenPWW = null;
    private IEnumerator AttackWhenPlayerWithWarrior()
    {
        yield return new WaitForSeconds(2);

        attWhenPWW = null;
    }


    /// <summary>
    /// Death
    /// </summary>
    private void Death()
    {
        if (enemyAnimator.animator.GetLayerWeight(1) != 0)
            enemyAnimator.SetAnimatorLayerWeight(1, 0);

        if (stateEnemy != StateEnemy.death)
        {
            StopAllCoroutines();
            colliderEnemy.enabled = false;
            UiHealthBar.gameObject.SetActive(false);
            stateEnemy = StateEnemy.death;
            navMeshAgent.isStopped = true;
            currentSpeed = 0;
            navMeshAgent.speed = currentSpeed;
            enemyAnimator.SetStateAnimator(speed, currentSpeed);
            enemyAnimator.SetStateAnimator(death);

            if (currentTargetPlayer)
            {
                inDamageModuleEnemy.deach.RemoveListener(DeathCurrentEnemy);  // ������� �� ����������
                currentTargetPlayer = null;
            }

            if (rigObject)
            {
                rigObject.weight = 0;                        //          ��� ���� 
            }
        }

        StartCoroutine(TimerDeath());

    }


    /// <summary>
    /// ����� 
    /// </summary>
    private void Retreat()
    {
        if (currentSpeed != speedRun)
        {
            currentLerpSpeed = speedRun;
            navMeshAgent.speed = currentSpeed;
            enemyAnimator.SetStateAnimator(speed, currentSpeed);
            //navMeshTarget = transform.position + vectorDirection;
            navMeshTarget = vectorDirection;
        }

        if (refTimerRetreatCoroutine == null)
        {
            isRetreat = true;
            refTimerRetreatCoroutine = StartCoroutine(TimerRetreat(timeRetreat));
        }
    }

    private IEnumerator TimerCheckObstacleToEnemy()
    {

        yield return new WaitForSeconds(0.2f);

    }

    private void FollowAfterRetreat()
    {
        if (currentSpeed != speedRun)
        {
            currentLerpSpeed = speedRun;
            navMeshAgent.speed = currentSpeed;
            enemyAnimator.SetStateAnimator(speed, currentSpeed);

        }

        // if (/*  ���� ��������� �� ������ ������ �������� �����, �� ������������� � ��������� �����*/)
        // ���� ��������� ��� �����
        if (distToEnemy < distanceAttack)
        {
            stateEnemy = StateEnemy.attack;
        }

    }


    /// <summary>
    /// ������� ���� �������
    /// </summary>
    /// <param name="maxDistanceVisibleEnemy"></param>
    private IEnumerator CheckAllEnemy(float maxDistanceVisibleEnemy)
    {
        if (PlayerParameters.Instance) 
        {
            PlayerController playerController = PlayerParameters.Instance.GetPlayerController();

            if (Vector3.Distance(playerController.transform.position, transform.position) < maxDistanceVisibleEnemy)
                    if (playerController && playerController.gameIsPlayed)
                        if (refTimerCheckAllEnemyCoroutine == null)
                            refTimerCheckAllEnemyCoroutine = StartCoroutine(TimerCheckAllEnemy(playerController.transform));
        }

        yield return new WaitForSeconds(1);

        checkEnemyCoroutine = null;
    }


    /// <summary>
    /// �������� Follow
    /// </summary>
    /// <returns></returns>
    IEnumerator TimerFollow()
    {
        yield return new WaitForSeconds(timerFollow);

        distToSpawn = Vector3.Distance(transform.position, startPosition);                     // ��������� �� spawn

        if (distToEnemy < distanceAttack || distToSpawn < radiusPatrol)                        // ���� ��������� ��������� ��������� ��� ��������� �� ����� ������ � ������� ��������������
        {
            refTimerFollowCoroutine = StartCoroutine(TimerFollow());                           // ������������� ��������
        }
        else
        {
            if (refTimerPatrolCoroutine == null)
            {
                refTimerPatrolCoroutine = StartCoroutine(TimerPatrol());
            }

            currentAggression = true;
            navMeshTarget = startPosition;
            DeathCurrentEnemy();
            stateEnemy = StateEnemy.patrol;
            refTimerFollowCoroutine = null;
        }
    }

    IEnumerator TimerPatrol()
    {
        yield return new WaitForSeconds(timerPatrol);
        refTimerPatrolCoroutine = null;
    }

    /// <summary>
    /// �������� ������ ����
    /// </summary>
    /// <returns></returns>
    IEnumerator TimerSelectTarget()
    {
        yield return new WaitForSeconds(timerChangeTarget);

        Vector3 tempPosition = SelectPoint();
        navMeshTarget = CheckObstacle(tempPosition);

        currentAggression = false;
        stateEnemy = StateEnemy.patrol;
        refTimerSelectTargetCoroutine = null;
    }

    /// <summary>
    /// ������ ������
    /// </summary>
    /// <returns></returns>
    IEnumerator TimerRetreat(float timerRetreat)
    {
        yield return new WaitForSeconds(timeRetreat);

        stateEnemy = StateEnemy.follow;

        if (distToEnemy < minDistanceAttack) attWhenPWW = StartCoroutine(AttackWhenPlayerWithWarrior());

        refTimerRetreatCoroutine = null;
    }

    /// <summary>
    /// ������ �����
    /// </summary>
    /// <returns></returns>
    IEnumerator TimerAttack(float speedAttack)
    {
        yield return new WaitForSeconds(speedAttack);

        refTimerAttackCoroutine = null;
    }

    /// <summary>
    /// �������� ����� ��������
    /// </summary>
    /// <param name="hitCollider"></param>
    /// <returns></returns>
    IEnumerator TimerCheckAllEnemy(Transform newTarget)
    {
        float delayCheckAllEnemy = 1.0f;
        yield return new WaitForSeconds(delayCheckAllEnemy);

        currentTargetPlayer = newTarget.transform;
        SetCurrentEnemy();
        refTimerCheckAllEnemyCoroutine = null;
    }

    private void Jump()
    {
        bool isGround = true;

        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            enemyAnimator.SetStateAnimator(jump, true);
        }
        else
        {
            enemyAnimator.SetStateAnimator(jump, false);
        }
    }

    /// <summary>
    /// ������� ���������
    /// </summary>
    private void CallForHelp(float maxDistanceVisibleEnemy)
    {
        Vector3 center = transform.position;

        Collider[] hitColliders = Physics.OverlapSphere(center, maxDistanceVisibleEnemy);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<EnemyBase>())
            {
                if (!hitCollider.GetComponent<EnemyBase>().currentTargetPlayer)
                {
                   // Debug.Log(" Help me!!!");
                    hitCollider.GetComponent<EnemyBase>().currentTargetPlayer = currentTargetPlayer;
                }
            }
        }

    }

    private IEnumerator TimerDeath()
    {
        yield return new WaitForSeconds(timerDeath);

        //transform.position = startPosition;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// ��������� ����
    /// </summary>
    private bool CheckInDamage()
    {
        bool result = false;

        if (currentHeal != inDamageModule.GetHeal())
        {
            if (inDamageModule.GetHeal() <= 0)
            {
                if (currentTargetPlayer) {
                    if (questAction != null) {//судя по всему когда бот меняет состояние currentTarget сбрасывается и он не может найти QuestManager
                        QuestManager questManager = currentTargetPlayer.GetComponent<QuestManager>();

                        for (int j = 0; j < questAction.Length; j++)
                            questAction[j].CheckQuestPlayer(questManager);
                    }

                    if (missionQuestBrandBases != null)                                                               //solo
                    {
                        QuestBrandManager questBrandManager = currentTargetPlayer.GetComponent<QuestBrandManager>();  //solo
                        for (int j = 0; j < missionQuestBrandBases.Length; j++)                                       //solo
                            missionQuestBrandBases[j].CheckQuestPlayer(questBrandManager);                            //solo
                    }
                }

                Death();
            }
            else
            {
                Transform objectDamage = inDamageModule.GetTarget();          // ���� 
                PlayerController playerController = null;

                if (objectDamage != null)
                    playerController = objectDamage.GetComponentInParent<PlayerController>();

                if (playerController != null)    // ��� �����
                {
                    if (refTimerPatrolCoroutine == null)
                    {
                        currentTargetPlayer = playerController.transform;
                        SetCurrentEnemy();

                        navMeshTarget = currentTargetPlayer.position;
                        stateEnemy = StateEnemy.follow;
                    }

                    if (needHelp)                 // ����� �� ������
                    {
                        CallForHelp(maxDistanceOffSpawn);
                    }
                }
            }

            currentHeal = inDamageModule.GetHeal();
            result = true;
        }
        return result;
    }

    /// <summary>
    /// ��������� ���� ����
    /// </summary>
    private Vector3 SelectPoint()
    {
        Vector3 result = Vector3.zero;
        float xPosition = Random.Range(-radiusPatrol, radiusPatrol);
        float zPosition = Random.Range(-radiusPatrol, radiusPatrol);
        xPosition += startPosition.x;
        zPosition += startPosition.z;
        result = new Vector3(xPosition, 0.0f, zPosition);
        return result;
    }

    /// <summary>
    /// ��������� ����������� ����� �����
    /// </summary>
    private Vector3 CheckObstacle(Vector3 currentTarget)
    {
        Vector3 result = currentTarget;
        RaycastHit hit;
        float offSet = 0.5f;
        Vector3 startRay = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        startRay.y += offSet;
        Vector3 finishRay = currentTarget - transform.position;
        finishRay.y += offSet;
        //��� ���, ���������� �� ������� ����� ������� � ��������� � ������� ����
        Ray ray = new Ray(startRay, finishRay);
        // Debug.DrawLine(startRay, finishRay, Color.green, 3);

        float maxDist = Vector3.Distance(transform.position, currentTarget);
        // Debug.Log("������� ���");
        if (Physics.Raycast(ray, out hit, maxDist))
        {
            //Debug.DrawLine(startRay, hit.point, Color.red, 3);
            //Debug.Log(" ������ " + hit.collider.name);
            result = hit.point;
        }
        return result;
    }
    private bool CheckObstacleToEnemy()
    {
        bool result = false;

        RaycastHit hit;
        float offSet = 0.5f;
        Vector3 startRay = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        startRay.y += offSet;

        Vector3 finishRay = currentTargetPlayer.position - transform.position;
        finishRay.y += offSet;

        //��� ���, ���������� �� ������� ����� ������� � ��������� � ������� ����
        Ray ray = new Ray(startRay, finishRay);
        //Debug.DrawLine(startRay, finishRay, Color.blue, 1);

        float maxDist = Vector3.Distance(transform.position, currentTargetPlayer.position);

        // Debug.Log("������� ���");
        if (Physics.Raycast(ray, out hit, maxDist))
        {
            //Debug.DrawLine(startRay, hit.point, Color.red, 0.5f);

            if (hit.transform.GetComponent<PlayerController>())
            {
                result = false;
            }
            else
            {
                result = true;
            }

            //Debug.Log(" ������ " + hit.collider.name);
        }

        return result;
    }

    /// <summary>
    /// �������� �� ������ ����
    /// </summary>
    private void SetCurrentEnemy()
    {
        if (currentTargetPlayer)
        {
            if (inDamageModule.GetHeal() > 0)
            {
                inDamageModuleEnemy = currentTargetPlayer.GetComponent<InDamageModule>();
                if (currentTargetPlayer != null) inDamageModuleEnemy.deach.AddListener(DeathCurrentEnemy);
                else
                    Debug.LogError("EnemyDist DeathCurrentEnemy error");
            }
        }
    }

    /// <summary>
    /// ������� ��� ������ ����
    /// </summary>
    private void DeathCurrentEnemy()
    {
        //inDamageModuleEnemy.deach.RemoveListener(DeathCurrentEnemy);
        currentTargetPlayer = null;
    }

    private void UpdateUIEnemy()
    {
        textLevelEnemy.text = levelEnemy.ToString();
        textNameEnemy.text = nameEnemy.ToString();
    }

    /// <summary>
    /// �������� �� �������
    /// </summary>
    public class itemsEnemy
    {
        public int ID;
        public int hance;
        public bool nft;
    }

}
