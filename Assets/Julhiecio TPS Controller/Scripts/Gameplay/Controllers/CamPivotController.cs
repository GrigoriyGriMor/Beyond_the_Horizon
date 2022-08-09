using UnityEngine;
using JU_INPUT_SYSTEM;
[AddComponentMenu("JU TPS/Gameplay/Third Person System/Camera Controller")]
public class CamPivotController : MonoBehaviour
{
	[Header("Camera Target")]
	public ThirdPersonController PlayerTarget;
	private Transform SpineTarget;

	[HideInInspector] public Camera mCamera;
	[HideInInspector] private float StartCameraFOV;
	[Header("Settings")]
	public bool IsMobile;
	private Touchfield touchfield;
	private Touchfield touch_shootbutton;

	public float MovementSpeed = 25;
	public bool CameraShakeWhenShooting = true;

	[Range(0, 2)] public float CameraShakeSensibility = 0.5f;

	public LayerMask CameraCollisionLayer;
	public LayerMask CameraCollisionDrivingLayer;

	public float RotationSpeed = 5f;
	public float Distance = 3f;
	public float IsArmedDistance = 1f;

	[HideInInspector]
	float ActualDistance = 5;

	[Header("Limit FPS")]
	public int FPS_Limit = 60;

	[Header("Camera Position Adjustment")]
	public float TargetHeight = 0;
	float StartTargetHeight;
	public float X_adjust = 0.6f;
	public float Y_adjust = 0.45f;

	public bool Aiming;


	[Header("Camera Position Driving Adjustment")]
	public float DistanceCarDriving = 7;
	public float DistanceMotocycleDriving = 5;

	public float TargetHeightDriving = 1.3f;
	public float X_adjustDriving = 0f;
	public float Y_adjustDriving = 0;
	public float Z_adjustDriving = 1;

	public bool VehicleCameraAutoRotation;
	private float CurrentTimeToAutoRotation;

	[Header("Camera Rotation Limit")]
	public float MinRotation = -80f;
	public float MaxRotation = 80f;

	//Camera Rotation Axis
	[HideInInspector]
	public float rotX;
	[HideInInspector]
	public float rotY;
	float xmouse;
	float ymouse;
	[HideInInspector]
	public float rotxtarget;

	[Header("Slowmotion Settings")]
	public bool EnableSlowmotion = true;
	[HideInInspector] public float SlowDownFactor = 0.05f;
	[HideInInspector] public float SlowDownLenght = 1;


