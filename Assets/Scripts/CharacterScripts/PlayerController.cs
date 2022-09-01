using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
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

    [SerializeField] private QuickMenuSystem quickMenuSystem;

    [Header("Info Use Object")]
    [SerializeField] private GameObject useObjectPanel;
    [SerializeField] private Text textWhatDo;
    [SerializeField] private Text textObjectName;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;

    [Header("Other settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded = false;

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

        if (moveController.enabled) moveController.Init(visual, _rb, playerAnim, spineBone, mainCamera);
        if (lookController.enabled) lookController.Init(visual, _rb, playerAnim, spineBone, mainCamera);
        if (jumpModules.enabled) jumpModules.Init(visual, _rb, playerAnim, spineBone, mainCamera, this);
        if (attackModule.enabled) attackModule.Init(visual, _rb, playerAnim, spineBone, mainCamera);
        if (weaponModule != null && weaponModule.enabled) weaponModule.Init(visual, _rb, playerAnim, spineBone, mainCamera, this);
        if (inDamageModule.enabled) inDamageModule.Init(visual, _rb, playerAnim);
        if (inventoryModule != null && inventoryModule.enabled) inventoryModule.Init(this, weaponModule);
        if (skillsModule != null && skillsModule.enabled) skillsModule.Init(visual, _rb, playerAnim, spineBone, mainCamera, this);
        if (GetComponent<ChecknteractbleObj>()) GetComponent<ChecknteractbleObj>().Init(mainCamera);

        cameraView = mainCamera.fieldOfView;

        gameIsPlayed = false;

        SetNewFixMode(SupportClass.PlayerStateMode.Idle);

        StartCoroutine(StartGame());

        inDamageModule.deach.AddListener(() => StartCoroutine(Death()));
    }

    private void Start() {
        StartCoroutine(SetPlayer());
    }

    private IEnumerator SetPlayer() {
        while (!PlayerParameters.Instance) {
            yield return new WaitForFixedUpdate();
        }

        PlayerParameters.Instance.SetPlayerController(this);
    }

    public void StartThisGame() {
        StartCoroutine(StartGame());
    }


    [HideInInspector] public ushort body_type = 2;

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1);
        gameIsPlayed = true;
    }

    private void Update() {
        if (!gameIsPlayed) return;

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

    #region QuickSystem
    public void UseQuickSystem() {
        quickMenuSystem.UseQuickMenu();
    }

    public void UseMap() {
        quickMenuSystem.UseMap();
    }

    public void UseMission() {
        quickMenuSystem.UseMission();
    }
    public void UseInventory() {
        quickMenuSystem.UseInventory();
    }

    public void UseStore() {
        quickMenuSystem.UseStore();
    }

    public void UseIslandStore() {
        quickMenuSystem.UseIslandStore();
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