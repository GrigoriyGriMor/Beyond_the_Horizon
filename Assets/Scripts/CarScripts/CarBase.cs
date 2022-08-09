using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum StateCar
{
    idleCar,
    startInCar,
    inCar,
    move,
    startOutCar,
    outCar
}

public class CarBase : AbstractIO
{
    [Header("Trigger Name")]
    [SerializeField] private string rotateCar;
    [SerializeField] private string crashCar = "CrashCar";
    [SerializeField] private string sitDown = "SitDown";
    [SerializeField] private string getUp = "GetUp";
    [SerializeField]
    private float timerOutCar = 0.1f;
    [SerializeField]
    private StateCar stateCar;
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private Animator animatorDoor;
    [SerializeField]
    private GameObject activateObject;
    [SerializeField]
    private CharacterBase characterBase;
    private InputPlayerManager inputPlayerManager;
    private Rigidbody rigidbodyPlayer;
    private Collider colliderPlayer;
    private Transform transformPlayer;
    private Rigidbody rigidbodyCar;
    private Transform thisTransform;
    private Animator animatorPlayer;
    private QuestManager questManager;
    KeyCode useObject;
    private Vector2 moveAxis;
    [Header("Control Buttons")]
    public KeyCode brake = KeyCode.Space;
    [Header("Repair Buttons")]
    [SerializeField]
    private KeyCode repairCar = KeyCode.R;
    [SerializeField]
    private CarController carController;
    [SerializeField]
    private Transform centerMass;
    [Header(" “очка дл€ посадки в машину")]
    [SerializeField]
    private Transform pointStartSeatDown;
    [Header(" “очка посадки в машине")]
    [SerializeField]
    private Transform pointSeatDown;
    [Header(" UI Elements")]
    [SerializeField]
    private Transform miniMapIco;
    [SerializeField]
    private Transform speedPanel;
    [SerializeField]
    private Transform interactionPanel;
    [SerializeField]
    private Text textHotKey;
    [SerializeField]
    private Text textSpeedCar;
    private bool isUpside
    {
        get
        {
            return (thisTransform.rotation.z > -0.27 && thisTransform.rotation.z < 0.27);
        }
    }
    private float speedCar;
    [HideInInspector] public UnityEvent exitCar = new UnityEvent();

