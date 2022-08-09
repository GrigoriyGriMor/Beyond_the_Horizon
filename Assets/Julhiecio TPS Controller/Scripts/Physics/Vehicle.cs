using UnityEngine;
using UnityEditor;
using JU_INPUT_SYSTEM;

[AddComponentMenu("JU TPS/Physics/Vehicle")]
[RequireComponent(typeof(Rigidbody))]
public class Vehicle : MonoBehaviour
{
	ThirdPersonController pl;

	private float _horizontal;
	private float _vertical;
	private bool _break;

	[Header("Vehicle Settings")]
	public VehicleType TypeOfVehicle;
	public GameObject SteeringWheel;
	public float MaxVelocity = 60;
	public float TorqueForce = 2000;
	public float BreakForce = 8000;
	public float Friction = 200;
	public float AntiRoll = 50000;
	public float WheelRotationVelocity = 5f;
	public float WheelAngleMax = 45;

	[Header("Motorcycle Settings")]
	public float MotocycleAngleMax = 30;
	public Transform MotorcycleBodyPivot;
	public Transform MotorcycleRotZ;

	[HideInInspector] float RotZ;
	[HideInInspector] float SmothRotZ;
	[HideInInspector] float SmothSteeringWheel;
	[HideInInspector] float smoothYrotation;


	[Header("IK and Player Settings")]
	public Transform PlayerLocation;
	public Transform LeftHandPositionIK, RightHandPositionIK, LeftFootPositionIK, RightFootPositionIK;

	[Header("Physics Settings")]
	public bool AirControl;
	public float InAirForce;
	public LayerMask WhatIsGround;
	public Transform CenterOfMassPosition;
	public WheelCollider[] WheelColliders;
	public Transform[] Wheels;

	public bool KillPlayerWhenHitsTooHard;
	public float VelocityToKillPLayer = 30;

	[HideInInspector]public Rigidbody rb;
	private RaycastHit hit;

	[Header("Overturned Check")]
	public bool CheckOverturned;
	public Vector3 OverturnedCheckOfsset;
	public Vector3 OverturnedCheckScale;

	public bool IsOverturned;




	[Header("Vehicle Stats")]
	public bool IsOn;
	public bool OnFloor;
	public bool ControllRotation;

	[Header("Mesh Collider Correction")]
	public bool GenerateOnStart;
	public GameObject VehicleMesh;
	private MeshCollider MeshColliderCorrect;

	public enum VehicleType{
		Car,
		Motorcycle,
	}

    void Start()
    {
		//IsMobile = FindObjectOfType<GameManagerAndUI>().IsMobile;

		pl = FindObjectOfType<ThirdPersonController>();
		if (MeshColliderCorrect != null)
		{
			MeshColliderCorrect.transform.SetParent(null);
		}
		rb = GetComponent<Rigidbody> ();
		rb.centerOfMass = CenterOfMassPosition.localPosition;
		if(TypeOfVehicle == VehicleType.Motorcycle && MotorcycleBodyPivot != null)
        {
			MotorcycleBodyPivot.SetParent(null);
        }
        //if (IsMobile)
        //{
		//	LeftButton = GameObject.Find("LeftButton").GetComponent<ButtonVirtual>();
		//	RightButton = GameObject.Find("RightButton").GetComponent<ButtonVirtual>();
		//	BackButon = GameObject.Find("BackButton").GetComponent<ButtonVirtual>();
		//	ForwardButton = GameObject.Find("ForwardButton").GetComponent<ButtonVirtual>();
		//	BreakButton = GameObject.Find("BreakButton").GetComponent<ButtonVirtual>();
		//}

		if (GenerateOnStart)
			GenerateMeshColliderCorrection();
	}
    void FixedUpdate()
    {
		UpdateMeshColliderCorrection();

		//---MOTORCYCLE VEHICLE TYPE---
		if (TypeOfVehicle == VehicleType.Motorcycle) {
			//If vehicle is on you can controll
			ControllMotorcycle ();
			VehicleControl();
		}
		//-------CAR VEHICLE TYPE------
		if (TypeOfVehicle == VehicleType.Car) {
			AntiOverturned ();
			VehicleControl();
		}

	}
    private void Update()
    {
		//Ground Check
		if (Physics.Raycast(CenterOfMassPosition.position, -transform.up, out hit, 5, WhatIsGround))
		{
			OnFloor = true;
			if(hit.collider.tag == "Loop")
            {
				ControllRotation = false;
            }
            else
            {
				ControllRotation = true;
			}
		}
		else
		{
			ControllRotation = true;
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation, 0.2f * Time.deltaTime);
			OnFloor = false;
		}

