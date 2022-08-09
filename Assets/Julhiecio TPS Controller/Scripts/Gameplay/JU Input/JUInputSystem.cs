using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JU_INPUT_SYSTEM;
[AddComponentMenu("JU TPS/Gameplay/Game/JU Input System")]

public class JUInputSystem : MonoBehaviour
{
	GameManagerAndUI Game;

	//Move and Rotate Axis
	[HideInInspector] public float MoveHorizontal;
	[HideInInspector] public float MoveVertical;
	[HideInInspector] public float RotateHorizontal;
	[HideInInspector] public float RotateVertical;
	
	
	[Header("Mobile Inputs")]
	//Mobile Joysticks
	public JoystickVirtual MovementJoystick;

	//Mobile Touchpads
	public Touchfield Touchfield;
	public Touchfield ShotButtonTouchfield;


	//Mobile Buttons
	private ButtonVirtual ShotButton, AimingButton, ReloadButton, RunButton, RunButtonRight, JumpButton,
		 CrouchButton, RollButton, PickWeaponButton, EnterVehicleButton, NextWeaponButton, PreviousWeaponButton, RightButton, LeftButton, ForwardButton, BackButton, BrakeButton;

	//Input Bools
	[HideInInspector]public bool PressedShooting, PressedAiming, PressedReload, PressedRun, PressedJump,
		 PressedCrouch, PressedRoll, PressedPickupWeapon, PressedEnterVehicle, PressedNextWeapon, PressedPreviousWeapon;

	[HideInInspector]public bool PressedShootingDown, PressedAimingDown, PressedReloadDown, PressedRunDown, PressedJumpDown,
		 PressedCrouchDown, PressedRollDown, PressedPickupWeaponDown, PressedEnterVehicleDown, PressedNextWeaponDown, PressedPreviousWeaponDown;
	
	[HideInInspector]public bool PressedShootingUp, PressedAimingUp, PressedReloadUp, PressedRunUp, PressedJumpUp,
		 PressedCrouchUp, PressedRollUp, PressedPickupWeaponUp, PressedEnterVehicleUp, PressedNextWeaponUp, PressedPreviousWeaponUp;


