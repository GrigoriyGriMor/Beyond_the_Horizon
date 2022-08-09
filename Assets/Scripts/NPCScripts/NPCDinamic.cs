using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NPCDinamic : NpcController
{
    // ������ ������
    [Header("������ ������")]
    [SerializeField]
    private Transform campFire;

    [Header("Trigger Name")]
    public string speed = "Speed";

    //[SerializeField]
    //private string dialogWalk = "DialogWalk";

    [SerializeField]
    private string campNPCin = "capm_in";

    [SerializeField]
    private string campNPCout = "capm_out";

    /// <summary>
    /// �������� Walk
    /// </summary>
    [Header("�������� Walk")]
    [SerializeField]
    private float speedWalk = 1.0f;

    /// <summary>
    /// �������� Run
    /// </summary>
    [Header("�������� Run")]
    [SerializeField]
    private float speedRun = 3.0f;

    /// <summary>
    /// ������
    /// </summary>
    [Header("������")]
    [SerializeField]
    private bool isCamp;

    /// <summary>
    /// ������ �������������
    /// </summary>
    [Header("������ �������������")]
    [SerializeField]
    public float timerWake = 100.0f;

    /// <summary>
    /// ������ ������
    /// </summary>
    [Header("������ ������")]
    [SerializeField]
    public float timerCamp = 10.0f;

    [SerializeField]
    private float distNight = 10.0f;

    /// <summary>
    /// ����� ��������
    /// </summary>
    [Header("����� ��������")]
    [SerializeField]
    private Transform[] pointsRoute;

    /// <summary>
    /// ��������� �� ������� ����� ����������� � �����
    /// </summary>
    [Header("��������� �� ������� ����� ����������� � ����� ")]
    [SerializeField]
    private float distanceToChangeTarget = 1.0f;
    private float distToTarget;

    /// <summary>
    /// vector �����������
    /// </summary>
    private Vector3 vectorDirection;

    // ������ ������� ����� ���������� NPC
    //[SerializeField]
    private int indexPointsRoute = 0;

    //// ��������� NPC
    //[SerializeField]
    //private StateNPC stateNPC;

    // ���� ��������� NPC
    private StateNPC lastStateNPC;

    // ��������� ����� �������
    private Transform thisTransform;

    // ������� ����
    private Transform currentTarget;

    // �������� ��������
    private float speedRotate = 5.0f;

    // ����� ��������
    private float timerRotate = 1.0f;

    // ������ �������� ����� ��������
    private Coroutine refTimerRotate;
    private Coroutine refTimerWake;
    private Coroutine refTimerCamp;

    // ������� ��������
    private float currentSpeed;

    // ������� �������� Lerp
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
        campFire.gameObject.SetActive(false);
        stateNPC = StateNPC.idleNPC;
        ActivedCamping();
    }

    /// <summary>
    /// ��� �������
    /// </summary>
    private void ActivedCamping()
    {
        if (isCamp)
        {
            if (refTimerWake == null)
            {
                refTimerWake = StartCoroutine(TimerWake());
            }
        }
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

            case StateNPC.runNPC:
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

            case StateNPC.dialogCampNPC:
                DialogCampNPC();
                break;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, currentLerpSpeed, Time.fixedDeltaTime * 5.0f);

    }

    private void IdleNPC()
    {
        campFire.gameObject.SetActive(false);
        currentLerpSpeed = 0;
        navMeshAgent.speed = currentSpeed;

        if (animator)
        {
            animator.SetFloat(speed, currentSpeed);
        }

        ChangeTargetPoint();
    }

    private void WalkNPC()
    {
        if (!currentTarget) return;
        navMeshAgent.SetDestination(currentTarget.position);
        distToTarget = Vector3.Distance(currentTarget.position, thisTransform.position);

        if (distToTarget < distanceToChangeTarget)  // ���� ������� � ����
        {
            stateNPC = StateNPC.idleNPC;
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

    private void RotateNPC()
    {
        currentSpeed = 0;
        navMeshAgent.speed = currentSpeed;

        if (animator)
        {
            animator.SetFloat(speed, currentSpeed);
        }

        SmoothRotate(currentPlayer);

        if (refTimerRotate == null)
        {
            refTimerRotate = StartCoroutine(TimerRotate());
        }

    }

    private void SmoothRotate(Transform target)
    {
        //Vector3 direction = target.transform.position - thisTransform.position;
        //Quaternion rotation = Quaternion.LookRotation(direction);
        //thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, rotation, speedRotate * Time.fixedDeltaTime);

        Vector3 tempRotation = new Vector3(thisTransform.localEulerAngles.x, thisTransform.localEulerAngles.y, thisTransform.localEulerAngles.z);
        Vector3 direction = target.transform.position - thisTransform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, rotation, speedRotate * Time.fixedDeltaTime);
        thisTransform.localEulerAngles = new Vector3(tempRotation.x, thisTransform.localEulerAngles.y, tempRotation.z);
    }

    private IEnumerator TimerRotate()
    {
        dialogBase.StartDialog(stateNPC);
        yield return new WaitForSeconds(timerRotate);
        refTimerRotate = null;
        stateNPC = StateNPC.dialogNPC;
        //dialogBase.StartDialog(stateNPC);


    }

    private void ChangeTargetPoint()
    {
        currentTarget = pointsRoute[indexPointsRoute];

        //Debug.Log(" indexPoints " + indexPointsRoute);

        stateNPC = StateNPC.walkNPC;

        indexPointsRoute++;

        if (indexPointsRoute == pointsRoute.Length)
        {
            indexPointsRoute = 0;
        }
    }

    private void DialogNPC()
    {
        currentSpeed = 0;
        navMeshAgent.speed = currentSpeed;
        if (animator)
        {
            animator.SetFloat(speed, currentSpeed);
        }
    }

    private void DialogCampNPC()
    {

    }

    private void CampNPC()
    {
        currentSpeed = 0;
        navMeshAgent.speed = currentSpeed;

        if (animator)
        {
            animator.SetFloat(speed, currentSpeed);
        }
    }

    private void WalkSleepNPC()
    {
        navMeshAgent.SetDestination(vectorDirection);

        distToTarget = Vector3.Distance(vectorDirection, thisTransform.position);

        if (distToTarget < distanceToChangeTarget)  // ���� ������� � ����
        {
            PutCampFire();
            stateNPC = StateNPC.campNPC;
            animator.SetTrigger(campNPCin);

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

    private void PutCampFire()
    {
        Vector3 currentTarget = campFire.position + campFire.up * 3;

        RaycastHit hit;
        Vector3 startRay = currentTarget;
        Vector3 finishRay = campFire.position - currentTarget;

        Ray ray = new Ray(startRay, finishRay);
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.DrawLine(startRay, hit.point, Color.red, 3.0f);
            campFire.position = hit.point;
        }
        campFire.gameObject.SetActive(true);
    }

    private IEnumerator TimerWake()
    {
        //Debug.Log("Start Day");

        yield return new WaitForSeconds(timerWake);

        refTimerWake = null;

        if (SetVectorDirCamp())          // ������ � �������
        {
            refTimerCamp = StartCoroutine(TimerCamp());
        }
        else
        {
            if (refTimerWake == null)
            {
                refTimerWake = StartCoroutine(TimerWake());
            }
        }
    }

    private IEnumerator TimerCamp()
    {
        //Debug.Log("Start Night");

        stateNPC = StateNPC.walkSleepNPC;
        yield return new WaitForSeconds(timerCamp);

        refTimerCamp = null;

        if (refTimerWake == null)
        {
            refTimerWake = StartCoroutine(TimerWake());
        }

        stateNPC = StateNPC.idleNPC;
        animator.SetTrigger(campNPCout);
    }

    private bool SetVectorDirCamp()
    {
        int sign = (Random.Range(0, 2) == 0 ? -1 : 1);
        vectorDirection = transform.position + transform.right * (sign * distNight);                  // ������� �� ������

        if (CheckObstacle(vectorDirection))
        {
            sign *= -1;                                                                                   // ���� ��� ����������� 
            vectorDirection = transform.position + transform.right * (sign * distNight);                  // ���� � ���������������
        }
        else
        {
            return true;
        }

        if (CheckObstacle(vectorDirection))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool CheckObstacle(Vector3 currentTarget)
    {
        bool result = false;
        RaycastHit hit;
        float offSet = 0.5f;
        Vector3 startRay = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        startRay.y += offSet;
        Vector3 finishRay = currentTarget - transform.position;
        finishRay.y += offSet;
        // ���, ���������� �� ������� ����� ������� � ��������� � ������� ����
        Ray ray = new Ray(startRay, finishRay);
        //Debug.DrawLine(startRay, finishRay, Color.green, 3);
        float maxDist = Vector3.Distance(transform.position, currentTarget) + 5.0f;
        // Debug.Log("������� ���");
        if (Physics.Raycast(ray, out hit, maxDist))
        {
            //Debug.DrawLine(startRay, hit.point, Color.red, 3);
            result = true;
        }
        return result;
    }

    public override void StartDialog(PlayerController playerController)
    {
        if (interactbleObjectController.enabled && dialogBase.enabled)
        {
            if (stateNPC != StateNPC.dialogNPC)
            {
                if (stateNPC != StateNPC.dialogCampNPC)
                {
                    if (stateNPC != StateNPC.rotateNPC)
                    {
                        lastStateNPC = stateNPC;
                        currentPlayer = playerController.transform;

                        if (stateNPC == StateNPC.campNPC)
                        {
                            stateNPC = StateNPC.dialogCampNPC;

                            dialogBase.StartDialog(stateNPC);
                        }
                        else
                        {
                            stateNPC = StateNPC.rotateNPC;
                        }

                        if (isCamp)
                        {
                            if (refTimerWake != null)
                            {
                                StopCoroutine(refTimerWake);
                                refTimerWake = null;
                            }

                            if (refTimerCamp != null)
                            {
                                StopCoroutine(refTimerCamp);
                                refTimerCamp = null;
                            }
                        }
                    }
                }
            }
        }
    }

    public override void EndDialog()
    {
        dialogBase.EndDialog();
        currentPlayer = null;

        ActivedCamping();

        if (lastStateNPC == StateNPC.campNPC)
        {
            stateNPC = StateNPC.idleNPC;
            animator.SetTrigger(campNPCout);

            //refTimerWake = null;
            //refTimerCamp = StartCoroutine(TimerCamp());
        }
        else
        {
            stateNPC = lastStateNPC;
        }

        if (refTimerRotate != null)
        {
            StopCoroutine(refTimerRotate);
            refTimerRotate = null;
        }

    }

    private void UpdateUINPC()
    {
        refTextNPC.text = GetName().ToString();
    }


}
