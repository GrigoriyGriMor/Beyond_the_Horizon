using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class EnemyControllerNear : EnemyBase
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
    public EnemyDeath enemyDeath;
    public AttackEnemy attackEnemy;
    public Collider colliderEnemy;
    public Transform UiHealthBar;

    [SerializeField]
    private Text textNameEnemy;

    [SerializeField]
    private Text textLevelEnemy;

    private Rig rigObject;

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

    private float distToSpawn;

    [HideInInspector]
    public Vector3 currentTarget;

    [HideInInspector]
    public Vector3 navMeshTarget;

    private float distToTarget;

    //[HideInInspector]
    //public Transform currentTargetPlayer;

    [Header(" ������ ���� ������� ����� ������ ")]
    [SerializeField]
    private float timerDeath = 10.0f;

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

    //[HideInInspector]
    public StateEnemy stateEnemy;

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

    private InDamageModule inDamageModuleEnemy;

    private ControlQuestBase[] questAction = new ControlQuestBase[1];

    private Coroutine refCoroutineUpdate;

    private void Start()
    {
        questAction = GetComponents<ControlQuestBase>();
        if (questAction == null)
            questAction = GetComponentsInChildren<ControlQuestBase>();
        // if (refCoroutineUpdate == null) refCoroutineUpdate = StartCoroutine(CustomUpdate());


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

        if (enemyAnimator != null) enemyAnimator = GetComponent<EnemyAnimator>();
        if (navMeshAgent != null) navMeshAgent = GetComponent<NavMeshAgent>();
        if (inDamageModule != null) inDamageModule = GetComponent<InDamageModule>();
        if (attackEnemy != null) attackEnemy = GetComponent<AttackEnemy>();
        if (rigObject != null) rigObject = GetComponentInChildren<Rig>();

        UiHealthBar.gameObject.SetActive(true);
        inDamageModule.ReloadParam();
        currentHeal = inDamageModule.GetHeal();
        colliderEnemy.enabled = true;
        startPosition = transform.position;
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

            case StateEnemy.death:

                break;

            default:
                break;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, currentLerpSpeed, Time.fixedDeltaTime * 5.0f);

        navMeshAgent.SetDestination(navMeshTarget);

        // isAggression = aggression;

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
                checkEnemyCoroutine = StartCoroutine(CheckAllEnemy(maxDistanceVisibleEnemy));   // ���� �����������
        }

        // ���� ���� ��������� �� ��� �������������
        if (currentTargetPlayer)
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

        // ���� ��������� �� ������ ������ �������� �����, �� ������������� � ��������� �����*/)
        // ���� ��������� ��� �����
        if (distToEnemy < distanceAttack)
        {
            stateEnemy = StateEnemy.attack;
        }

        // (/*  ���� ��������� �� ������ ������ ��������� ������� ��� ��������� �� ����� ������ ������ ������������ ���������,
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

            if (refTimerAttackCoroutine == null)
            {
                navMeshTarget = currentTargetPlayer.position;

                speedAttack = attackEnemy.UpdateAttack(distToEnemy);                          // �����
                refTimerAttackCoroutine = StartCoroutine(TimerAttack(speedAttack));
            }
        }

    }


    /// <summary>
    /// Death
    /// </summary>
    private void Death()
    {
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
                //inDamageModuleEnemy.deach.RemoveListener(DeathCurrentEnemy);  // ������� �� ����������
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
            //isAggression = currentAggression;
            navMeshTarget = startPosition;
            DeathCurrentEnemy();
            //currentTargetPlayer = null;       //
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


    private IEnumerator TimerDeath()
    {
        yield return new WaitForSeconds(timerDeath);

        //  transform.position = startPosition;
        gameObject.SetActive(false);
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
                if (currentTargetPlayer)
                {
                    if (questAction != null)
                    {
                        QuestManager questManager = currentTargetPlayer.GetComponent<QuestManager>();

                        for (int j = 0; j < questAction.Length; j++)
                            questAction[j].CheckQuestPlayer(questManager);
                    }

                }

                Death();
            }
            else
            {
                Transform objectDamage = inDamageModule.GetTarget();          // ���� 
                PlayerController playerController = null;

                if (objectDamage != null) playerController = objectDamage.GetComponentInParent<PlayerController>();

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


    /// <summary>
    /// �������� �� �������
    /// </summary>
    public class itemsEnemy
    {
        public int ID;
        public int hance;
        public bool nft;
    }

    private void SetCurrentEnemy()
    {
        if (currentTargetPlayer)
        {
            if (inDamageModule.GetHeal() > 0)
            {
                inDamageModuleEnemy = currentTargetPlayer.GetComponent<InDamageModule>();
                inDamageModuleEnemy.deach.AddListener(DeathCurrentEnemy);
            }
        }
    }

    private void DeathCurrentEnemy()
    {
       // inDamageModuleEnemy.deach.RemoveListener(DeathCurrentEnemy);
        currentTargetPlayer = null;
    }

    private void UpdateUIEnemy()
    {
        textLevelEnemy.text = levelEnemy.ToString();
        textNameEnemy.text = nameEnemy.ToString();
    }

}