	private void Awake()
	{
		Game = FindObjectOfType<GameManagerAndUI>();

		//Touchfields
		Touchfield = GameObject.Find("Touch").GetComponent<Touchfield>();
		ShotButtonTouchfield = GameObject.Find("ShotButton").GetComponent<Touchfield>();

		//Controll Buttons
		MovementJoystick = FindObjectOfType<JoystickVirtual>();
		ShotButton = GameObject.Find("ShotButton").GetComponent<ButtonVirtual>();
		AimingButton = GameObject.Find("AimingButton").GetComponent<ButtonVirtual>();
		JumpButton = GameObject.Find("JumpButton").GetComponent<ButtonVirtual>();
		RunButton = GameObject.Find("RunButton").GetComponent<ButtonVirtual>();
		RunButtonRight = GameObject.Find("RightRunButton").GetComponent<ButtonVirtual>();
		RollButton = GameObject.Find("RollButton").GetComponent<ButtonVirtual>();
		CrouchButton = GameObject.Find("CrouchButton").GetComponent<ButtonVirtual>();
		ReloadButton = GameObject.Find("ReloadButton").GetComponent<ButtonVirtual>();

		//Interact Buttons
		PickWeaponButton = GameObject.Find("WeaponButton").GetComponent<ButtonVirtual>();
		EnterVehicleButton = GameObject.Find("VehicleButton").GetComponent<ButtonVirtual>();

		//Weapon Switch
		PreviousWeaponButton = GameObject.Find("PreviousWeaponButton").GetComponent<ButtonVirtual>();
		NextWeaponButton = GameObject.Find("NextWeaponButton").GetComponent<ButtonVirtual>();

		//Driving Buttons
		RightButton = GameObject.Find("RightButton").GetComponent<ButtonVirtual>();
		LeftButton = GameObject.Find("LeftButton").GetComponent<ButtonVirtual>();
		ForwardButton = GameObject.Find("ForwardButton").GetComponent<ButtonVirtual>();
		BackButton = GameObject.Find("BackButton").GetComponent<ButtonVirtual>();
		BrakeButton = GameObject.Find("BrakeButton").GetComponent<ButtonVirtual>();

	}
    private void Update()
    {
		UpdateGetButtonDown();
		UpdateGetButton();
		UpdateGetButtonUp();
        UpdateAxis();

		if (Game.IsMobile)
			UpdateMobileButtons();
    }
	private void UpdateMobileButtons()
    {
		PickWeaponButton.gameObject.SetActive(Game.PlayerCharacter.ToPickupWeapon);
		ReloadButton.gameObject.SetActive(Game.PlayerCharacter.IsArmed);
		AimingButton.gameObject.SetActive(Game.PlayerCharacter.IsArmed);

		if (Game.PlayerCharacter.EnabledPunchAttacks == false)
			ShotButton.gameObject.SetActive(Game.PlayerCharacter.IsArmed);
		if(Game.PlayerCharacter.IsDriving == false)
			EnterVehicleButton.gameObject.SetActive(Game.PlayerCharacter.ToEnterVehicle);
		
    }
    private void UpdateAxis() 
	{
		// >>> Joystick Movements
		MoveHorizontal = Input.GetAxis("Horizontal") + MovementJoystick.Horizontal();
		MoveVertical = Input.GetAxis("Vertical") + MovementJoystick.Vertical();

		MoveHorizontal = Mathf.Clamp(MoveHorizontal, -1,1);
		MoveVertical = Mathf.Clamp(MoveVertical, -1, 1);

		// >>> Mobile Driving Buttons

		if (ForwardButton.IsPressed)
			MoveVertical = 1;

		if (BackButton.IsPressed)
			MoveVertical = -1;

		if (RightButton.IsPressed)
			MoveHorizontal = 1;

		if (LeftButton.IsPressed)
			MoveHorizontal = -1;

		if (BrakeButton.IsPressed)
			PressedJump = true;

		//Rotate Screen
		if (Game.IsMobile)
		{
			RotateHorizontal = Touchfield.TouchDistance.x / 5 + ShotButtonTouchfield.TouchDistance.x / 5;
			RotateVertical = Touchfield.TouchDistance.y / 5 + ShotButtonTouchfield.TouchDistance.y / 5;
        }
        else
        {
			RotateHorizontal = Input.GetAxis("Mouse X");
			RotateVertical = Input.GetAxis("Mouse Y");
		}

		//Crouch
		if (Input.GetAxis("Crouch") < -0.9 && PressedCrouch == false)
		{
			PressedCrouch = true;
        }
        else
        {
			PressedCrouch = false;
        }
		if (Input.GetAxis("Crouch") > 0.9 && PressedCrouch == false)
		{
			PressedCrouchUp = true;
        }
        else
        {
			PressedCrouchUp = false;
        }

	}
    private void UpdateGetButtonDown()
	{
		// >>> Get Button Down

		//this check is so that when you touch the touch of the cell phone the player does not shoot or aim unwantedly.
		if (Game.IsMobile)
		{
			if (ShotButton.IsPressedDown)
			{
				PressedShootingDown = true;
			}
			else
			{
				PressedShootingDown = false;
			}

			if (AimingButton.IsPressedDown)
			{
				PressedAimingDown = true;
			}
			else
			{
				PressedAimingDown = false;
			}
		}
        else
        {
			if (Input.GetButtonDown("Fire1"))
			{
				PressedShootingDown = true;
			}
			else
			{
				PressedShootingDown = false;
			}

			if (Input.GetButtonDown("Fire2"))
			{
				PressedAimingDown = true;
			}
			else
			{
				PressedAimingDown = false;
			}
		}

		if (Input.GetButtonDown("Reload") || ReloadButton.IsPressedDown)
		{
			PressedJumpDown = true;
		}
		else
		{
			PressedJumpDown = false;
		}

		if (Input.GetButtonDown("Jump") || JumpButton.IsPressedDown)
		{
			PressedJumpDown = true;
		}
		else
		{
			PressedJumpDown = false;
		}

		if (Input.GetButtonDown("Run") || RunButton.IsPressedDown || RunButtonRight.IsPressedDown)
		{
			PressedRunDown = true;
		}
		else
		{
			PressedRunDown = false;
		}

		if (Input.GetButtonDown("Roll") || RollButton.IsPressedDown)
		{
			PressedRollDown = true;
		}
		else
		{
			PressedRollDown = false;
		}

		if (Input.GetButtonDown("Crouch") || CrouchButton.IsPressedDown)
		{
			PressedCrouchDown = true;
		}
		else
		{
			PressedCrouchDown = false;
		}

		if (Input.GetButtonDown("Reload") || ReloadButton.IsPressedDown)
		{
			PressedReloadDown = true;
		}
		else
		{
			PressedReloadDown = false;
		}

		if (Input.GetButtonDown("Interact") || PickWeaponButton.IsPressedDown)
		{
			PressedPickupWeaponDown = true;
		}
		else
		{
			PressedPickupWeaponDown = false;
		}

		if (Input.GetButtonDown("Interact") || EnterVehicleButton.IsPressedDown)
		{
			PressedEnterVehicleDown = true;
		}
		else
		{
			PressedEnterVehicleDown = false;
		}

		if (Input.GetButtonDown("Next") || NextWeaponButton.IsPressedDown)
		{
			PressedNextWeaponDown = true;
		}
		else
		{
			PressedNextWeaponDown = false;
		}

		if (Input.GetButtonDown("Previous") || PreviousWeaponButton.IsPressedDown)
		{
			PressedPreviousWeaponDown = true;
		}
		else
		{
			PressedPreviousWeaponDown = false;
		}
	}
	private void UpdateGetButton()
    {
		// >>> Get Button 
		//this check is so that when you touch the touch of the cell phone the player does not shoot or aim unwantedly.
		if (Game.IsMobile)
		{
			if (ShotButton.IsPressed)
			{
				PressedShooting = true;
			}
			else
			{
				PressedShooting = false;
			}

			if (AimingButton.IsPressedDown)
			{
				PressedAiming = true;
			}
			else
			{
				PressedAiming = false;
			}
		}
        else
        {
			if (Input.GetButton("Fire1"))
			{
				PressedShooting = true;
			}
			else
			{
				PressedShooting = false;
			}

			if (Input.GetButton("Fire2"))
			{
				PressedAiming = true;
			}
			else
			{
				PressedAiming = false;
			}
		}

		if (Input.GetButton("Reload") || ReloadButton.IsPressed)
		{
			PressedJump = true;
		}
		else
		{
			PressedJump = false;
		}

		if (Input.GetButton("Jump") || JumpButton.IsPressed)
		{
			PressedJump = true;
		}
		else
		{
			PressedJump = false;
		}

		if (Input.GetButton("Run") || RunButton.IsPressed || RunButtonRight.IsPressed)
		{
			PressedRun = true;
		}
		else
		{
			PressedRun = false;
		}

		if (Input.GetButton("Roll") || RollButton.IsPressed)
		{
			PressedRoll = true;
		}
		else
		{
			PressedRoll = false;
		}

		if (Input.GetButton("Crouch") || CrouchButton.IsPressed)
		{
			PressedCrouch = true;
		}
		else
		{
			PressedCrouch = false;
		}

		if (Input.GetButton("Reload") || ReloadButton.IsPressed)
		{
			PressedReload = true;
		}
		else
		{
			PressedReload = false;
		}

		if (Input.GetButton("Interact") || PickWeaponButton.IsPressed)
		{
			PressedPickupWeapon = true;
		}
		else
		{
			PressedPickupWeapon = false;
		}

		if (Input.GetButton("Interact") || EnterVehicleButton.IsPressed)
		{
			PressedEnterVehicle = true;
		}
		else
		{
			PressedEnterVehicle = false;
		}

		if (Input.GetButton("Next") || NextWeaponButton.IsPressed)
		{
			PressedNextWeapon = true;
		}
		else
		{
			PressedNextWeapon = false;
		}
		if (Input.GetButton("Previous") || PreviousWeaponButton.IsPressed)
		{
			PressedPreviousWeapon = true;
		}
		else
		{
			PressedPreviousWeapon = false;
		}
	}
	private void UpdateGetButtonUp()
    {
		// >>> Get Button Up
		if (Game.IsMobile)
		{
			if (ShotButton.IsPressedUp)
			{
				PressedShootingUp = true;
			}
			else
			{
				PressedShootingUp = false;
			}

			if (AimingButton.IsPressedUp)
			{
				PressedAimingUp = true;
			}
			else
			{
				PressedAimingUp = false;
			}
		}
		else
		{
			if (Input.GetButtonUp("Fire1"))
			{
				PressedShootingUp = true;
			}
			else
			{
				PressedShootingUp = false;
			}

			if (Input.GetButtonUp("Fire2"))
			{
				PressedAimingUp = true;
			}
			else
			{
				PressedAimingUp = false;
			}
		}

		if (Input.GetButtonUp("Reload") || ReloadButton.IsPressedUp)
		{
			PressedJumpUp = true;
		}
		else
		{
			PressedJumpUp = false;
		}

		if (Input.GetButtonUp("Jump") || JumpButton.IsPressedUp)
		{
			PressedJumpUp = true;
		}
		else
		{
			PressedJumpUp = false;
		}

		if (Input.GetButtonUp("Run") || RunButton.IsPressedUp || RunButtonRight.IsPressedUp)
		{
			PressedRunUp = true;
		}
		else
		{
			PressedRunUp = false;
		}

		if (Input.GetButtonUp("Roll") || RollButton.IsPressedUp)
		{
			PressedRollUp = true;
		}
		else
		{
			PressedRollUp = false;
		}

		if (Input.GetButtonUp("Crouch") || CrouchButton.IsPressedUp)
		{
			PressedCrouchUp = true;
		}
		else
		{
			PressedCrouchUp = false;
		}

		if (Input.GetButtonUp("Reload") || ReloadButton.IsPressedUp)
		{
			PressedReloadUp = true;
		}
		else
		{
			PressedReloadUp = false;
		}

		if (Input.GetButtonUp("Interact") || PickWeaponButton.IsPressedUp)
		{
			PressedPickupWeaponUp = true;
		}
		else
		{
			PressedPickupWeaponUp = false;
		}

		if (Input.GetButtonUp("Interact") || EnterVehicleButton.IsPressedUp)
		{
			PressedEnterVehicleUp = true;
		}
		else
		{
			PressedEnterVehicleUp = false;
		}

		if (Input.GetButtonUp("Next") || NextWeaponButton.IsPressedUp)
		{
			PressedNextWeaponUp = true;
		}
		else
		{
			PressedNextWeaponUp = false;
		}
		if (Input.GetButtonUp("Previous") || PreviousWeaponButton.IsPressedUp)
		{
			PressedPreviousWeaponUp = true;
		}
		else
		{
			PressedPreviousWeaponUp = false;
		}
	}
}
namespace JU_INPUT_SYSTEM{
	public class JUInput
	{
		static JUInputSystem InputButtons;
		static void GetJUInputInstance()
		{
			if (InputButtons != null) return;
			InputButtons = GameObject.FindObjectOfType<JUInputSystem>();
		}
		public enum Axis { MoveHorizontal, MoveVertical, RotateHorizontal, RotateVertical} 
		public enum Buttons{ ShotButton, AmingButton, JumpButton, RunButton,
							   RollButton, CrouchButton, ReloadButton,
							   PickupButton, EnterVehicleButton, PreviousWeaponButton, NextWeaponButton }