    //[HideInInspector]
    public UnityEvent sitDownCar = new UnityEvent();

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        TryGetComponent(out thisTransform);
        TryGetComponent(out rigidbodyCar);
        if (centerMass) rigidbodyCar.centerOfMass = centerMass.localPosition;
        speedPanel.gameObject.SetActive(false);
        miniMapIco.gameObject.SetActive(true);
        interactionPanel.gameObject.SetActive(false);
        SetActivateObject(activateObject, false);
        textHotKey.text = repairCar.ToString();
        stateCar = StateCar.idleCar;
    }

    void Start()
    {

    }

    private void Update()
    {
        if (playerController && stateCar == StateCar.move)
            CheckButtonExit();
    }

    void FixedUpdate()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        switch (stateCar)
        {
            case StateCar.idleCar:
                break;

            case StateCar.startInCar:
                StartInCar();
                break;

            case StateCar.inCar:
                StateInCar();
                break;

            case StateCar.move:
                StateMove();
                break;

            case StateCar.startOutCar:
                StartOutCar();
                break;

            case StateCar.outCar:
                StateOutCar();
                break;

        }
    }

    private void StartInCar()
    {
        EnableCar();
        return;
    }

    private void StateInCar()
    {
        transformPlayer.rotation = thisTransform.rotation;
        animatorPlayer.transform.rotation = transformPlayer.rotation;
        transformPlayer.position = pointSeatDown.position;
        transformPlayer.SetParent(gameObject.transform);

        if (animatorPlayer)
        {
            animatorPlayer.SetTrigger(sitDown);
        }
        if (animatorDoor)
        {
            animatorDoor.SetTrigger(sitDown);
        }
        miniMapIco.gameObject.SetActive(false);
        speedPanel.gameObject.SetActive(true);

        stateCar = StateCar.move;

        return;

    }

    private void StartOutCar()
    {
        if (animatorPlayer)
        {
            animatorPlayer.SetTrigger(getUp);
        }

        if (animatorDoor)
        {
            animatorDoor.SetTrigger(getUp);
        }

        exitCar.Invoke();
        stateCar = StateCar.outCar;
        return;
    }

    private void StateOutCar()
    {
        DisableCar();
    }

    private void StateMove()
    {
        CheckBrake();
        MoveCar(moveAxis);
    }

    public void StartCar(PlayerController playerController)
    {
        if (stateCar == StateCar.idleCar)
        {
            this.playerController = playerController;
            inputPlayerManager = playerController.GetComponent<InputPlayerManager>();
            useObject = inputPlayerManager.uesObject;
            inputPlayerManager.carBase = this;
            stateCar = StateCar.startInCar;
            return;
        }
    }

    public void SetAxis(Vector2 moveAxis)
    {
        this.moveAxis = moveAxis;
    }

    public void MoveCar(Vector2 moveAxis)
    {
        carController.motor = moveAxis.x;
        carController.steering = moveAxis.y;
        UpdateUiCar();
    }

    private void CheckButtonExit()
    {
        if (Input.GetKeyDown(useObject))
        {
            stateCar = StateCar.startOutCar;
        }
    }

    private void CheckBrake()
    {
        carController.brakeCar = Input.GetKey(brake);
    }

    private void UpdateUiCar()
    {
        float currentSpeedCar = rigidbodyCar.velocity.magnitude;
        currentSpeedCar = currentSpeedCar * 4.0f;
        speedCar = (int)Mathf.Lerp(currentSpeedCar, speedCar, Time.fixedDeltaTime * 2.0f);
        textSpeedCar.text = speedCar.ToString();
    }

    private void EnableCar()
    {
        playerController.TryGetComponent(out rigidbodyPlayer);
        playerController.TryGetComponent(out colliderPlayer);
        playerController.TryGetComponent(out questManager);
        animatorPlayer = playerController.GetComponent<MoveModule>().playerAnim;
        transformPlayer = playerController.transform;
        colliderPlayer.enabled = false;
        rigidbodyPlayer.useGravity = false;
        rigidbodyPlayer.isKinematic = true;
        SetActivateObject(activateObject, true);
        transformPlayer.position = pointStartSeatDown.position;
        stateCar = StateCar.inCar;
        sitDownCar.Invoke();
    }

    public void DisableCar()
    {
        transformPlayer.SetParent(null);
        transformPlayer.position = pointStartSeatDown.position;
        transformPlayer.rotation = Quaternion.Euler(0, transformPlayer.eulerAngles.y, 0);
        StartCoroutine(TimerOutCar());
        SetActivateObject(activateObject, false);
        this.playerController = null;
        useObject = KeyCode.None;
        moveAxis.x = 0;
        moveAxis.y = 0;
        MoveCar(moveAxis);
        speedPanel.gameObject.SetActive(false);
        miniMapIco.gameObject.SetActive(true);
        stateCar = StateCar.idleCar;
    }

    private void SetActivateObject(GameObject gameObject, bool isActive)
    {
       if (gameObject) gameObject.SetActive(isActive);
    }

    private void CrashCar()
    {
        animatorPlayer.SetTrigger(crashCar);
    }

    private bool IsAnimationPlaying(Animator animator, string animationName)
    {
        // берем информацию о состо€нии
        var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // смотрим, есть ли в нем им€ какой-то анимации, то возвращаем true
        if (animatorStateInfo.IsName(animationName))
            return true;

        return false;
    }

    private void CheckTiltAngle()
    {
        if (!isUpside)
        {
            interactionPanel.gameObject.SetActive(true);
            if (Input.GetKeyDown(repairCar))
            {
                SetNormAngleCar();
            }
        }
        else
        {
            interactionPanel.gameObject.SetActive(false);
        }
    }

    private void SetNormAngleCar()
    {
        thisTransform.rotation = Quaternion.Euler(thisTransform.eulerAngles.x, thisTransform.eulerAngles.y, 0);
        thisTransform.position = new Vector3(thisTransform.position.x, thisTransform.position.y + 2.0f, thisTransform.position.z);
    }

    public QuestManager GetQuestManager()
    {
        return questManager;
    }

    public QuestBrandManager GetQuestBrandManager()
    {
        return playerController.GetComponent<QuestBrandManager>(); ;
    }


    private IEnumerator TimerOutCar()
    {
        yield return new WaitForSeconds(timerOutCar);
        colliderPlayer.enabled = true;
        rigidbodyPlayer.useGravity = true;
        rigidbodyPlayer.isKinematic = false;
        rigidbodyPlayer = null;
        transformPlayer = null;
        colliderPlayer = null;
        animatorPlayer = null;
        questManager = null;
        inputPlayerManager.carBase = null;
        inputPlayerManager = null;
    }
}
