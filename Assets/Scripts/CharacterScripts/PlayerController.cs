using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    [Header("Сервер/Клиент")]
    public SupportClass.gameState UDPStatus = SupportClass.gameState.client;

    public Text playerName;

    private SupportClass.PlayerStateMode mode = SupportClass.PlayerStateMode.Idle;
    private WeaponController UsedWeapon;

    [Header("refferenses")]
    [SerializeField] private GameObject visual;
    [SerializeField] private Transform spineBone;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator playerAnim;

    [Header("Moduls")]
    [SerializeField] private MoveModule moveController;
    [SerializeField] private LookModule lookController;
    [SerializeField] private JumpModule jumpModules;
    [SerializeField] private WeaponModule weaponModule;
    [SerializeField] private AttackModule attackModule;
    [SerializeField] private InDamageModule inDamageModule;
    [SerializeField] private SkillsModule skillsModule;

    [SerializeField] private InventoryController inventoryModule;

    [SerializeField] private ChatSystem chatSystem;
    [SerializeField] private QuickMenuSystem quickMenuSystem;

    [Header("Brand Shop")]
    [SerializeField] private BrandStoreSystem brandStoreSystem;

    [Header("Info Use Object")]
    [SerializeField] private GameObject useObjectPanel;
    [SerializeField] private Text textWhatDo;
    [SerializeField] private Text textObjectName;


    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;

    [Header("Other settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded = false;
    //[SerializeField] private int sprintLayerIndex = 2;

    [HideInInspector] public bool gameIsPlayed = false;
    private Vector2 moveAxis;
    private CapsuleCollider _collider;
    private bool isJumping = false;
    private bool isCrouching = false;
    private bool isFlipping = false;
    private bool isSprint = false;

    [Header("Riggin Detals")]
    [SerializeField] private MultiAimConstraint[] targetDetals = new MultiAimConstraint[3];
    [SerializeField] private TwoBoneIKConstraint leftHand;
    [SerializeField] private float maxLeftWeight = 0.7f;

    [Range(1, 20)]
    [SerializeField] private float weightUpSpeed = 5;

    [Header("CameraView Setting")]
    [SerializeField] private float sprintCameraView = 80;
    [SerializeField] private float aimingCameraView = 25;
    private float cameraView;

    public ushort playerID;

    [HideInInspector] public InputPlayerManager inputModule;

    [Header("Respawn Particle")]
    [SerializeField] private ParticleSystem respawnParticle;

    public SupportClass.PlayerStateMode GetMode() {
        return mode;
    }

    private void Awake() {
        if (_rb == null) Debug.LogError("Rigidbody _rb component is NULL");
        if (playerAnim == null) Debug.LogError("Animator playernim component is NULL");
        _collider = GetComponent<CapsuleCollider>();

        inputModule = GetComponent<InputPlayerManager>();

        if (moveController.enabled) moveController.Init(UDPStatus, visual, _rb, playerAnim, spineBone, mainCamera);
        if (lookController.enabled) lookController.Init(UDPStatus, visual, _rb, playerAnim, spineBone, mainCamera);
        if (jumpModules.enabled) jumpModules.Init(UDPStatus, visual, _rb, playerAnim, spineBone, mainCamera, this);
        if (attackModule.enabled) attackModule.Init(UDPStatus, visual, _rb, playerAnim, spineBone, mainCamera);
        if (weaponModule != null && weaponModule.enabled) weaponModule.Init(UDPStatus, visual, _rb, playerAnim, spineBone, mainCamera, this);
        if (inDamageModule.enabled) inDamageModule.Init(UDPStatus, visual, _rb, playerAnim);
        if (inventoryModule != null && inventoryModule.enabled) inventoryModule.Init(UDPStatus, this, weaponModule);
        if (skillsModule != null && skillsModule.enabled) skillsModule.Init(UDPStatus, visual, _rb, playerAnim, spineBone, mainCamera, this);
        if (GetComponent<ChecknteractbleObj>()) GetComponent<ChecknteractbleObj>().Init(mainCamera);

        cameraView = mainCamera.fieldOfView;

        gameIsPlayed = false;

        SetNewFixMode(SupportClass.PlayerStateMode.Idle);

        if (UDPStatus != SupportClass.gameState.client) StartCoroutine(StartGame());

        inDamageModule.deach.AddListener(() => StartCoroutine(Death()));
    }

    private void Start() {
        //if (PlayerParameters.Instance && (UDPStatus == SupportClass.gameState.client || UDPStatus == SupportClass.gameState.test)) PlayerParameters.Instance.SetPlayerController(this);
        StartCoroutine(SetPlayer());
    }

    private IEnumerator SetPlayer() {
        while (!PlayerParameters.Instance) {
            yield return new WaitForFixedUpdate();
        }

        if (PlayerParameters.Instance && (UDPStatus == SupportClass.gameState.client || UDPStatus == SupportClass.gameState.test)) PlayerParameters.Instance.SetPlayerController(this);
    }

    public void StartThisGame() {
        StartCoroutine(StartGame());
    }

    private Vector3 currentPlayerPosition;
    private Vector3 currentPlayerRotation;
    private Vector3 currentPlayerVisualRotation;

    public void SetServerParamSinc(Vector3 playerPos, Vector3 playerRotate, Vector3 playerVisualRotate,
        Vector3 cameraRotate, Vector3 aimRotate, Vector3 velocity, float healPoint,
        float shieldPoint, int usedGunN, byte _mode) {

        currentPlayerPosition = playerPos;
        currentPlayerRotation = playerRotate;
        currentPlayerVisualRotation = playerVisualRotate;

        lookController.SetCameraRotate(cameraRotate);
        lookController.SetAimRotate(aimRotate);

        _rb.velocity = velocity;

        inDamageModule.SetHeal(healPoint);
        inDamageModule.SetShield(shieldPoint);

        /*  switch (_mode) {
              case 0:
                  if (mode != SupportClass.PlayerStateMode.Idle)
                      SetNewFixMode(SupportClass.PlayerStateMode.Idle);
                  break;
              case 1:
                  if (mode != SupportClass.PlayerStateMode.Combat)
                      SetNewFixMode(SupportClass.PlayerStateMode.Combat);
                  break;
              case 2:
                  if (mode != SupportClass.PlayerStateMode.Sprint)
                      SetNewFixMode(SupportClass.PlayerStateMode.Sprint);
                  break;
          }*/

        if (!oneMoment && weaponModule.GetUsedGun() != usedGunN) {
            UseWeapon(usedGunN);
            oneMoment = true;
        }
    }
    bool oneMoment = false;

    public void SetServerAnimatorParametrs(float[] animLayerWeight, AnimatorParamData[] animParam) {

        for (int i = 0; i < animLayerWeight.Length; i++)
            playerAnim.SetLayerWeight(i, animLayerWeight[i]);

        for (int j = 0; j < animParam.Length; j++) {
            string name = playerAnim.GetParameter(animParam[j].indexParam).name;
            switch (animParam[j].type) {
                case (0):
                    playerAnim.SetFloat(name, animParam[j].defaultFloat);
                    break;
                case (1):
                    if (animParam[j].defaultBool == 1)
                        playerAnim.SetBool(name, true);
                    else
                        playerAnim.SetBool(name, false);
                    break;
                case (2):
                    //   playerAnim.SetTrigger(name);
                    break;
                case (3):
                    playerAnim.SetInteger(name, animParam[j].defaultInt);
                    break;
            }
        }
    }

    [HideInInspector] public ushort body_type = 2;

    public void InitPlayer(string _playerName, int _id = -1, string session = "") {
        if (_id != -1) playerID = (ushort)_id;
        if (playerName != null) playerName.text = _playerName;

        if (chatSystem != null) chatSystem.Init(_playerName, _id, session);

        if (brandStoreSystem != null) brandStoreSystem.Init(_id, session);
    }

    public void InitPlayer(string _playerName, ushort _body_type = 0) {
        body_type = _body_type;
        if (playerName != null) playerName.text = _playerName;
    }

    public void InitPlayer(string _playerName) {
        if (playerName != null) playerName.text = _playerName;
    }

    public PlayerCoreData GetPlayerData()
    {
        PlayerCoreData newData = new PlayerCoreData();

        newData.playerPosX = transform.position.x;
        newData.playerPosY = transform.position.y;
        newData.playerPosZ = transform.position.z;

        newData.playerRotateX = transform.eulerAngles.x;
        newData.playerRotateY = transform.eulerAngles.y;
        newData.playerRotateZ = transform.eulerAngles.z;

        newData.playerVisualRotateX = playerAnim.transform.eulerAngles.x;
        newData.playerVisualRotateY = playerAnim.transform.eulerAngles.y;
        newData.playerVisualRotateZ = playerAnim.transform.eulerAngles.z;

        Vector3 camRotate = lookController.GetCameraRotate();
        newData.cameraRotateX = camRotate.x;
        newData.cameraRotateY = camRotate.y;
        newData.cameraRotateZ = camRotate.z;

        Vector3 aimRotate = lookController.GetAimRotate();
        newData.aimRotateX = aimRotate.x;
        newData.aimRotateY = aimRotate.y;
        newData.aimRotateZ = aimRotate.z;

        newData.velocityX = _rb.velocity.x;
        newData.velocityY = _rb.velocity.y;
        newData.velocityZ = _rb.velocity.z;

        newData.healPoint = inDamageModule.GetHeal();
        newData.shieldPoint = inDamageModule.GetShield();

        newData.usedGunN = weaponModule.GetUsedGunNumber();

        newData.inputData = inputModule.GetCurrentButtonState();

        switch (mode) {
            case SupportClass.PlayerStateMode.Idle:
                newData.playerState = 0;
                break;
            case SupportClass.PlayerStateMode.Combat:
                newData.playerState = 1;
                break;
            case SupportClass.PlayerStateMode.Sprint:
                newData.playerState = 2;
                break;
        }

        {/*  float[] layerW = new float[3];
          for (int i = 0; i < layerW.Length; i++)
              layerW[i] = playerAnim.GetLayerWeight(i);

          newData.animLayerWeight = layerW;

          AnimatorParamData[] apd = new AnimatorParamData[20];
          for (int i = 0; i < playerAnim.parameterCount; i++) {
              apd[i].indexParam = (ushort)i;

              switch (playerAnim.parameters[i].type) {
                  case (AnimatorControllerParameterType.Float):
                      apd[i].type = 0;
                      apd[i].defaultFloat = playerAnim.GetFloat(playerAnim.parameters[i].name);
                      break;
                  case (AnimatorControllerParameterType.Bool):
                      apd[i].type = 1;
                      if (playerAnim.GetBool(playerAnim.parameters[i].name))
                          apd[i].defaultBool = 1;
                      else
                          apd[i].defaultBool = 0;
                      break;
                  case (AnimatorControllerParameterType.Trigger):
                      apd[i].type = 2;
                      break;
                  case (AnimatorControllerParameterType.Int):
                      apd[i].type = 3;
                      apd[i].defaultInt = playerAnim.GetInteger(playerAnim.parameters[i].name);
                      break;
              }
          }
        newData.animParam = apd;*/
        }

        return newData;
    }


    //for test
    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1);
      //  if (CoursorController.Instance) CoursorController.Instance.UI_Object_Off();
        gameIsPlayed = true;
    }

    //====================================================Отправка===================================================================
    public void SetNewTrigger(string triggerName) {
        for (int i = 0; i < playerAnim.parameterCount; i++) {
            if (playerAnim.parameters[i].name == triggerName)
                triggerActive.Add(i);
        }
    }
    private List<int> triggerActive = new List<int>();
    //==============================================================================================================================

    private void Update() {
        if (!gameIsPlayed) return;
        //================================================Прием====================================================================
        /*for (int i = 0; i < playerAnim.parameterCount; i++) {
            if (playerAnim.parameters[i].type == AnimatorControllerParameterType.Trigger) {
                for (int j = 0; j < triggerActive.Count; j++) {
                    if (i == triggerActive[j])
                        triggerActive.RemoveAt(j);
                        Debug.LogError("Trigger " + playerAnim.parameters[i].name);
                }
            }
        }*/
        //==============================================================================================================================


        if (UDPStatus == SupportClass.gameState.client || UDPStatus == SupportClass.gameState.clone) {
            transform.position = Vector3.Lerp(transform.position, currentPlayerPosition, 15 * Time.deltaTime);

            Quaternion tr = Quaternion.Euler(currentPlayerRotation);
            transform.rotation = Quaternion.Lerp(transform.rotation, tr, 15 * Time.deltaTime);
            tr = Quaternion.Euler(currentPlayerVisualRotation);
            visual.transform.rotation = Quaternion.Lerp(visual.transform.rotation, tr, 15 * Time.deltaTime);
        }

        playerAnim.SetFloat("VelocityY", _rb.velocity.y);

        Collider[] col = Physics.OverlapSphere(new Vector3(transform.position.x, _collider.bounds.min.y, transform.position.z), 0.2f);
        if (col.Length > 1 && !isGrounded)
        {
            isGrounded = true;
            playerAnim.SetBool("isGrounded", isGrounded);
        }
        else
            if (col.Length <= 1 && isGrounded)
        {
            isGrounded = false;
            playerAnim.SetBool("isGrounded", isGrounded);
        }

        if (hightControl)
        {
            if (_rb.velocity.y < -15 && mode == SupportClass.PlayerStateMode.Combat)
                SetNewFixMode(SupportClass.PlayerStateMode.Idle);

            if (_rb.velocity.y < -15 && isGrounded)
            {
                hightControl = false;
                inDamageModule.InDamageAfterFall(Mathf.Abs(_rb.velocity.y));
                StartCoroutine(StayAfterFall());
            }
            else {
                if (_rb.velocity.y < -10 && isGrounded)
                {
                    hightControl = false;
                    StartCoroutine(FlipAfterJump());
                    playerAnim.SetTrigger("FlipAfterJump");
                }
                else
                {
                    if (isGrounded)
                        hightControl = false;
                }
            }
        }

        if (!isGrounded)
            moveAxis = Vector2.zero;
    }

    private IEnumerator StayAfterFall() {
        isFlipping = true;
        SetNewBoneWeight(0);
        _rb.velocity = new Vector3(0, 0, 0);

        yield return new WaitForSeconds(0.8f);
        isFlipping = false;
    }

    private IEnumerator FlipAfterJump()
    {
        isFlipping = true;
        SetNewBoneWeight(0);
        _rb.velocity = new Vector3(0, 0, 0);
        yield return new WaitForFixedUpdate();

        _rb.AddForce(playerAnim.transform.forward * 7.5f, ForceMode.VelocityChange);

        yield return new WaitForSeconds(0.8f);

        if (mode == SupportClass.PlayerStateMode.Combat) SetNewBoneWeight(1);
        isFlipping = false;
    }

    #region _Move
    public void MoveCharacter(Vector2 moveVector) {
        if (chatIsUse) return;

        if (gameIsPlayed && (isGrounded && !isFlipping)) {
            moveAxis = moveVector;
            moveController.MoveAxis(moveAxis, mode, isCrouching);

            if (moveAxis == Vector2.zero && isSprint)
                SprintControl(false);
        }
    }
    #endregion

    #region _Jump
    public void Jump()
    {
        if (chatIsUse) return;

        if (isJumping || !isGrounded || isCrouching) return;

        StartCoroutine(JumpingReloadTime());
    }

    private IEnumerator JumpingReloadTime()
    {
        isJumping = true;

        jumpModules.Jumping(moveAxis, mode);

        yield return new WaitForSeconds(0.5f);
        isJumping = false;
    }
    #endregion

    #region _Crouch
    public void CrouchControl(bool crouch = false)
    {
        isCrouching = crouch;

        if (isCrouching != playerAnim.GetBool("Crouch"))
            playerAnim.SetBool("Crouch", isCrouching);

        if (isCrouching)
        {
            _collider.height = _collider.height / 1.25f;
            _collider.center = new Vector3(_collider.center.x, _collider.center.y / 1.25f, _collider.center.z);
        }
        else
        {
            _collider.height = _collider.height * 1.25f;
            _collider.center = new Vector3(_collider.center.x, _collider.center.y * 1.25f, _collider.center.z);
        }
    }
    #endregion

    #region _Sprint
    private SupportClass.PlayerStateMode lastSprintMode;
    public void SprintControl(bool sprint = false)
    {
        isSprint = sprint;

        if (isSprint && mode != SupportClass.PlayerStateMode.Sprint)
        {
            lastSprintMode = mode;
            SetNewFixMode(SupportClass.PlayerStateMode.Sprint);
            SetNewCameraView(sprintCameraView);
        }
        else
            if (!isSprint && mode != lastSprintMode)
        {
            SetNewFixMode(lastSprintMode);
            SetNewCameraView(cameraView);
        }
    }
    #endregion

    #region _CameraLook
    public void CameraRotate(Vector2 mouseAxis, bool AIM = true)
    {
        if (mode == SupportClass.PlayerStateMode.Combat && AIM)
        {
            if (isGrounded && (moveAxis.x > 0.05f || moveAxis.x < -0.05f || moveAxis.y < -0.05f || moveAxis.y > 0.05f))
                lookController.CameraAIMRotate(mouseAxis, true);
            else
                lookController.CameraAIMRotate(mouseAxis, false);
        }
        else
        {
            if ((moveAxis.x > 0.05f || moveAxis.x < -0.05f || moveAxis.y < -0.05f || moveAxis.y > 0.05f) && isGrounded)
                lookController.CameraRotate(mouseAxis, true);
            else
                lookController.CameraRotate(mouseAxis, false);
        }
    }
    #endregion

    #region Swap Weapon
    public void UseWeapon(int wNumber)
    {
        if (chatIsUse) return;

        if (weaponModule != null) weaponModule.GetWeapon(wNumber);
    }
    #endregion

    #region Attack
    public void Attack(bool attack)
    {
        if (mode == SupportClass.PlayerStateMode.Combat)
        {
            attackModule.InAttack(UsedWeapon);
        }
        else
        {
            if (UsedWeapon != null)
                SetNewFixMode(SupportClass.PlayerStateMode.Combat);
        }
    }
    #endregion

    #region Aiming
    public void AimActivate(bool aiming)
    {
        if (UsedWeapon == null) return;

        if (aiming)
        {
            SetNewFixMode(SupportClass.PlayerStateMode.Combat);
            SetNewAnimLayerW(1);
            playerAnim.SetBool("Aiming", aiming);
            SetNewCameraView(aimingCameraView);
            lookController.Aim(true);
        }
        else
        {
            playerAnim.SetBool("Aiming", aiming);
            if (!weaponModule.reload) SetNewAnimLayerW(0);
            SetNewCameraView(cameraView);
            lookController.Aim(false);
        }
    }

    Coroutine cameraViewCoroutine;
    public void SetNewCameraView(float newValue)
    {
        if (cameraViewCoroutine != null) StopCoroutine(cameraViewCoroutine);
        cameraViewCoroutine = StartCoroutine(SNCV(newValue));
    }

    private IEnumerator SNCV(float newValue)
    {
        while (Mathf.Abs(newValue - mainCamera.fieldOfView) > 0.1f)
        {
            mainCamera.fieldOfView = Mathf.LerpUnclamped(mainCamera.fieldOfView, newValue, 10 * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }

        mainCamera.fieldOfView = newValue;

        cameraViewCoroutine = null;
    }

    Coroutine setNewLW;
    public void SetNewAnimLayerW(float targetLayerWeight)
    {
        if (mode == SupportClass.PlayerStateMode.Combat && playerAnim.GetBool("Aiming")) return;

        if (setNewLW != null) StopCoroutine(setNewLW);
        setNewLW = StartCoroutine(SetNewAnimLayerWight(targetLayerWeight));
    }

    private IEnumerator SetNewAnimLayerWight(float targetLayerWeight)
    {
        while (Mathf.Abs(targetLayerWeight - playerAnim.GetLayerWeight(2)) > 0.1f)
        {
            playerAnim.SetLayerWeight(2, Mathf.LerpUnclamped(playerAnim.GetLayerWeight(2), targetLayerWeight, 10 * Time.deltaTime));
            yield return new WaitForFixedUpdate();
        }

        playerAnim.SetLayerWeight(2, targetLayerWeight);

        setNewLW = null;
    }

    #endregion

    #region ReloadGun
    public void ReloadGun()
    {
        if (UsedWeapon != null)
        {
            weaponModule.ReloadGun();
            playerAnim.SetBool("Aiming", false);
        }
    }
    #endregion

    #region Chat
    bool chatIsUse = false;
    public void UseChat() {
        chatIsUse = chatSystem.UseChat();
    }
    #endregion

    #region QuickSystem
    public void UseQuickSystem() {
        if (!chatIsUse) quickMenuSystem.UseQuickMenu();
    }

    public void UseMap() {
        if (!chatIsUse) quickMenuSystem.UseMap();
    }

    public void UseMission() {
        if (!chatIsUse) quickMenuSystem.UseMission();
    }
    public void UseInventory() {
        if (!chatIsUse) quickMenuSystem.UseInventory();
    }

    public void UseStore() {
        if (!chatIsUse) quickMenuSystem.UseStore();
    }

    public void UseIslandStore() {
        if (!chatIsUse) quickMenuSystem.UseIslandStore();
    }
    #endregion

    public void DialogIsActive() {
        playerAnim.SetFloat("Speed", 0);
    }

    #region Skills
    public void UseSkill(int i) {
        skillsModule.UseSkill(i);
    }
    #endregion

    private IEnumerator Death() {
        gameIsPlayed = false;

        AimActivate(false);
        SetNewFixMode(SupportClass.PlayerStateMode.Idle);

        yield return new WaitForSeconds(5);

        inDamageModule.ReloadParam();
        playerAnim.SetTrigger("RepeatPlayer");

        if (UDPStatus == SupportClass.gameState.server) {
            if (SpawnerPlayer.Instance) {
                Vector3 point = SpawnerPlayer.Instance.GetSpawnPos();
                transform.position = new Vector3(point.x, point.y + 2, point.z);
            }
            else
                transform.position = new Vector3(Random.Range(-5, 5), 10, Random.Range(-5, 5));

            gameIsPlayed = true;
        }
        else
            StartCoroutine(RespawnCorutine());
    }

    private IEnumerator RespawnCorutine() {
        if (SpawnerPlayer.Instance) {
            Vector3 point = SpawnerPlayer.Instance.GetSpawnPos();
            transform.position = new Vector3(point.x, point.y, point.z);
        }

        yield return new WaitForSeconds(0.15f);
        if (respawnParticle != null) respawnParticle.Play();
        gameIsPlayed = true;
    }

    #region When object need use
    private InteractbleObjectController currentInteractiveItem = null;

    public void UseItem() {
        if (currentInteractiveItem != null) {
            switch (currentInteractiveItem.itemType) {
                case SupportClass.interactiveItemType.NPC:
                    if (currentInteractiveItem.GetComponent<NpcController>()) currentInteractiveItem.GetComponent<NpcController>().StartDialog(this);
                    break;
                case SupportClass.interactiveItemType.Car:
                    CarBase car = currentInteractiveItem.GetComponentInParent<CarBase>();
                    if (car != null) {
                        if (mode == SupportClass.PlayerStateMode.Combat) SetNewFixMode(SupportClass.PlayerStateMode.Idle);
                        car.StartCar(this);
                        lookController.SetInCar();
                        car.exitCar.AddListener(() => {
                            lookController.ExitCar();
                            car.exitCar.RemoveAllListeners();
                        });
                    }
                    break;
                case SupportClass.interactiveItemType.Other:
                    if (inventoryModule != null) inventoryModule.SetNewItem(currentInteractiveItem.GetComponent<ItemAtSceneController>().GetItemInfo());
                    break;
                case SupportClass.interactiveItemType.Billboard:
                    if (brandStoreSystem != null) {
                        BillboardInGameInfo data = currentInteractiveItem.GetComponent<BilboardController>().GetInfo();
                        if (data.brand_name != "" || data.good_id != "")
                            brandStoreSystem.OpenWindowsWithSelectbleParam(currentInteractiveItem.GetComponent<BilboardController>().GetInfo());
                    }
                    break;
                default:
                    break;
            }
        }
            //currentInteractiveItem.UseActivity(this);      
    }

    public void CanUseObject(string _nameItem, string _eventItem, InteractbleObjectController itemController) {
        if (useObjectPanel == null) return;

        useObjectPanel.gameObject.SetActive(true);

        textWhatDo.text = _eventItem;
        textObjectName.text = _nameItem;

        currentInteractiveItem = itemController;
    }

    public void OutUseObject() {
        if (useObjectPanel == null) return;

        useObjectPanel.gameObject.SetActive(false);

        textWhatDo.text = "";
        textObjectName.text = "";

        currentInteractiveItem = null;
    }
    #endregion

    [SerializeField] private float fightModeExitTime = 30;
    [SerializeField] private Animator aimAnim;
    public void SetNewFixMode(SupportClass.PlayerStateMode newMode)
    {
        switch (newMode)
        {
            case SupportClass.PlayerStateMode.Idle :
                mode = newMode;
                if (weightCoroutine != null) StopCoroutine(weightCoroutine);
                weightCoroutine = StartCoroutine(WeightCorection(0));
                if (aimAnim && aimAnim.GetBool("CombatAIM")) aimAnim.SetBool("CombatAIM", false);

                //need correct
                playerAnim.SetLayerWeight(1, 0);
                break;
            case SupportClass.PlayerStateMode.Combat :
                mode = newMode;
                
                if (UsedWeapon != null)
                {
                    if (weightCoroutine != null) StopCoroutine(weightCoroutine);
                    weightCoroutine = StartCoroutine(WeightCorection(maxLeftWeight));
                }

                if (aimAnim && !aimAnim.GetBool("CombatAIM")) aimAnim.SetBool("CombatAIM", true);
                //need correct
                playerAnim.SetLayerWeight(1, 1);
                break;
            case SupportClass.PlayerStateMode.Sprint:
                mode = newMode;
                if (weightCoroutine != null) StopCoroutine(weightCoroutine);
                weightCoroutine = StartCoroutine(WeightCorection(0));
                if (aimAnim && aimAnim.GetBool("CombatAIM")) aimAnim.SetBool("CombatAIM", false);
                break;
            default:
                mode = newMode;
                break;
        }
    }

    //Отвечает за вес настройки IK для привязки направления кости к позиции таргета 
    public void SetNewBoneWeight(float targetW)
    {
        if (weightCoroutine != null || mode != SupportClass.PlayerStateMode.Combat) return;

        weightCoroutine = StartCoroutine(WeightCorection(targetW));
    }

    private Coroutine weightCoroutine;
    private IEnumerator WeightCorection(float targetWeight)
    {
        if (targetDetals.Length == 0) yield break;

        while (Mathf.Abs(targetDetals[0].weight - targetWeight) > 0.1f)
        {
            for (int i = 0; i < targetDetals.Length; i++)
                targetDetals[i].weight = Mathf.LerpUnclamped(targetDetals[i].weight, targetWeight, 5 * Time.deltaTime);

            leftHand.weight = Mathf.LerpUnclamped(leftHand.weight, targetWeight, 5 * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }

        for (int i = 0; i < targetDetals.Length; i++)
            targetDetals[i].weight = targetWeight;

        leftHand.weight = targetWeight;

        weightCoroutine = null;
    }

    //смена мода combat|base mode
    public void SwitchMode()
    {
        switch (mode)
        {
            case SupportClass.PlayerStateMode.Idle:
                if (UsedWeapon != null) SetNewFixMode(SupportClass.PlayerStateMode.Combat);
                break;
            case SupportClass.PlayerStateMode.Combat:
                SetNewFixMode(SupportClass.PlayerStateMode.Idle);
                break;
            default:
                SetNewFixMode(SupportClass.PlayerStateMode.Idle);
                break;
        }
    }

    private bool hightControl = false;
    public void HightControll()
    {
        hightControl = true;
    }

    public void SetUsedWeapon(WeaponController controller)
    { 
        UsedWeapon = controller;

        if (UsedWeapon == null) SetNewFixMode(SupportClass.PlayerStateMode.Idle);
    }
}

[System.Serializable]
public class WeaponInfo
{
    public ItemBaseParametrs ItemInfo;
    [HideInInspector] public GameObject weaponGameObject;
    public Transform weapon_Pos;
}