		public static float GetAxis(Axis axis)
		{
			GetJUInputInstance();
			switch (axis)
			{
				case Axis.MoveHorizontal:
					return InputButtons.MoveHorizontal;

				case Axis.MoveVertical:
					return InputButtons.MoveVertical;

				case Axis.RotateHorizontal:
					return InputButtons.RotateHorizontal;

				case Axis.RotateVertical:
					return InputButtons.RotateVertical;

				default:
					return 0;
			}
		}
		public static bool GetButtonDown(Buttons Button)
        {
			GetJUInputInstance();
            switch(Button){
				case Buttons.ShotButton:
					return InputButtons.PressedShootingDown;

				case Buttons.AmingButton:
					return InputButtons.PressedAimingDown;

				case Buttons.JumpButton:
					return InputButtons.PressedJumpDown;

				case Buttons.RunButton:
					return InputButtons.PressedRunDown;

				case Buttons.RollButton:
					return InputButtons.PressedRollDown;

				case Buttons.CrouchButton:
					return InputButtons.PressedCrouchDown;

				case Buttons.ReloadButton:
					return InputButtons.PressedReloadDown;

				case Buttons.PickupButton:
					return InputButtons.PressedPickupWeaponDown;

				case Buttons.EnterVehicleButton:
					return InputButtons.PressedEnterVehicleDown;

				case Buttons.PreviousWeaponButton:
					return InputButtons.PressedPreviousWeaponDown;

				case Buttons.NextWeaponButton:
					return InputButtons.PressedNextWeaponDown;


				default:
					return false;

            }
        }
		public static bool GetButton(Buttons Button)
		{
			GetJUInputInstance();
			switch (Button)
			{
				case Buttons.ShotButton:
					return InputButtons.PressedShooting;

				case Buttons.AmingButton:
					return InputButtons.PressedAiming;

				case Buttons.JumpButton:
					return InputButtons.PressedJump;

				case Buttons.RunButton:
					return InputButtons.PressedRun;

				case Buttons.RollButton:
					return InputButtons.PressedRoll;

				case Buttons.CrouchButton:
					return InputButtons.PressedCrouch;

				case Buttons.ReloadButton:
					return InputButtons.PressedReload;

				case Buttons.PickupButton:
					return InputButtons.PressedPickupWeapon;

				case Buttons.EnterVehicleButton:
					return InputButtons.PressedEnterVehicle;

				case Buttons.PreviousWeaponButton:
					return InputButtons.PressedPreviousWeapon;

				case Buttons.NextWeaponButton:
					return InputButtons.PressedNextWeapon;


				default:
					return false;

			}
		}
		public static bool GetButtonUp(Buttons Button)
		{
			GetJUInputInstance();
			switch (Button)
			{
				case Buttons.ShotButton:
					return InputButtons.PressedShootingUp;

				case Buttons.AmingButton:
					return InputButtons.PressedAimingUp;

				case Buttons.JumpButton:
					return InputButtons.PressedJumpUp;

				case Buttons.RunButton:
					return InputButtons.PressedRunUp;

				case Buttons.RollButton:
					return InputButtons.PressedRollUp;

				case Buttons.CrouchButton:
					return InputButtons.PressedCrouchUp;

				case Buttons.ReloadButton:
					return InputButtons.PressedReloadUp;

				case Buttons.PickupButton:
					return InputButtons.PressedPickupWeaponUp;

				case Buttons.EnterVehicleButton:
					return InputButtons.PressedEnterVehicleUp;

				case Buttons.PreviousWeaponButton:
					return InputButtons.PressedPreviousWeaponUp;

				case Buttons.NextWeaponButton:
					return InputButtons.PressedNextWeaponUp;


				default:
					return false;

			}
		}


	}




}
