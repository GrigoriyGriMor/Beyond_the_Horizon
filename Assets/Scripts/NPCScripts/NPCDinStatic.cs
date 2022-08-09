using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class NPCDinStatic : NpcController
{
    [SerializeField]
    private DialogBase dialogBaseRandom;

    private DialogBase tempDialogBase;

    [SerializeField]
    private MonoBehaviour[] moduleOff;

    [SerializeField]
    private MonoBehaviour[] moduleOn;

    [Header("Trigger Name")]
    public string speed;

    [SerializeField]
    private string campNPC = "CampNPC";

    /// <summary>
    /// Скорость Walk
    /// </summary>
    [Header("Скорость Walk")]
    [SerializeField]
    private float speedWalk = 1.0f;

    /// <summary>
    /// Скорость Run
    /// </summary>
    [Header("Скорость Run")]
    [SerializeField]
    private float speedRun = 3.0f;

    /// <summary>
    /// Таймер сна
    /// </summary>
    //[Header("Таймер Сна")]
    //[SerializeField]
    //public float timerSleep = 10.0f;

    [Header("Ночь")]
    //[HideInInspector]
    public bool isNight;

    /// <summary>
    /// Точка cна
    /// </summary>
    [Header("Точка сна")]
    [SerializeField]
    private Transform pointSleep;

    /// <summary>
    /// Точка Stay
    /// </summary>
    [Header("Точка Stay")]
    [SerializeField]
    private Transform pointStay;

    /// <summary>
    /// Растояние на которое нужно приблизится к точке
    /// </summary>
    [Header("Растояние на которое нужно приблизится к точке ")]
    [SerializeField]
    private float distanceToChangeTarget = 1.0f;

    private float distToTarget;

    // состояние NPC
    //[SerializeField]
    //private StateNPC stateNPC;

    // пред состояние NPC
    private StateNPC lastStateNPC;

    // трансформ этого обьекта
    private Transform thisTransform;

    // текущая цель
    private Transform currentTarget;

    // скорость поворота
    private float speedRotate = 5.0f;

    // время поворота
    private float timerRotate = 1.0f;

    private bool talk = false;

    // ссылка коротина время поворота
    private Coroutine refTimerRotate;

    // текущая скорость
    private float currentSpeed;

    // текущая скорость Lerp
    private float currentLerpSpeed;

    private void OnEnable()
    {
        Init();
    }

    void Start()
    {
        UpdateUINPC();
    }

    private void Update()
    {
        SetState();
    }

    private void Init()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        thisTransform = GetComponent<Transform>();
        stateNPC = StateNPC.idleNPC;
        ModuleOnOff(moduleOn, moduleOff);
    }

    private void SetState()
    {
        switch (stateNPC)
        {
            case StateNPC.idleNPC:
                IdleNPC();
                break;

            case StateNPC.walkNPC:
                WalkNPC();
                break;

            case StateNPC.rotateNPC:
                RotateNPC();
                break;

            case StateNPC.dialogNPC:
                DialogNPC();
                break;

            case StateNPC.walkSleepNPC:
                WalkSleepNPC();
                break;

            case StateNPC.campNPC:
                CampNPC();
                break;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, currentLerpSpeed, Time.fixedDeltaTime * 5.0f);
    }

    private void IdleNPC()
    {
        currentLerpSpeed = 0;
        navMeshAgent.speed = currentSpeed;
        if (animator)
        {
            animator.SetFloat(speed, currentSpeed);
        }
        if (isNight)
        {
            stateNPC = StateNPC.walkSleepNPC;
            ModuleOnOff(moduleOff, moduleOn);
            tempDialogBase = dialogBase;
            dialogBase = dialogBaseRandom;
        }
        else
        {
            if (!pointStay) return;
            distToTarget = Vector3.Distance(pointStay.position, thisTransform.position);
            if (distToTarget > distanceToChangeTarget)  // если подошел к цели
            {
                stateNPC = StateNPC.walkNPC;
            }
        }
    }

    private void RotateNPC()
    {
        SmoothRotate(currentPlayer);
        if (refTimerRotate == null)
        {
            refTimerRotate = StartCoroutine(TimerRotate());
        }
    }

    private void WalkSleepNPC()
    {
        if (!pointSleep) return;
        navMeshAgent.SetDestination(pointSleep.position);
        distToTarget = Vector3.Distance(pointSleep.position, thisTransform.position);
        if (distToTarget < distanceToChangeTarget)  // если подошел к цели
        {
            stateNPC = StateNPC.campNPC;
        }
        else
        {
            //currentLerpSpeed = speedWalk;
            currentSpeed = speedWalk;
            navMeshAgent.speed = currentSpeed;
            if (animator)
            {
                animator.SetFloat(speed, currentSpeed);
            }
        }
    }

    private void WalkNPC()
    {
        if (!pointStay) return;
        navMeshAgent.SetDestination(pointStay.position);
        distToTarget = Vector3.Distance(pointStay.position, thisTransform.position);
        if (distToTarget < distanceToChangeTarget)  // если подошел к цели
        {
            stateNPC = StateNPC.idleNPC;
            ModuleOnOff(moduleOn, moduleOff);
            dialogBase = tempDialogBase;
        }
        else
        {
            //currentLerpSpeed = speedWalk;
            currentSpeed = speedWalk;
            navMeshAgent.speed = currentSpeed;

            if (animator)
            {
                animator.SetFloat(speed, currentSpeed);
            }
        }
    }

    private void SmoothRotate(Transform target)
    {
        Vector3 tempRotation = new Vector3(thisTransform.localEulerAngles.x, thisTransform.localEulerAngles.y, thisTransform.localEulerAngles.z);
        Vector3 direction = target.transform.position - thisTransform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, rotation, speedRotate * Time.fixedDeltaTime);
        thisTransform.localEulerAngles = new Vector3(tempRotation.x, thisTransform.localEulerAngles.y, tempRotation.z);
    }

    private IEnumerator TimerRotate()
    {
        yield return new WaitForSeconds(timerRotate);
        refTimerRotate = null;
        stateNPC = StateNPC.dialogNPC;
    }

    private void DialogNPC()
    {
        //currentLerpSpeed = 0;
        navMeshAgent.speed = 0;
        //if (animator)
        //{
        //    //animator.SetBool(dialogWalk, true);
        //    //animator.SetFloat(speed, currentSpeed);
        //}
    }

    private void CampNPC()
    {
        currentSpeed = 0;
        navMeshAgent.speed = currentSpeed;
        if (animator)
        {
            animator.SetFloat(speed, currentSpeed);
        }
        if (!isNight)
        {
            stateNPC = StateNPC.walkNPC;
        }
    }

    public override void StartDialog(PlayerController playerController)
    {
        if (interactbleObjectController.enabled)
        {
            if (stateNPC == StateNPC.idleNPC)
            {
                //Debug.Log(" Start Dialog " + name + " " + playerController.playerID);
                if (stateNPC != StateNPC.dialogNPC)
                {
                    currentPlayer = playerController.transform;
                    stateNPC = StateNPC.rotateNPC;
                }
                dialogBase.StartDialog(stateNPC);  ////
            }
            else
            {
                dialogBase.StartDialog(stateNPC);
            }
        }
    }

    public override void EndDialog()
    {
        dialogBase.EndDialog();
        currentPlayer = null;
        stateNPC = StateNPC.idleNPC;
        if (refTimerRotate != null)
        {
            StopCoroutine(refTimerRotate);
            refTimerRotate = null;
        }
    }

    private void ModuleOnOff(MonoBehaviour[] moduleOff, MonoBehaviour[] moduleOn)
    {
        SetModule(moduleOff, false);
        SetModule(moduleOn, true);
    }

    private void SetModule(MonoBehaviour[] module, bool active)
    {
        for (int index = 0; index < module.Length; index++)
        {
            module[index].enabled = active;
        }
    }

    public void GotoHome()
    {
        if (talk) StartCoroutine(GoHome());
        // else
        // идет домой    
    }

    private IEnumerator GoHome()
    {
        yield return new WaitForSeconds(2);
        if (talk) StartCoroutine(GoHome());
        // else
        // идет домой   
    }


    private void UpdateUINPC()
    {
        refTextNPC.text = GetName().ToString();
    }
}