		//Overturned check and correction
		Overturned();

		//Input Controller (Mobile and PC)
		InputController();
		UpdateWheelsMeshes();

	}

    Vector3 pos;
	Quaternion rot;
	public void VehicleControl(){
		if (rb.velocity.magnitude > MaxVelocity)
		{
			var rbvel = rb.velocity;
			rbvel.y = rb.velocity.y;
			rb.velocity = Vector3.ClampMagnitude(rbvel, MaxVelocity);
		}

		for (int i = 0; i < WheelColliders.Length; i++)
		{
			//Update wheels 
			WheelColliders [i].GetWorldPose (out pos, out rot);
			//Wheels[i].position = pos;
			//Wheels[i].rotation = rot;
			//Wheels [i].rotation = Quaternion.Lerp(Wheels[i].rotation, rot, 15 * Time.deltaTime);

			if (IsOn == true) {
				WheelColliders [i].brakeTorque = _break ? BreakForce : Friction - Mathf.Abs (_vertical * Friction);

				//If is a car
				if (TypeOfVehicle == VehicleType.Car){
					if (i < 2) {
						//Wheel Rotation
						if (_horizontal > 0.2f || _horizontal < -0.2f)
						{
							WheelColliders[i].steerAngle = Mathf.Lerp(WheelColliders[i].steerAngle, WheelAngleMax * _horizontal, WheelRotationVelocity * Time.deltaTime);
                        }
                        else
                        {
							WheelColliders[i].steerAngle = Mathf.Lerp(WheelColliders[i].steerAngle, WheelAngleMax * _horizontal, 4 * WheelRotationVelocity * Time.deltaTime);
						}
					}
					if(_vertical != 0)
						WheelColliders [i].motorTorque = _vertical * TorqueForce;
				}


				//If is a motorcycle
				if (TypeOfVehicle == VehicleType.Motorcycle)
				{
					//Wheel Rotation
					WheelColliders[0].steerAngle = Mathf.Lerp(WheelColliders[i].steerAngle, _horizontal * WheelAngleMax, WheelRotationVelocity * Time.deltaTime);
					WheelColliders[i].motorTorque = _vertical * TorqueForce;
				}

				if (SteeringWheel != null)
				{
					SmothSteeringWheel = Mathf.Lerp(SmothSteeringWheel, WheelColliders[i].steerAngle, WheelRotationVelocity * Time.deltaTime);
					SteeringWheel.transform.localEulerAngles = new Vector3(SteeringWheel.transform.localEulerAngles.x, SmothSteeringWheel, SteeringWheel.transform.localEulerAngles.x);
				}
            }
            else
            {
				WheelColliders[i].brakeTorque = BreakForce / 4;
			}
		}

        if (AirControl && IsOn)
        {
			rb.AddTorque(transform.up * _horizontal * 100 * InAirForce / 4f);
			rb.AddTorque(transform.right * _vertical * 100* InAirForce);
		}
	}
	public void UpdateWheelsMeshes()
	{
		for (int i = 0; i < WheelColliders.Length; i++)
		{
			//Update wheels 
			WheelColliders[i].GetWorldPose(out pos, out rot);
			Wheels[i].position = pos;
			Wheels[i].rotation = rot;
		}
	}
	public void ControllMotorcycle()
	{
		if (IsOn)
		{
			if (ControllRotation)
			{
				if (_vertical > 0 && _horizontal != 0f)
				{
					RotZ = Mathf.LerpAngle(RotZ, rb.velocity.normalized.magnitude * -_horizontal * MotocycleAngleMax, 8f * Time.deltaTime);
				}
				else
				{
					if (_vertical < 0)
					{
						RotZ = Mathf.LerpAngle(RotZ, rb.velocity.normalized.magnitude * -_horizontal * MotocycleAngleMax / 2, 3f * Time.deltaTime);
					}
					else
					{
						RotZ = Mathf.LerpAngle(RotZ, 0, 6f * Time.deltaTime);
					}
				}
            }
            else
            {
				RotZ = 0;
            }
			
				var rot = MotorcycleRotZ.localEulerAngles;
				rot.x = 0;
				rot.y = 0;
				rot.z = RotZ;

				MotorcycleRotZ.localEulerAngles = new Vector3(0, 0,RotZ);
				if (OnFloor)
				{
					MotorcycleBodyPivot.rotation = Quaternion.Slerp(MotorcycleBodyPivot.rotation, Quaternion.FromToRotation(MotorcycleBodyPivot.up, hit.normal) * MotorcycleBodyPivot.rotation, 5 * Time.deltaTime);
					MotorcycleBodyPivot.position = hit.point;
				}
				else
				{
					MotorcycleBodyPivot.rotation = Quaternion.Slerp(MotorcycleBodyPivot.rotation, Quaternion.FromToRotation(MotorcycleBodyPivot.up, Vector3.up) * MotorcycleBodyPivot.rotation, 0.8f * Time.deltaTime);
					MotorcycleBodyPivot.position = transform.position;
				}
				MotorcycleBodyPivot.eulerAngles = new Vector3(MotorcycleBodyPivot.eulerAngles.x, transform.eulerAngles.y, MotorcycleBodyPivot.eulerAngles.z);
				var motorcyclerot = MotorcycleBodyPivot.eulerAngles;
				motorcyclerot.z = MotorcycleRotZ.localEulerAngles.z;

				transform.rotation = Quaternion.Slerp(transform.rotation, MotorcycleRotZ.rotation, 3 * Time.deltaTime);
			
			//If is on floor -> freeze rotation
			if (OnFloor)
			{
				rb.angularDrag = 5;
				rb.constraints = RigidbodyConstraints.FreezeRotationX;
				rb.constraints = RigidbodyConstraints.FreezeRotationZ;
			}
			else
			{
				rb.angularDrag = 1f;
				rb.constraints = RigidbodyConstraints.None;
			}
		}
	}
	public void AntiOverturned(){

		WheelHit hit;
		var travelL = 1.0;
		var travelR = 1.0;

		var groundedL = WheelColliders[1].GetGroundHit(out hit);
		if (groundedL)
			travelL = (-WheelColliders[1].transform.InverseTransformPoint(hit.point).y - WheelColliders[1].radius) / WheelColliders[1].suspensionDistance;

		var groundedR = WheelColliders[0].GetGroundHit(out hit);
		if (groundedR)
			travelR = (-WheelColliders[0].transform.InverseTransformPoint(hit.point).y - WheelColliders[0].radius) / WheelColliders[0].suspensionDistance;

		var antiRollForceDouble = (travelL - travelR) * AntiRoll;
		float antiRollForce = (float)antiRollForceDouble;
		if (groundedL)
			rb.AddForceAtPosition(WheelColliders[1].transform.up * -antiRollForce,
				WheelColliders[1].transform.position);  
		if (groundedR)
			rb.AddForceAtPosition(WheelColliders[0].transform.up * antiRollForce,
				WheelColliders[0].transform.position);  
		
	}	
	public void InputController()
    {
		_horizontal = JUInput.GetAxis(JUInput.Axis.MoveHorizontal);
		_vertical = JUInput.GetAxis(JUInput.Axis.MoveVertical);

		// Vehicle Speed Up
		if (JUInput.GetButton(JUInput.Buttons.JumpButton))
			_vertical = 1;

		// Vehicle Brake
		if (JUInput.GetButton(JUInput.Buttons.NextWeaponButton) || Input.GetKey(KeyCode.Space))
		{
			_break = true;
		}
		else
		{
			_break = false;
		}
	
	}
	public void UpdateMeshColliderCorrection()
	{
		if (MeshColliderCorrect != null && VehicleMesh != null)
		{
			MeshColliderCorrect.transform.position = VehicleMesh.transform.position;
			MeshColliderCorrect.transform.rotation = VehicleMesh.transform.rotation;
		}
	}
	public void GenerateMeshColliderCorrection()
    {
		if(VehicleMesh != null)
        {
			if (MeshColliderCorrect == null)
			{
				var newmeshcollider = (GameObject)Instantiate(VehicleMesh, VehicleMesh.transform.position, VehicleMesh.transform.rotation);

				//Destroy all childs
				var childs = newmeshcollider.GetComponentsInChildren<Transform>();
				foreach (Transform child in childs)
				{
					if(child.transform != newmeshcollider.transform)
						Destroy(child.gameObject);
				}

				newmeshcollider.transform.SetParent(null);

				//destroy existing collider
				if (newmeshcollider.TryGetComponent(out Collider collider) != false)
				{
					DestroyImmediate(collider.GetComponent<Collider>());
				}
				//add mesh collider
				newmeshcollider.AddComponent<MeshCollider>();

				//set mesh collider
				newmeshcollider.GetComponent<MeshCollider>().convex = false;
				newmeshcollider.GetComponent<MeshCollider>().sharedMesh = VehicleMesh.GetComponent<MeshFilter>().sharedMesh;
				MeshColliderCorrect = newmeshcollider.GetComponent<MeshCollider>();

				//add mesh collider correct script and set mesh
				newmeshcollider.AddComponent<MeshColliderCorrect>();
				newmeshcollider.GetComponent<MeshColliderCorrect>().Mesh = VehicleMesh;

				//set new mesh collider properties
				newmeshcollider.name = "[Mesh Collider Correct]";
				newmeshcollider.layer = 12; //Vehicle Mesh Collider

				//Destroy mesh renderer
				if (newmeshcollider.TryGetComponent(out MeshFilter meshfilter) != false)
				{
					DestroyImmediate(meshfilter.GetComponent<MeshRenderer>());
					DestroyImmediate(meshfilter.GetComponent<MeshFilter>());
				}

			}
		}
        else
        {
			Debug.LogError("Mesh Collider Correct Generate: need a linked mesh");
        }
    }
	public void Overturned()
	{
		if (CheckOverturned == true)
		{
			var overturned_check = Physics.OverlapBox(transform.position + transform.up * OverturnedCheckOfsset.y 
				+ transform.right * OverturnedCheckOfsset.x + transform.forward * OverturnedCheckOfsset.z,
				OverturnedCheckScale, transform.rotation, WhatIsGround);

			if (transform.eulerAngles.z > 45 || transform.eulerAngles.z < -45)
			{
				if (overturned_check.Length != 0)
				{
					IsOverturned = true;
				}
            }
            else
            {
				IsOverturned = false;
            }

			if (IsOverturned == true)
			{
				/*var rot = transform.eulerAngles;
				rot.z = Mathf.Lerp(rot.z, 0, 10 * Time.deltaTime);
				transform.eulerAngles = rot;*/
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation, 0.3f * Time.deltaTime);
			}
		}
	}
    private void OnCollisionEnter(Collision collision)
    {
		if (IsOn == false)
			return;
        if(transform.InverseTransformDirection(rb.velocity).z > VelocityToKillPLayer && KillPlayerWhenHitsTooHard)
        {
			pl.Health = 0;
        }
    }
	private void OnDrawGizmos()
	{
		if (CheckOverturned)
			Gizmos.color = Color.white;
			Gizmos.DrawWireCube(transform.position + transform.up * OverturnedCheckOfsset.y + transform.right * OverturnedCheckOfsset.x + transform.forward * OverturnedCheckOfsset.z, OverturnedCheckScale);
	}
}
