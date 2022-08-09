using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using JU_INPUT_SYSTEM;

[AddComponentMenu("JU TPS/Gameplay/Third Person System/Third Person Controller")]
[RequireComponent(typeof(Rigidbody), typeof(AudioSource), typeof(CapsuleCollider))]
public class ThirdPersonController : MonoBehaviour
{
    [HideInInspector] GameManagerAndUI Game;
    [HideInInspector] public Animator anim;
    [HideInInspector] Rigidbody rb;
    [HideInInspector] Camera MyCamera;
    [HideInInspector] CamPivotController MyPivotCamera;
    [HideInInspector] AdvancedRagdollController advancedRagdollController;
    [HideInInspector] JUFootPlacement FootPlacerIK;

    [Header("Movement Settings")]
    public float Speed = 3;
    public float RotationSpeed = 3;
    public float JumpForce = 2.5f;

    public float AirInfluenceControll = 0.5f;
    public bool CurvedMovement = true;

    public bool BodyInclination = true;

    public bool RootMotion = false;
    public float RootMotionSpeed = 1;
    public bool RootMotionRotation = true;
    public Vector3 RootMotionDeltaPosition;

    [HideInInspector] public Transform DirectionTransform;

    float VelocityMultiplier;
    float VerticalY;
    float HorizontalX;
    [HideInInspector] float BodyRotation;
    Vector3 DesiredDirection;
    [HideInInspector] Vector3 EulerRotation;
    [HideInInspector] float LastX, LastY, LastVelMult;
    [HideInInspector] Quaternion DesiredCameraRotation;

    [Header("Death Events")]
    public bool RagdollWhenDie;
    public bool SlowmotionWhenDie;

    [Header("Ground Check Settings")]
    public LayerMask WhatIsGround;
    public float GroundCheckRadious = 0.1f;
    public float GroundCheckHeighOfsset = 0f;
    public float GroundCheckSize = 1;

    [Header("Step Settings")]
    public bool EnableStepCorrection = true;
    public LayerMask StepCorrectionMask;
    public float FootstepHeight = 0.55f;
    public float ForwardStepOffset = 0.3f;
    public float StepHeight = 0.05f;

    [Header("Footstep Settings")]
    public float MaxFootstepTime = 0.45f;
    [HideInInspector] float currentFootstepTime;
    [HideInInspector] AudioSource AudioS;
    //public AudioClip[] FootstepAudioClips;
    public List<FootstepAudios> FootstepAudioClips = new List<FootstepAudios>(4);

    [Header("Punch Attacks Settings")]
    public bool EnabledPunchAttacks;
    [HideInInspector] float currenttimetodisable_isattacking = 0;
    public GameObject PunchCollisionParticle;

    [Header("Weapons Settings")]
    public LayerMask CrosshairHitMask;
    public GameObject PivotWeapons;
    public WeaponAimRotationCenter WeaponPositions;
    public Weapon[] Weapons;
    [HideInInspector] public Weapon WeaponInUse;
    private int WeaponID = -1; // [-1] = Hand
    [HideInInspector] float IsArmedWeight;
    [HideInInspector] RaycastHit CrosshairHit;


    //Hand IK Targets
    [HideInInspector] public Transform IKPositionRightHand;
    [HideInInspector] public Transform IKPositionLeftHand;

    [HideInInspector] public Transform HumanoidSpine;
    float WeightIK;

    [Header("Pick Up Weapons")]
    private LayerMask WeaponLayer;

    [HideInInspector]
    public Vehicle VehicleInArea;

    [Header("States")]
    public float Health = 100;
    public bool IsDead;
    public bool DisableAllMove;
    public bool CanMove;
    public bool IsMoving;
    public bool IsRunning;
    public bool IsCrouched;
    public bool IsJumping;
    public bool IsGrounded = true;
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsArmed;
    [HideInInspector] public bool IsAiming;
    [HideInInspector] public bool ToPickupWeapon;
    [HideInInspector] public bool IsRolling;
    [HideInInspector] public bool IsDriving;
    public bool ToEnterVehicle;
    [HideInInspector] public bool Shot;
    [HideInInspector] public bool CanShoot;
    [HideInInspector] public bool IsReloading;
    [HideInInspector] public bool WallInFront;
    [HideInInspector] public bool InverseKinematics = true;


