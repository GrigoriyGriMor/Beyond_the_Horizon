using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputPlayerManager : MonoBehaviour
{
    public PlayerController player;

    public CarBase carBase;

    [Header("MoveAxis")]
    [SerializeField] private Vector2 moveAxis = Vector2.zero;
    [SerializeField] private Vector2 mouseAxis = Vector2.zero;

    [Header("Control Buttons")]
    public KeyCode jump = KeyCode.Space;
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode crouch = KeyCode.LeftControl;
    public KeyCode aiming = KeyCode.Mouse1;
    public KeyCode fire = KeyCode.Mouse0;
    public KeyCode reloadWeapon = KeyCode.R;
    public KeyCode uesObject = KeyCode.F;
    public KeyCode useGrenade = KeyCode.G;
    public KeyCode exitFightMode = KeyCode.Z;

    [Header("Weapon use buttons")]
    public KeyCode weapon_1 = KeyCode.Alpha1;
    public KeyCode weapon_2 = KeyCode.Alpha2;

    [Header("SkillsButtons")]
    public KeyCode skill_1 = KeyCode.Q;
    public KeyCode skill_2 = KeyCode.E;

    [Header("Usible Buttons")]
    public KeyCode openQuickMenu = KeyCode.Tab;
    public KeyCode openMap = KeyCode.M;
    public KeyCode openInventory = KeyCode.I;
    public KeyCode openJournal = KeyCode.J;
    public KeyCode openStore = KeyCode.O;
    public KeyCode openIslandStore = KeyCode.P;

    public KeyCode openChat = KeyCode.Return;
    public KeyCode escapeButton = KeyCode.Escape;

    public KeyCode lookCameraRotation = KeyCode.C;

    [Header("Режим")]
    [SerializeField] private ClientDataInput serverConnect;
    public Keys croupKeys = 0;

    private InputClientData currentButtonState;

    private bool clientControl = true;

    public void ClientControl(bool b) {
            clientControl = b;

        if (!clientControl)
            player.DialogIsActive();
    }

    public InputClientData GetCurrentButtonState()
    {
        return currentButtonState;
    }

    private void Awake() {
        serverConnect = GameObject.FindObjectOfType<ClientDataInput>();
    }

    public void NewInputIteration(InputClientData newData) {
        try
        {
            if (!player.gameIsPlayed) return;
            currentButtonState = newData;

            //for test
            moveAxis = new Vector2(newData.AxisX, newData.AxisY);
            mouseAxis = new Vector2(newData.mouseDX * 0.01f, newData.mouseDY * 0.01f);

            //0 = notUse, 1 = KeyDown, 2 = KeyDrag, 3 = KeyUp
            //Use camera controller
            if (player.UDPStatus != SupportClass.gameState.clone) {
                if (newData.btnlookCameraR == 2)
                    player.CameraRotate(mouseAxis, false);
                else
                    player.CameraRotate(mouseAxis, true);
            }

            // Use move controller
            player.MoveCharacter(moveAxis);

            // use swap player control mode (Idle, Combat)
            if (newData.btnSwapFightMode == 1)
                player.SwitchMode();

            //Use jumping
            if (newData.btnJump == 1)
                player.Jump();

            if (newData.btnCrouch == 1)
                player.CrouchControl(true);
            else
                if (newData.btnCrouch == 3)
                player.CrouchControl(false);

            if (newData.btnSprint == 1 && moveAxis != Vector2.zero)
                player.SprintControl(true);
            else
                if (newData.btnSprint == 3)
                player.SprintControl(false);

            if (newData.btnWeapon1 == 1)
                player.UseWeapon(0);
            else
                if (newData.btnWeapon2 == 1)
                player.UseWeapon(1);

            if (newData.btnFire == 2)
                player.Attack(true);
            else
                if (newData.btnFire == 3)
                player.Attack(false);

            if (newData.btnReload == 1)
                player.ReloadGun();

            if (newData.btnAim == 1)
                player.AimActivate(true);
            else
                if (newData.btnAim == 3)
                player.AimActivate(false);
        }
        catch (Exception ex)
        {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), name + gameObject.name);
        }
    }

    private void Update()
    {
        switch (player.UDPStatus) {
            case SupportClass.gameState.test: {

                    if (Input.GetKeyDown(openChat))
                        player.UseChat();

                    if (Input.GetKeyDown(openQuickMenu))
                        player.UseQuickSystem();

                    if (Input.GetKeyDown(openMap))
                        player.UseMap();
                    if (Input.GetKeyDown(openJournal))
                        player.UseMission();
                    if (Input.GetKeyDown(openInventory))
                        player.UseInventory();
                    if (Input.GetKeyDown(openStore))
                        player.UseStore();
                    if (Input.GetKeyDown(openIslandStore))
                        player.UseIslandStore();

                    if (!player.gameIsPlayed) return;

                    if (!clientControl) return;

                    //for test
                    moveAxis = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
                    mouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                    //Use camera controller
                    if (Input.GetKey(lookCameraRotation))
                        player.CameraRotate(mouseAxis, false);
                    else
                        player.CameraRotate(mouseAxis, true);

                    if (carBase) {
                        carBase.SetAxis(moveAxis);
                        return;
                    }
                    else {
                        // Use move controller
                        player.MoveCharacter(moveAxis);
                    }

                    // use swap player control mode (Idle, Combat)
                    if (Input.GetKeyDown(exitFightMode))
                        player.SwitchMode();

                    //Use jumping
                    if (Input.GetKeyDown(jump))
                        player.Jump();

                    if (Input.GetKeyDown(crouch))
                        player.CrouchControl(true);
                    else
                        if (Input.GetKeyUp(crouch))
                        player.CrouchControl(false);

                    if (Input.GetKeyDown(sprint) && moveAxis != Vector2.zero)
                        player.SprintControl(true);
                    else
                        if (Input.GetKeyUp(sprint))
                        player.SprintControl(false);

                    if (Input.GetKeyDown(weapon_1))
                        player.UseWeapon(0);
                    else
                        if (Input.GetKeyDown(weapon_2))
                        player.UseWeapon(1);

                    if (Input.GetKey(fire))
                        player.Attack(true);
                    else
                        if (Input.GetKeyUp(fire))
                        player.Attack(false);

                    if (Input.GetKeyDown(reloadWeapon))
                        player.ReloadGun();

                    if (Input.GetKeyDown(aiming))
                        player.AimActivate(true);
                    else
                        if (Input.GetKeyUp(aiming))
                        player.AimActivate(false);

                    if (Input.GetKeyDown(uesObject))
                        player.UseItem();

                    if (Input.GetKeyDown(skill_1))
                        player.UseSkill(0);

                    if (Input.GetKeyDown(skill_2))
                        player.UseSkill(1);
                }
                break;
            case SupportClass.gameState.server:
                if (!player.gameIsPlayed) return;

                break;
            case SupportClass.gameState.clone:
                if (!player.gameIsPlayed) return;

                break;
            case SupportClass.gameState.client: {

                    if (Input.GetKeyDown(openChat))
                        player.UseChat();

                    if (Input.GetKeyDown(openQuickMenu))
                        player.UseQuickSystem();

                    if (Input.GetKeyDown(openMap))
                        player.UseMap();
                    if (Input.GetKeyDown(openJournal))
                        player.UseMission();
                    if (Input.GetKeyDown(openInventory))
                        player.UseInventory();
                    if (Input.GetKeyDown(openStore))
                        player.UseStore();
                    if (Input.GetKeyDown(openIslandStore))
                        player.UseIslandStore();

                    if (!player.gameIsPlayed) return;

                    if (!clientControl) return;

                    moveAxis = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
                    mouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                    // mouseAxis = Input.mousePosition;
                    //for test 
                    serverConnect.SetNewMoveAxis(moveAxis);
                    serverConnect.SetNewMousePos(mouseAxis);

                    croupKeys = Keys.none;

                    //Use camera controller
                    if (Input.GetKey(lookCameraRotation))
                        croupKeys |= Keys.lookCameraR;

                    // use swap player control mode (Idle, Combat)
                    if (Input.GetKey(exitFightMode))
                        croupKeys |= Keys.exitFightMode;

                    //Use jumping
                    if (Input.GetKey(jump))
                        croupKeys |= Keys.jump;

                    if (Input.GetKey(crouch))
                        croupKeys |= Keys.crouch;

                    if (Input.GetKey(sprint))
                        croupKeys |= Keys.sprint;

                    if (Input.GetKey(weapon_1))
                        croupKeys |= Keys.weapon_1;
                    else
                        if (Input.GetKey(weapon_2))
                        croupKeys |= Keys.weapon_2;

                    if (Input.GetKey(fire))
                        croupKeys |= Keys.fire;

                    if (Input.GetKey(reloadWeapon))
                        croupKeys |= Keys.reloadWeapon;

                    if (Input.GetKey(aiming))
                        croupKeys |= Keys.aiming;

                    serverConnect.SetNewKeysState(croupKeys);

                    #region input TEST
                    //дублируем инпут в контроллер плеера для предсказаний анимаций и действий

                    //Use camera controller
                  /*  if (Input.GetKey(lookCameraRotation))
                        player.CameraRotate(mouseAxis, false);
                    else
                        player.CameraRotate(mouseAxis, true);*/

                    // Use move controller
                    player.MoveCharacter(moveAxis);

                    // use swap player control mode (Idle, Combat)
                    if (Input.GetKeyDown(exitFightMode))
                        player.SwitchMode();

                    //Use jumping
                    if (Input.GetKeyDown(jump))
                        player.Jump();

                    if (Input.GetKeyDown(crouch))
                        player.CrouchControl(true);
                    else
                        if (Input.GetKeyUp(crouch))
                        player.CrouchControl(false);

                    if (Input.GetKeyDown(sprint) && moveAxis != Vector2.zero)
                        player.SprintControl(true);
                    else
                        if (Input.GetKeyUp(sprint))
                        player.SprintControl(false);

                    if (Input.GetKeyDown(weapon_1))
                        player.UseWeapon(0);
                    else
                        if (Input.GetKeyDown(weapon_2))
                        player.UseWeapon(1);

                    if (Input.GetKey(fire))
                        player.Attack(true);
                    else
                        if (Input.GetKeyUp(fire))
                        player.Attack(false);

                    if (Input.GetKeyDown(reloadWeapon))
                        player.ReloadGun();

                    if (Input.GetKeyDown(aiming))
                        player.AimActivate(true);
                    else
                        if (Input.GetKeyUp(aiming))
                        player.AimActivate(false);
                    #endregion
            }
            break;
        }
    }
}

public enum Keys : uint
{
    none = 0b00000000000000000000000000000000,

    //контролирующие кнопки
    lookCameraR = 0b00000000000000000000000000000001,         //[0]  drag or not
    exitFightMode = 0b00000000000000000000000000000010,         //[1]  down
    jump = 0b00000000000000000000000000000100,         //[2] down
    crouch = 0b00000000000000000000000000001000,         //[3] down|up
    sprint = 0b00000000000000000000000000010000,         //[4] down|up
    weapon_1 = 0b00000000000000000000000000100000,         //[5] down
    weapon_2 = 0b00000000000000000000000001000000,         //[6] down
    fire = 0b00000000000000000000000010000000,         //[7] drag | up
    reloadWeapon = 0b00000000000000000000000100000000,         //[8] down
    aiming = 0b00000000000000000000001000000000,         //[9] down | up
    useObject = 0b00000000000000000000010000000000,         //[10] down
    useGrenade = 0b00000000000000000000100000000000,         //[11] down | up
}