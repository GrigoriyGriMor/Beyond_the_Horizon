using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum StateNPC
{
    idleNPC,
    walkNPC,
    runNPC,
    rotateNPC,
    dialogNPC,
    walkSleepNPC,
    campNPC,
    dialogCampNPC
}

public class NPCStatic : NpcController
{
    // состояние NPC

    [Header("Разворот на Player")]
    [SerializeField]
    private bool isRotation = true;

    private Transform thisTransform;

    // скорость поворота
    private float speedRotate = 5.0f;

    // время поворота
    private float timerRotate = 1.0f;

    private Coroutine refTimerRotate;

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
        //navMeshAgent = GetComponent<NavMeshAgent>();
        thisTransform = GetComponent<Transform>();
        stateNPC = StateNPC.idleNPC;
        //if (UIDialogWindow)
        //{
        //    UIDialogWindow.gameObject.SetActive(false);
        //}

    }

    private void SetState()
    {
        switch (stateNPC)
        {
            case StateNPC.idleNPC:
                break;


            case StateNPC.walkNPC:
                break;

            case StateNPC.runNPC:
                break;


            case StateNPC.rotateNPC:
                RotateNPC();
                break;

            case StateNPC.dialogNPC:

                break;


        }
    }

    private void RotateNPC()
    {
        if (isRotation)
        {
            SmoothRotate(currentPlayer);
        }

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
        yield return new WaitForSeconds(timerRotate);
        refTimerRotate = null;
        stateNPC = StateNPC.dialogNPC;
    }

    public override void StartDialog(PlayerController playerController)
    {
        // Debug.Log(" Start Dialog " + name + " " + playerController.playerID);
        if (interactbleObjectController.enabled)
        {

            if (stateNPC != StateNPC.dialogNPC)
            {
                currentPlayer = playerController.transform;
                stateNPC = StateNPC.rotateNPC;
            }

            dialogBase.StartDialog(stateNPC); ////
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

    private void UpdateUINPC()
    {
        refTextNPC.text = GetName().ToString();
    }

}