    void Start()
    {
        Game = FindObjectOfType<GameManagerAndUI>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        AudioS = GetComponent<AudioSource>();

        WeaponPositions = PivotWeapons.GetComponent<WeaponAimRotationCenter>();
        WeaponID = -1;
        WeaponLayer = LayerMask.GetMask("Default", "Terrain", "Vehicle Mesh Collider", "Walls", "Weapon");
        WeaponInUse = null;

        MyCamera = Camera.allCameras[0];
        MyPivotCamera = FindObjectOfType<CamPivotController>();


        if (HumanoidSpine == null)
        {
            if (anim.GetBoneTransform(HumanBodyBones.UpperChest) == null)
            {
                HumanoidSpine = anim.GetBoneTransform(HumanBodyBones.Chest);
            }
            else
            {
                HumanoidSpine = anim.GetBoneTransform(HumanBodyBones.UpperChest);
            }
        }

        if (TryGetComponent(out AdvancedRagdollController adv))
        {
            advancedRagdollController = adv;
        }
        if (TryGetComponent(out JUFootPlacement footplacer))
        {
            FootPlacerIK = footplacer;
        }
        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].gameObject.layer = 0;
            Weapons[i].GetComponent<Collider>().enabled = false;
            Weapons[i].gameObject.SetActive(false);
        }

        // Generate Transform Targets

        DirectionTransform = new GameObject("Direction Transform").transform;
        DirectionTransform.position = transform.position;
        DirectionTransform.parent = transform;
        DirectionTransform.hideFlags = HideFlags.HideInHierarchy;
        DirectionTransform.gameObject.hideFlags = HideFlags.HideAndDontSave;

        IKPositionLeftHand = new GameObject("Left Hand IK Target").transform;
        IKPositionLeftHand.position = transform.position;
        IKPositionLeftHand.parent = transform;
        IKPositionLeftHand.hideFlags = HideFlags.HideInHierarchy;
        IKPositionLeftHand.gameObject.hideFlags = HideFlags.HideAndDontSave;

        IKPositionRightHand = new GameObject("Right Hand IK Target").transform;
        IKPositionRightHand.position = transform.position;
        IKPositionRightHand.parent = transform;
        IKPositionRightHand.hideFlags = HideFlags.HideInHierarchy;
        IKPositionRightHand.gameObject.hideFlags = HideFlags.HideAndDontSave;
    }
    void FixedUpdate()
    {
        if (IsDead == true)
            return;
        Movement();
        StepCorrect();
    }
    void Update()
    {
        if (IsDead == true)
        {
            Health = 0;
            CanMove = false;
            IsRunning = false;
            IsCrouched = false;
            IsJumping = false;
            IsGrounded = false;
            IsArmed = false;
            IsRolling = false;
            IsDriving = false;
            Shot = false;
            WallInFront = false;
            InverseKinematics = false;
            anim.SetLayerWeight(1, 0);

            var rot = transform.rotation;
            rot.x = 0;
            rot.z = 0;
            transform.rotation = rot;

            GetComponent<Collider>().isTrigger = false;
            rb.useGravity = true;
            rb.velocity = transform.up * rb.velocity.y;

            return;
        }


        if (IsDead == false && DisableAllMove == false)
        {
            Inputs();
            Rotate();
            FootstepsTimer();
            WeaponControl();
            DriveControl();
            PickUpWeapons();
            WeaponIKControl();
            FootPlacementIKController();
        }
        if (DisableAllMove)
        {
            anim.SetLayerWeight(1, 0f);
            WeightIK = 0;
        }
    }

    #region Weapon Switch
    private void _NextWeapon()
    {
        SwitchWeapons(SwitchDirection.Forward);
    }
    private void _PreviousWeapon()
    {
        SwitchWeapons(SwitchDirection.Backward);
    }

    private enum SwitchDirection { Forward, Backward }
    void SwitchWeapons(SwitchDirection Direction)
    {
        IsAiming = false;
        if (IsReloading == true) return;

        PivotWeapons.transform.localEulerAngles = new Vector3(PivotWeapons.transform.localEulerAngles.x + 30, 0, 0);
        WeightIK = 0.4f;

        if (Direction == SwitchDirection.Forward)
        {
            WeaponID++;
            for (int i = 0; i < Weapons.Length; i++)
            {
                if (i != WeaponID)
                {
                    Weapons[i].gameObject.SetActive(false);
                }
                else
                {
                    if (Weapons[i].Unlocked == true)
                    {
                        Weapons[i].gameObject.SetActive(true);
                        WeaponInUse = Weapons[i];
                    }
                    else
                    {
                        _NextWeapon();
                        WeaponInUse = null;
                    }
                }
            }
        }
        if (Direction == SwitchDirection.Backward)
        {
            WeaponID--;

            for (int i = Weapons.Length - 1; i > -1; i--)
            {
                if (i != WeaponID)
                {
                    Weapons[i].gameObject.SetActive(false);
                }
                else
                {
                    if (Weapons[i].Unlocked == true)
                    {
                        Weapons[i].gameObject.SetActive(true);
                        WeaponInUse = Weapons[i];
                    }
                    else
                    {
                        WeaponInUse = null;
                        _PreviousWeapon();
                    }
                }
            }
        }
        if (IsArmed)
        {
            IsAttacking = false;
            currenttimetodisable_isattacking = 0;
        }
        if (WeaponID == -2)
        {
            WeaponID = Weapons.Length;
        }
        if (WeaponID == Weapons.Length)
        {
            WeaponID = -1;
            WeaponInUse = null;
            IsArmed = false;
        }

        //NO WEAPONS
        if (WeaponID == -1)
        {
            WeaponInUse = null;
            InverseKinematics = false;
        }
        else
        {
            InverseKinematics = true;
        }
        Shot = false;
    }
    #endregion

    #region Actions

    // LOCOMOTION
    void Inputs()
    {
        //Dying
        if (Health <= 0 && IsDead == false)
        {
            anim.SetTrigger("die");

            if (SlowmotionWhenDie)
            {
                MyPivotCamera.DoSlowMotion(0.1f, 4f);
            }
            if (RagdollWhenDie == true && advancedRagdollController != null)
            {
                advancedRagdollController.State = AdvancedRagdollController.RagdollState.Ragdolled;
                advancedRagdollController.TimeToGetUp = 900;
            }
            IsDead = true;
        }

        HorizontalX = JUInput.GetAxis(JUInput.Axis.MoveHorizontal);
        VerticalY = JUInput.GetAxis(JUInput.Axis.MoveVertical);

        HorizontalX = Mathf.Clamp(HorizontalX, -1, 1);
        VerticalY = Mathf.Clamp(VerticalY, -1, 1);

        //GetTheCameraRotation
        DesiredCameraRotation = MyCamera.transform.rotation;
        DesiredCameraRotation.x = 0;
        DesiredCameraRotation.z = 0;

        //Crouch
        if (JUInput.GetButtonDown(JUInput.Buttons.CrouchButton) && IsGrounded == true && IsRunning == false && IsDriving == false)
        {
            IsCrouched = !IsCrouched;
        }

        //XBOX 360 CROUCH 
        if (JUInput.GetButton(JUInput.Buttons.CrouchButton) && IsGrounded == true && IsRunning == false && IsDriving == false)
        {
            IsCrouched = true;
        }

        if (JUInput.GetButtonUp(JUInput.Buttons.CrouchButton) && IsGrounded == true && IsRunning == false && IsDriving == false)
        {
            IsCrouched = false;
        }

        anim.SetBool("crouched", IsCrouched);

        //Ground Check
        if (IsDriving == false)
        {
            var groundcheck = Physics.OverlapBox(transform.position + transform.up * GroundCheckHeighOfsset, new Vector3(GroundCheckRadious, GroundCheckSize, GroundCheckRadious), transform.rotation, WhatIsGround);
            if (groundcheck.Length != 0 && IsJumping == false)
            {
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
        }
        anim.SetBool("isgrounded", IsGrounded);
        anim.SetBool("jumping", IsJumping);
        //Wall in front
        RaycastHit HitFront;
        if (Physics.Raycast(transform.position + transform.up * 1f, DirectionTransform.forward, out HitFront, 0.6f, WhatIsGround))
        {
            WallInFront = true;
            Debug.DrawLine(HitFront.point, transform.position + transform.up * 1f);
        }
        else
        {
            WallInFront = false;
        }
        if (WallInFront == true)
        {
            VelocityMultiplier = Mathf.Lerp(VelocityMultiplier, 0, 5 * Time.deltaTime);
        }

        //Jump
        if (JUInput.GetButtonDown(JUInput.Buttons.JumpButton) && IsGrounded == true && IsJumping == false && IsRolling == false && IsDriving == false)
        {
            rb.AddForce(transform.up * 200 * JumpForce, ForceMode.Impulse);
            IsGrounded = false;
            IsJumping = true;
            IsCrouched = false;
            Invoke("_disablejump", .5f);
        }

        //Roll
        if (JUInput.GetButtonDown(JUInput.Buttons.RollButton) && IsGrounded == true && IsRolling == false)
        {
            anim.SetTrigger("roll");
            Invoke("_disableroll", 1f);
        }
        //Running
        if (JUInput.GetButton(JUInput.Buttons.RunButton))
        {
            IsRunning = true;
            IsCrouched = false;
        }
        else
        {
            IsRunning = false;
        }


        anim.SetBool("running", IsRunning);

        //Punch Attacks
        if (EnabledPunchAttacks)
        {
            if (JUInput.GetButtonDown(JUInput.Buttons.ShotButton) && IsRolling == false && IsDriving == false && IsArmed == false && WeaponInUse == null)
            {
                PunchAttack();
            }
        }

        //Is Attacking time counter
        if (IsAttacking == true)
        {
            currenttimetodisable_isattacking += Time.deltaTime;
            if (currenttimetodisable_isattacking >= 0.7f)
            {
                currenttimetodisable_isattacking = 0;
                IsAttacking = false;
            }
        }

        //Shot / Reload / Aiming
        if (IsArmed && WeaponInUse != null)
        {
            //Aiming
            if (JUInput.GetButtonDown(JUInput.Buttons.AmingButton))
            {
                IsAiming = !IsAiming;
            }
            CanShoot = WeaponInUse.CanShoot;

            //Auto Shooting
            if (WeaponInUse.FireMode != Weapon.WeaponFireMode.SemiAuto)
            {
                if (JUInput.GetButton(JUInput.Buttons.ShotButton))
                {
                    if (IsRolling == false && IsDriving == false && WeightIK > 0.7f && CanShoot)
                    {
                        PullWeaponTrigger();
                    }
                }
            }
            else //Semi-Auto Shooting
            {
                //Shot in normal fire rate
                if (JUInput.GetButton(JUInput.Buttons.ShotButton))
                {
                    if (IsRolling == false && IsDriving == false && WeightIK > 0.7f && CanShoot)
                    {
                        PullWeaponTrigger();
                    }
                }

                //Force shooting out of firerate
                if (JUInput.GetButtonDown(JUInput.Buttons.ShotButton) && IsRolling == false && IsDriving == false && WeightIK > 0.7f && WeaponInUse.BulletsAmounts > 0 && WeaponInUse.IsShooted == true && WeaponInUse.CurrentFireRateToShoot > 0.12f)
                {
                    WeaponInUse.Shot();
                }
            }

            //Disable Aim
            if (IsRolling || IsDead || IsDriving || IsReloading || WeaponInUse.AimMode == Weapon.WeaponAimMode.None)
            {
                IsAiming = false;
            }

            //Reload
            if (JUInput.GetButtonDown(JUInput.Buttons.ReloadButton) && WeaponInUse.BulletsAmounts < WeaponInUse.BulletsPerMagazine && WeaponInUse.TotalBullets > 0 && IsReloading == false)
            {
                anim.SetTrigger("reload");
                IsReloading = true;
            }
            //Auto-Reload
            if (JUInput.GetButton(JUInput.Buttons.ShotButton) && WeaponInUse.BulletsAmounts == 0 && WeaponInUse.TotalBullets > 0 && IsReloading == false)
            {
                anim.SetTrigger("reload");
                IsReloading = true;
            }
        }
        // >>> Weapon Switch
        if (IsDriving == false)
        { //if IsDriving == true cant change weapons
            if (Input.GetAxis("Mouse ScrollWheel") >= 0.05f)
            {
                if (WeaponID < Weapons.Length - 1)
                {
                    _NextWeapon();
                    InverseKinematics = true;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") <= -0.05f)
            {
                if (WeaponID > -1)
                {
                    _PreviousWeapon();
                }
            }

            if (JUInput.GetButtonDown(JUInput.Buttons.NextWeaponButton))
            {
                _NextWeapon();
            }
            if (JUInput.GetButtonDown(JUInput.Buttons.PreviousWeaponButton))
            {
                _PreviousWeapon();
            }
        }

    }
    void Movement()
    {
        //Handle Slope Slide
        SlopeSlide();
        //Handle Root Motion
        if (RootMotion)
        {
            if (IsGrounded == true && IsJumping == false && IsRolling == false)
            {
                anim.applyRootMotion = true;
            }
            else
            {
                anim.applyRootMotion = false;
            }
        }
        //Moving Check
        if (VerticalY > 0.01f || VerticalY < -0.01f || HorizontalX > 0.01f || HorizontalX < -0.01f)
        {
            IsMoving = true;
        }
        if (VerticalY == 0 && HorizontalX == 0)
        {
            IsMoving = false;
        }

        if (!IsArmed && IsDriving == false)
        {
            IsArmedWeight = Mathf.Lerp(IsArmedWeight, 0, 5 * Time.deltaTime);
            anim.SetFloat("velocity", VelocityMultiplier);
            if (IsGrounded && CanMove && IsAttacking == false)
            {
                //rb.velocity = transform.forward * VelocityMultiplier * Speed + transform.up * rb.velocity.y;
                if (CurvedMovement == true && IsArmed == false)
                {
                    if (RootMotion == false)
                    {
                        rb.velocity = transform.forward * VelocityMultiplier * Speed + transform.up * rb.velocity.y;
                    }
                }
                else
                {
                    if (RootMotion == false)
                    {
                        rb.velocity = DirectionTransform.forward * VelocityMultiplier * Speed + transform.up * rb.velocity.y;
                    }
                }
            }
            if (IsMoving && IsAttacking == false)
            {
                if (IsRunning == true && WallInFront == false)
                {
                    float correctvel = 2 + (GroundAngle / 40);
                    VelocityMultiplier = Mathf.Lerp(VelocityMultiplier, 1.5f - GroundAngle / 40, correctvel * Time.deltaTime);
                }
                else if (WallInFront == false)
                {
                    VelocityMultiplier = Mathf.Lerp(VelocityMultiplier, 0.5f - GroundAngle / 400, 5 * Time.deltaTime);
                }
            }
            else
            {
                VelocityMultiplier = Mathf.Lerp(VelocityMultiplier, 0.0f, 5 * Time.deltaTime);
                IsRunning = false;
            }
        }
        else if (IsArmed && WeaponInUse != null && IsDriving == false)
        {
            if (IsRolling == false)
            {
                IsArmedWeight = Mathf.Lerp(IsArmedWeight, 1, 5 * Time.deltaTime);

                if (!Game.IsMobile)
                {
                    var VY = Mathf.Lerp(anim.GetFloat("vertical"), 3 * VelocityMultiplier * VerticalY, 4 * Time.deltaTime);
                    var HX = Mathf.Lerp(anim.GetFloat("horizontal"), 3 * VelocityMultiplier * HorizontalX, 4 * Time.deltaTime);
                    anim.SetFloat("vertical", VY);
                    anim.SetFloat("horizontal", HX);
                }
                else
                {
                    var VY = Mathf.Lerp(anim.GetFloat("vertical"), 3 * VelocityMultiplier * VerticalY, 8 * Time.deltaTime);
                    var HX = Mathf.Lerp(anim.GetFloat("horizontal"), 3 * VelocityMultiplier * HorizontalX, 8 * Time.deltaTime);
                    anim.SetFloat("vertical", VY);
                    anim.SetFloat("horizontal", HX);
                }


                //Movement
                if (CanMove && IsGrounded)
                {
                    rb.velocity = DirectionTransform.forward * VelocityMultiplier * Speed + transform.up * rb.velocity.y;
                }


                if (IsRunning == true && WallInFront == false && IsGrounded == true && IsMoving == true)
                {
                    float correctvel = 2 + (GroundAngle / 40);
                    VelocityMultiplier = Mathf.Lerp(VelocityMultiplier, 1f - GroundAngle / 90, correctvel * Time.deltaTime);
                }
                if (IsRunning == false && WallInFront == false && IsGrounded == true && IsMoving == true)
                {
                    VelocityMultiplier = Mathf.Lerp(VelocityMultiplier, 0.5f - GroundAngle / 400, 5 * Time.deltaTime);
                }
                if (IsMoving == false)
                {
                    VelocityMultiplier = Mathf.Lerp(VelocityMultiplier, 0f, 5 * Time.deltaTime);
                }
            }
        }

        anim.SetLayerWeight(1, IsArmedWeight);
        anim.SetBool("armed", IsArmed);

        //Rolling
        if (IsRolling == true && WallInFront == false)
        {
            transform.Translate(0, 0, 4 * Time.fixedDeltaTime);
            VelocityMultiplier = 0;
        }
        //Velocity On Jump
        if (IsGrounded)
        {
            LastX = HorizontalX;
            LastY = VerticalY;
            LastVelMult = VelocityMultiplier;
            CanMove = true;
        }
        else
        {
            transform.Translate(0, -1 * Time.deltaTime, 0);
            if (IsMoving)
            {
                rb.velocity = rb.velocity + DirectionTransform.forward * AirInfluenceControll / 10 * Time.deltaTime;
            }
        }
    }
    void Rotate()
    {
        if (IsDriving == true || IsAttacking == true)
            return;

        //Rotation Direction
        DesiredDirection = new Vector3(HorizontalX, 0, VerticalY);

        // ---- BODY ROTATION ----
        Vector3 DesiredEulerAngles = transform.eulerAngles;

        if (IsMoving)
        {
            // >>> Set Desired Direction
            float h = Mathf.Abs(HorizontalX);
            float v = Mathf.Abs(VerticalY);
            if (h > 0.01f || v > 0.01f)
            {
                DirectionTransform.rotation = Quaternion.LookRotation(DesiredDirection) * DesiredCameraRotation;
                DirectionTransform.rotation = Quaternion.FromToRotation(DirectionTransform.up, transform.up) * DirectionTransform.rotation;
            }
            DirectionTransform.position = transform.position;
            DesiredEulerAngles.y = Mathf.LerpAngle(DesiredEulerAngles.y, DirectionTransform.eulerAngles.y, RotationSpeed * Time.deltaTime);
        }


        // >>> Calculate Body Inclination
        float AngleBetweenDesiredDirectionAndCurrentDirection = Vector3.SignedAngle(transform.forward, DirectionTransform.forward, transform.up);

        if (IsMoving && BodyInclination && CanMove && !WallInFront)
        {
            if (IsGrounded)
            {
                BodyRotation = Mathf.LerpAngle(BodyRotation, AngleBetweenDesiredDirectionAndCurrentDirection / 180, 2.5f * Time.deltaTime);

                if (Mathf.Abs(AngleBetweenDesiredDirectionAndCurrentDirection) < 10)
                {
                    BodyRotation = Mathf.LerpAngle(BodyRotation, 0, 2 * Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(transform.rotation, DirectionTransform.rotation, 3 * Time.deltaTime);
                }
            }
            else
            {
                BodyRotation = Mathf.Lerp(BodyRotation, 0f, 8 * Time.deltaTime);
            }
        }
        else
        {
            BodyRotation = Mathf.Lerp(BodyRotation, 0f, 8 * Time.deltaTime);
        }


        anim.SetFloat("bodyrotation", BodyRotation);


        if (IsArmed && WeaponInUse != null)
        {
            if (IsRolling == false)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, DesiredCameraRotation, 15 * Time.deltaTime);
            }
        }
        else
        {
            // >>> Set Transform Rotation
            if (RootMotionRotation == false || RootMotion == false)
                transform.eulerAngles = DesiredEulerAngles;
        }

        anim.SetLayerWeight(1, IsArmedWeight);
        anim.SetBool("armed", IsArmed);

        //Rolling
        if (IsRolling == true)
        {
            IsArmedWeight = Mathf.Lerp(IsArmedWeight, 0f, 5 * Time.deltaTime);
            InverseKinematics = false;
            CanMove = false;
            if (DesiredDirection != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(DesiredDirection) * DesiredCameraRotation, 8 * Time.deltaTime);
        }
    }

    public bool IsSliding;
    [HideInInspector] private float SlidingVelocity;
    private float GroundAngle;
    void SlopeSlide()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up * 0.3f, -transform.up, out hit, 1, WhatIsGround))
        {
            GroundAngle = Vector3.Angle(Vector3.up, hit.normal);
            if (GroundAngle > 45)
            {
                if (IsSliding == false)
                {
                    SlidingVelocity = 0;
                    IsSliding = true;
                }
                else
                {
                    SlidingVelocity += Physics.gravity.y * Time.deltaTime;
                    rb.velocity = Vector3.up * SlidingVelocity + Vector3.forward * rb.velocity.z + Vector3.right * rb.velocity.x;
                }
            }
            else
            {
                IsSliding = false;
            }
        }
    }



    [HideInInspector] public RaycastHit Step_Hit;
    [HideInInspector] private bool AdjustHeight;
    void StepCorrect()
    {
        if (IsGrounded && IsMoving && EnableStepCorrection == true)
        {
            //Friction
            RaycastHit friction_hit;
            if (Physics.Raycast(transform.position + transform.up * 1.5f + DirectionTransform.forward * 0.3f, -transform.up, out friction_hit, 2f, WhatIsGround))
            {
                if (friction_hit.point.y + .05 < transform.position.y)
                {
                    //Apply Friction Force
                    rb.AddForce(0, -10, 0, ForceMode.Impulse);
                }
            }

            //Step height Correction
            if (Physics.Raycast(transform.position + transform.up * FootstepHeight + DirectionTransform.forward * ForwardStepOffset, -transform.up, out Step_Hit, FootstepHeight - StepHeight, WhatIsGround) && AdjustHeight == false)
            {
                if (Step_Hit.point.y > transform.position.y + .03f && AdjustHeight == false)
                {
                    AdjustHeight = true;
                }
            }


            //if (Step_Hit.point != Vector3.zero && Step_Hit.point.y > transform.position.y + FootstepHeight - FootstepHeight / 2)
            //{
            //    VelocityMultiplier = Mathf.Lerp(VelocityMultiplier, 0, 8 * Time.deltaTime);
            //    WallInFront = true;
            //}
        }
        if (AdjustHeight && Step_Hit.point != Vector3.zero)
        {
            if (IsGrounded == true)
            {
                transform.position = Vector3.Lerp(transform.position, Step_Hit.point + transform.up * ForwardStepOffset, VelocityMultiplier * 3 * Time.deltaTime);
            }

            if (transform.position == Step_Hit.point)
                AdjustHeight = false;
        }
    }

    void PunchAttack()
    {
        if (WeaponInUse == null && IsGrounded == true && IsArmed == false)
        {

            currenttimetodisable_isattacking = 0f;

            IsAttacking = true;
            anim.SetTrigger("punch");

            IsCrouched = false;
        }
    }

    // WEAPON SYSTEM
    void WeaponControl()
    {
        //Weapon Control
        if (IsArmed && WeaponInUse != null)
        {
            if (WeaponInUse.BulletsAmounts == 0)
            {
                CanShoot = false;
                Shot = false;
            }
        }
        //If is Armed
        if (WeaponID == -1)
        {
            IsArmed = false;
        }
        else
        {
            IsArmed = true;
        }

        //Clamp Weapon ID
        WeaponID = Mathf.Clamp(WeaponID, -1, Weapons.Length);
    }
    void PickUpWeapons()
    {
        RaycastHit hitweapon;
        if (Physics.Raycast(MyCamera.transform.position, MyCamera.transform.forward, out hitweapon, MyPivotCamera.Distance + 1, WeaponLayer))
        {
            if (hitweapon.collider.gameObject.layer == 14)
            {
                ToPickupWeapon = true;

                if (JUInput.GetButtonDown(JUInput.Buttons.PickupButton))
                {
                    if (Weapons.Length > 0)
                    {
                        if (Weapons[hitweapon.transform.GetComponent<Weapon>().WeaponSwitchID].Unlocked == false)
                        {
                            Weapons[hitweapon.transform.GetComponent<Weapon>().WeaponSwitchID].Unlocked = true;
                            Weapons[hitweapon.transform.GetComponent<Weapon>().WeaponSwitchID].TotalBullets += hitweapon.transform.GetComponent<Weapon>().BulletsAmounts;
                            Destroy(hitweapon.transform.gameObject);
                        }
                        else
                        {
                            Weapons[hitweapon.transform.GetComponent<Weapon>().WeaponSwitchID].TotalBullets += hitweapon.transform.GetComponent<Weapon>().BulletsAmounts;
                            Destroy(hitweapon.transform.gameObject);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("The Third Person Controller does not yet have any weapons registered to the ID:" + hitweapon.transform.GetComponent<Weapon>().WeaponSwitchID + " don't you forget to link any existing weapons within the hand of the third person controller with this ID?");
                    }
                }
            }
            else
            {
                ToPickupWeapon = false;
            }
        }
        else
        {
            ToPickupWeapon = false;
        }
    }
    void PullWeaponTrigger()
    {
        if (WeaponInUse != null)
        {
            WeaponInUse.PullTrigger();
            if (WeaponInUse.FireMode == Weapon.WeaponFireMode.BoltAction && WeaponInUse.IsShooted == true)
            {
                anim.SetTrigger("pullbolt");
                //IsAiming = false;
            }
        }
    }
    void WeaponIKControl()
    {
        if (IsArmed == true && IsRolling == false)
        {
            // Weapons Aim Rotation Center
            PivotWeapons.transform.position = HumanoidSpine.position;
            PivotWeapons.transform.rotation = Quaternion.Euler(PivotWeapons.transform.eulerAngles.x, transform.eulerAngles.y, PivotWeapons.transform.eulerAngles.z);
            PivotWeapons.transform.rotation = Quaternion.Lerp(PivotWeapons.transform.rotation, MyCamera.transform.rotation, 15 * Time.deltaTime);
        }

        // Hands IK
        if (WeaponInUse != null)
        {
            IKPositionRightHand.position = WeaponPositions.WeaponPositionTransform[WeaponInUse.WeaponPositionID].position;
            IKPositionRightHand.rotation = WeaponPositions.WeaponPositionTransform[WeaponInUse.WeaponPositionID].rotation;

            if (WeaponInUse.IK_Position_LeftHand != null)
            {
                IKPositionLeftHand.position = WeaponInUse.IK_Position_LeftHand.transform.position;
                IKPositionLeftHand.rotation = WeaponInUse.IK_Position_LeftHand.rotation;
            }
        }

        // IK Weight Control
        if (InverseKinematics == true)
        {
            WeightIK = Mathf.Lerp(WeightIK, 1, 5 * Time.deltaTime);
        }
        else
        {
            WeightIK = Mathf.Lerp(WeightIK, 0, 3 * Time.deltaTime);
        }
    }

    //VEHICLE SYSTEM
    void DriveControl()
    {
        //Driving Control
        anim.SetBool("driving", IsDriving);
        if (IsDriving == true && VehicleInArea != null)
        {
            CanMove = false;
            IsJumping = false;
            IsCrouched = false;
            VelocityMultiplier = 0;

            //SimulateInert
            rb.velocity = VehicleInArea.rb.velocity;

            IsArmedWeight = 0;
            WeaponID = -1;
            IsArmed = false;
            if (WeaponInUse != null)
            {
                WeaponInUse.gameObject.SetActive(false);
                WeaponInUse = null;
            }

            anim.SetBool("crouched", false);
            anim.SetLayerWeight(1, IsArmedWeight);
            anim.SetBool("armed", IsArmed);
            anim.SetBool("isgrounded", true);
            anim.SetBool("running", false);
            anim.SetFloat("bodyrotation", 0);


            transform.position = VehicleInArea.PlayerLocation.position;
            transform.rotation = VehicleInArea.PlayerLocation.rotation;

            GetComponent<Collider>().isTrigger = true;
            rb.useGravity = false;

            VehicleInArea.IsOn = true;
        }
        else
        {
            GetComponent<Collider>().isTrigger = false;
            rb.useGravity = true;
            if (VehicleInArea != null)
            {
                VehicleInArea.IsOn = false;
            }
        }
        //Enter in car
        if (JUInput.GetButtonDown(JUInput.Buttons.EnterVehicleButton))
        {
            if (IsDriving == false)
            {
                if (VehicleInArea != null)
                {
                    ToEnterVehicle = false;
                    IsDriving = true;
                    IsReloading = false;
                }
            }
            else
            {
                IsDriving = false;
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                if (VehicleInArea.TypeOfVehicle == Vehicle.VehicleType.Car)
                {
                    transform.position = VehicleInArea.transform.position - transform.right * 1.2f;
                }
                else
                {
                    transform.position = VehicleInArea.transform.position - transform.right * 0.5f;
                }
            }
        }

        if (VehicleInArea == null)
            IsDriving = false;
    }

    //FOOTSTEP
    private void FootstepsTimer()
    {
        if (IsGrounded == true && IsRolling == false && IsDriving == false && CanMove == true && FootstepAudioClips.Count > 0)
        {
            if (HorizontalX > 0.02f || HorizontalX < -0.02f || VerticalY > 0.02f || VerticalY < -0.02f)
            {
                float vm = VelocityMultiplier;
                vm = Mathf.Clamp(vm, 0, 1);
                currentFootstepTime += (vm) * Time.deltaTime;
                if (currentFootstepTime >= MaxFootstepTime)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, out hit, 1, WhatIsGround))
                    {
                        for (int i = 0; i < FootstepAudioClips.Count; i++)
                        {
                            if (FootstepAudioClips[i].SurfaceTag == hit.transform.gameObject.tag)
                            {
                                AudioS.PlayOneShot(FootstepAudioClips[i].AudioClips[Random.Range(0, FootstepAudioClips.Count)]);
                                currentFootstepTime = 0;
                            }
                        }
                    }
                }
            }
            else
            {
                currentFootstepTime = 0;
            }
        }
    }

    [System.Serializable]
    public class FootstepAudios
    {
        public string SurfaceTag;
        public List<AudioClip> AudioClips = new List<AudioClip>(4);
    }

    //FOOT PLACEMENT IK
    private void FootPlacementIKController()
    {
        if (FootPlacerIK != null)
        {
            if (!IsGrounded || IsJumping || IsDriving || IsDead || IsRolling)
            {
                if (!IsRolling)
                    FootPlacerIK.SmoothIKTransition = false;

                FootPlacerIK.BlockBodyPositioning = true;
                FootPlacerIK.EnableDynamicBodyPlacing = false;
            }
            else
            {
                FootPlacerIK.SmoothIKTransition = true;
                if (!IsCrouched)
                {
                    FootPlacerIK.EnableDynamicBodyPlacing = true;
                }
                else
                {
                    FootPlacerIK.EnableDynamicBodyPlacing = false;
                }
            }
        }
    }

    //DISABLE / ENABLE LOCOMOTION FUNCTIONS
    public void DisableLocomotion()
    {
        HorizontalX = 0;
        VerticalY = 0;
        VelocityMultiplier = 0;
        DisableAllMove = true;
    }
    public void DisableLocomotionForTime(float time)
    {
        HorizontalX = 0;
        VerticalY = 0;
        VelocityMultiplier = 0;
        DisableAllMove = true;
        Invoke("EnableMove", time);
    }
    public void EnableMove()
    {
        DisableAllMove = false;
    }

    #endregion

    #region Invoke Events
    void WeaponRecoil()
    {
        if (WeaponInUse != null)
        {
            WeaponPositions.WeaponPositionTransform[WeaponInUse.WeaponPositionID].Translate(0, 0, -WeaponInUse.RecoilForce);
            WeaponPositions.WeaponPositionTransform[WeaponInUse.WeaponPositionID].Rotate(0, -WeaponInUse.RecoilForceRotation, 0);

            if (MyPivotCamera.CameraShakeWhenShooting == true)
            {
                if (WeaponInUse.FireMode == Weapon.WeaponFireMode.BoltAction)
                {
                    MyPivotCamera.rotX -= MyPivotCamera.CameraShakeSensibility * 20 * WeaponInUse.RecoilForce;
                }
                else
                {
                    MyPivotCamera.rotX -= MyPivotCamera.CameraShakeSensibility * 20 * WeaponInUse.RecoilForce;
                }
            }
        }
    }
    private void _disablejump()
    {
        IsJumping = false;
    }
    private void _disableroll()
    {
        IsRolling = false;
    }


    #endregion

    #region Animation Events
    public void reload()
    {
        if (IsReloading == true)
        {
            WeaponInUse.Reload();
            IsReloading = false;
        }
    }
    public void bulletcasing()
    {
        if (WeaponInUse == null) return;
        if (WeaponInUse.BulletCasingPrefab != null)
        {
            WeaponInUse.EmitBulletCasing();
        }
    }
    public void nomove()
    {
        CanMove = false;
    }
    public void move()
    {
        DisableAllMove = false;
        CanMove = true;
    }
    public void uppunch()
    {
        IsAttacking = true;
        currenttimetodisable_isattacking = 0;
        print("Attacked with a punch up");
        rb.velocity = transform.forward * 4f;
        var lefthand = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        RaycastHit hit;
        if (Physics.Raycast(lefthand.transform.position - transform.forward * 0.5f, transform.forward, out hit, .95f, WhatIsGround))
        {
            var particle = (GameObject)Instantiate(PunchCollisionParticle, hit.point, lefthand.rotation);
            Destroy(particle, 1f);
            Debug.DrawLine(hit.point, lefthand.transform.position - transform.forward * 0.5f);
            MyPivotCamera.Shake(4f);
        }
    }
    public void Rpunch()
    {
        IsAttacking = true;
        currenttimetodisable_isattacking = 0;
        print("Attacked with a right punch");
        rb.velocity = transform.forward * 3f;
        var righthand = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
        RaycastHit hit;
        if (Physics.Raycast(righthand.transform.position - transform.forward * 0.5f, transform.forward, out hit, .95f, WhatIsGround))
        {
            var particle = (GameObject)Instantiate(PunchCollisionParticle, hit.point, righthand.rotation);
            Destroy(particle, 1f);
            Debug.DrawLine(hit.point, righthand.transform.position - transform.forward * 0.5f);
            MyPivotCamera.Shake(3f);
        }
    }
    public void Lpunch()
    {
        IsAttacking = true;
        currenttimetodisable_isattacking = 0;
        print("Attacked with a left punch");
        rb.velocity = transform.forward * 2f;
        var lefthand = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        RaycastHit hit;
        if (Physics.Raycast(lefthand.transform.position - transform.forward * 0.5f, transform.forward, out hit, .95f, WhatIsGround))
        {
            var particle = (GameObject)Instantiate(PunchCollisionParticle, hit.point, lefthand.rotation);
            Destroy(particle, 1f);
            Debug.DrawLine(hit.point, lefthand.transform.position - transform.forward * 0.5f);
            MyPivotCamera.Shake(2f);
        }
    }
    public void noik()
    {
        InverseKinematics = false;
    }
    public void ik()
    {
        InverseKinematics = true;
    }
    public void rollout()
    {
        CanMove = true;
        IsRolling = false;
        ik();
    }
    public void roll()
    {
        IsRolling = true;
        noik();
    }

    #endregion

    #region Physics Check
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "VehicleArea" && IsDriving == false)
        {
            VehicleInArea = other.GetComponentInParent<Vehicle>();
            ToEnterVehicle = true;
        }
        if (other.gameObject.tag == "DeadZone")
        {
            Health = 0;
            print("Killed by Dead Zone");
        }
        if (other.gameObject.tag == "RagdollZone" && advancedRagdollController != null)
        {
            advancedRagdollController.State = AdvancedRagdollController.RagdollState.Ragdolled;
            print("Ragdolled");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "VehicleArea" && IsDriving == false)
        {
            VehicleInArea = null;
            ToEnterVehicle = false;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "DeadZone")
        {
            Health = 0;
            print("Killed by Dead Zone");
        }
        if (other.gameObject.tag == "RagdollZone" && advancedRagdollController != null)
        {
            advancedRagdollController.State = AdvancedRagdollController.RagdollState.Ragdolled;
            print("Ragdolled");
        }
    }


    #endregion

    void OnAnimatorIK(int layerIndex)
    {
        if (layerIndex == 0 && DisableAllMove == false)
        {
            if (IsDead == false)
            {
                if (WeaponInUse != null && IsRolling == false && IsDriving == false)
                {
                    anim.SetIKRotationWeight(AvatarIKGoal.RightHand, WeightIK);
                    anim.SetIKPositionWeight(AvatarIKGoal.RightHand, WeightIK);

                    anim.SetIKPosition(AvatarIKGoal.RightHand, IKPositionRightHand.position);
                    anim.SetIKRotation(AvatarIKGoal.RightHand, IKPositionRightHand.rotation);

                    if (WeaponInUse.IK_Position_LeftHand != null)
                    {
                        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, WeightIK);
                        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, WeightIK);

                        anim.SetIKPosition(AvatarIKGoal.LeftHand, IKPositionLeftHand.position);
                        anim.SetIKRotation(AvatarIKGoal.LeftHand, IKPositionLeftHand.rotation);
                    }
                    anim.SetLookAtWeight(WeightIK, WeightIK / 2, WeightIK);
                    anim.SetLookAtPosition(HumanoidSpine.position + MyCamera.transform.forward * 200f);
                }
                if (IsDriving)
                {
                    anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);

                    anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                    anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                    anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

                    anim.SetLookAtWeight(0.9f, 0.0f, 0.8f);
                    anim.SetLookAtPosition(HumanoidSpine.position + MyCamera.transform.forward * 50);

                    anim.SetIKPosition(AvatarIKGoal.RightHand, VehicleInArea.RightHandPositionIK.position);
                    anim.SetIKRotation(AvatarIKGoal.RightHand, VehicleInArea.RightHandPositionIK.rotation);
                    anim.SetIKPosition(AvatarIKGoal.LeftHand, VehicleInArea.LeftHandPositionIK.position);
                    anim.SetIKRotation(AvatarIKGoal.LeftHand, VehicleInArea.LeftHandPositionIK.rotation);

                    anim.SetIKPosition(AvatarIKGoal.RightFoot, VehicleInArea.RightFootPositionIK.position);
                    anim.SetIKRotation(AvatarIKGoal.RightFoot, VehicleInArea.RightFootPositionIK.rotation);
                    anim.SetIKPosition(AvatarIKGoal.LeftFoot, VehicleInArea.LeftFootPositionIK.position);
                    anim.SetIKRotation(AvatarIKGoal.LeftFoot, VehicleInArea.LeftFootPositionIK.rotation);
                }
            }
        }
    }

    private void OnAnimatorMove()
    {
        if (RootMotion && IsGrounded && IsJumping == false && !IsArmed && !IsDriving)
        {
            anim.updateMode = AnimatorUpdateMode.AnimatePhysics;
            RootMotionDeltaPosition = anim.deltaPosition * Time.fixedDeltaTime;
            rb.velocity = RootMotionDeltaPosition * 10000 * RootMotionSpeed + transform.up * rb.velocity.y;
            if (RootMotionRotation)
            {
                transform.Rotate(0, anim.deltaRotation.y * 160, 0);
            }
        }
    }
}