    private void OnEnable()
    {
		mCamera = gameObject.GetComponentInChildren<Camera>();
	}
	void Start() {
		mCamera = gameObject.GetComponentInChildren<Camera>();
		StartCameraFOV = mCamera.fieldOfView;

		IsMobile = FindObjectOfType<GameManagerAndUI>().IsMobile;

		StartTargetHeight = TargetHeight;

		if (PlayerTarget == null)
		{
			PlayerTarget = FindObjectOfType<ThirdPersonController>();
		}
		if (PlayerTarget != null)
		{
			rotY = PlayerTarget.transform.eulerAngles.y;
			SpineTarget = PlayerTarget.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Spine);
		}
		if (IsMobile)
		{
			//Find Touch
			touchfield = GameObject.Find("Touch").GetComponent<Touchfield>();
			touch_shootbutton = GameObject.Find("ShotButton").GetComponent<Touchfield>();
		}
		else
		{
			///Lock Mouse
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		Application.targetFrameRate = FPS_Limit;
	}
	void LateUpdate()
	{
		//CAMERA POSITION ADJUST
		if (PlayerTarget.IsDriving == false && Aiming == false)
		{
			mCamera.transform.position = transform.position - transform.forward * ActualDistance + transform.right * X_adjust + transform.up * Y_adjust;
		}

		//CAMERA AIMING
		Aiming = PlayerTarget.IsAiming;
        if (Aiming && PlayerTarget.WeaponInUse.AimMode != Weapon.WeaponAimMode.None)
        {
			var gun = PlayerTarget.WeaponInUse;
			mCamera.transform.position = gun.transform.position
				+ gun.transform.right * gun.CameraAimingPosition.x
				+ gun.transform.up * gun.CameraAimingPosition.y
				+ transform.forward * gun.CameraAimingPosition.z;

			mCamera.fieldOfView = Mathf.Lerp(mCamera.fieldOfView,gun.CameraFOV, 15 * Time.deltaTime);
        }
        else
        {
			mCamera.fieldOfView = Mathf.Lerp(mCamera.fieldOfView, StartCameraFOV, 10 * Time.deltaTime);
		}

		//DEAD CAMERA
		if (PlayerTarget.IsDead == true)
		{
			transform.position = PlayerTarget.HumanoidSpine.position;

			mCamera.transform.position = PlayerTarget.HumanoidSpine.position - transform.forward * Distance + transform.right;

			mCamera.transform.LookAt(PlayerTarget.HumanoidSpine.position);
		}


		//SMOTH CAMERA DISTANCE
		if (PlayerTarget.IsArmed)
		{
			ActualDistance = Mathf.Lerp(ActualDistance, IsArmedDistance, 5 * Time.deltaTime);
		}
		else
		{
			ActualDistance = Mathf.Lerp(ActualDistance, Distance, 5 * Time.deltaTime);
		}

		

		//DRIVING POSITION ADJUSTMENT
		if (PlayerTarget.IsDriving == true)
		{
			//DRIVING DISTANCE ADJUSTMENT
			if (PlayerTarget.VehicleInArea.TypeOfVehicle == Vehicle.VehicleType.Car)
			{
				mCamera.transform.position = transform.position - transform.forward * DistanceCarDriving + transform.right * X_adjustDriving + transform.up * Y_adjustDriving;
			}
			else
			{
				mCamera.transform.position = transform.position - transform.forward * DistanceMotocycleDriving + transform.right * X_adjustDriving + transform.up * Y_adjustDriving;
			}
		}

		//CAMERA COLLISION
		RaycastHit hit;
		if (PlayerTarget.IsDriving == false)
		{
			if (Physics.Linecast(transform.position + transform.right * X_adjust / 2 + transform.up * Y_adjust / 2, mCamera.transform.position, out hit, CameraCollisionLayer))
			{
				mCamera.transform.position = hit.point + hit.normal * 0.05f;
			}
		}
		else
		{
			if (Physics.Linecast(transform.position, mCamera.transform.position, out hit, CameraCollisionDrivingLayer))
			{
				mCamera.transform.position = hit.point + hit.normal * 0.05f;
			}
		}
	}
    public void Update()
    {
		//CAMERA ROTATION
		if (PlayerTarget.IsDead == false)
		{
			if (Aiming == false)
			{
				xmouse = 100 * JUInput.GetAxis(JUInput.Axis.RotateVertical) * Time.deltaTime;
				ymouse = 100 * JUInput.GetAxis(JUInput.Axis.RotateHorizontal) * Time.deltaTime;
			}
			else
			{
				xmouse = 50 * JUInput.GetAxis(JUInput.Axis.RotateVertical) * Time.deltaTime;
				ymouse = 50 * JUInput.GetAxis(JUInput.Axis.RotateHorizontal) * Time.deltaTime;
			}

			rotxtarget -= xmouse * RotationSpeed;
			rotxtarget = Mathf.Clamp(rotxtarget, MinRotation, MaxRotation);

			rotX = Mathf.Lerp(rotX, rotxtarget, 30 * Time.fixedDeltaTime);
			rotY += ymouse * RotationSpeed;

			var rot = new Vector3(rotX, rotY, 0);
			transform.eulerAngles = rot;

			//VEHICLE CAMERA AUTO ROTATION
			if (PlayerTarget.IsDriving)
			{
				if (xmouse != 0 || ymouse != 0)
				{
					CurrentTimeToAutoRotation = 0;
					VehicleCameraAutoRotation = false;
				}
				if (VehicleCameraAutoRotation == true) 
				{
					rotY = Mathf.LerpAngle(rotY, PlayerTarget.VehicleInArea.transform.eulerAngles.y, 2*Time.deltaTime);
					rotxtarget = Mathf.LerpAngle(rotxtarget, PlayerTarget.VehicleInArea.transform.eulerAngles.x, 5 * Time.deltaTime);
                }
                else
                {
					CurrentTimeToAutoRotation += Time.deltaTime;
					if (CurrentTimeToAutoRotation >= 3)
					{
						VehicleCameraAutoRotation = true;
						CurrentTimeToAutoRotation = 0;
					}
                }
			}

		}

		//Slowmotion
		if (EnableSlowmotion)
		{
			Time.timeScale += (1f / SlowDownLenght) * Time.unscaledDeltaTime;
			Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);

			if (Input.GetKeyUp(KeyCode.L)) DoSlowMotion(0.1f,10f);
		}
	}
	void FixedUpdate()
    {
		if(PlayerTarget.IsDead == false)
		{
			if (PlayerTarget.IsDriving == false)
			{
				var pos = PlayerTarget.HumanoidSpine.transform.position + PlayerTarget.transform.up * TargetHeight;
				transform.position = Vector3.Lerp(transform.position, pos, MovementSpeed * Time.fixedDeltaTime);
            }
            else
            {
				var pos = PlayerTarget.VehicleInArea.transform.position + PlayerTarget.transform.up * Y_adjustDriving + PlayerTarget.VehicleInArea.transform.forward * Z_adjustDriving;
				transform.position = Vector3.Lerp(transform.position, pos, MovementSpeed * Time.fixedDeltaTime);
			}
		}
	}


	public void DoSlowMotion(float timescale, float duration)
	{
		SlowDownFactor = timescale;
		SlowDownLenght = duration;
		Time.timeScale = timescale;
		Time.fixedDeltaTime = Time.timeScale * .02f;
		Invoke("DisableSlowmotion", 0.4f * duration);
	}
	public void Shake(float Strenght)
    {
		if (CameraShakeWhenShooting)
		{
			rotX += CameraShakeSensibility * Strenght;
		}
    }
	public void DisableSlowmotion()
    {
		SlowDownFactor = 1;
		SlowDownLenght = 1;
		Time.timeScale = 1;
		Time.fixedDeltaTime = Time.timeScale * .02f;
    }
	void OnDrawGizmos(){
		if (mCamera == null)
		{
			mCamera = gameObject.GetComponentInChildren<Camera>(); return;
		}
		else
		{
			var poscam = mCamera.transform.position;
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, poscam);
			Gizmos.DrawSphere(poscam, 0.1f);
		}
	}
}