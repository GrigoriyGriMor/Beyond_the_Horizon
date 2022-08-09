using UnityEngine;
using UnityEditor;

[AddComponentMenu("JU TPS/Gameplay/Weapon System/Weapon")]
[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
	public LayerMask CrosshairHitMask;
	public WeaponAimRotationCenter WeaponAimPositions;
	public CamPivotController MyPivotCamera;
	public Transform MyCamera;

	[Header("Weapon Setting")]
	public string WeaponName;
	public int WeaponSwitchID;
	public int WeaponPositionID;
	public bool Unlocked;

	//Bullets
	[Range(1,200)]
	public int BulletsPerMagazine = 10;
	public int TotalBullets = 150;
	public int BulletsAmounts = 10;

	//Fire Rate
	public float Fire_Rate = 0.3f;
	public float CurrentFireRateToShoot;



	//Precision
	[Range(0.1f, 50f)]
	public float Precision = 0.5f;
	[Range(0.01f, 1f)]
	public float LossOfAccuracyPerShot = 1;

	public float ShotErrorProbability;



	//Shooting 
	public GameObject BulletPrefab;
	public GameObject MuzzleFlashParticlePrefab;
	public Transform Shoot_Position;
	
	//Aim Mode
	public WeaponFireMode FireMode;
	public WeaponAimMode AimMode;
	public Vector3 CameraAimingPosition = new Vector3(0,0.1f,-0.2f);
	public Sprite ScopeTexture;
	public float CameraFOV = 30f;

	//Shooting States
	public bool CanShoot;
	public bool IsShooted;

	[Header("Procedural Animation")]
	public bool GenerateProceduralAnimation = true;
	[Range(0f, 0.3f)]
	public float RecoilForce = 0.1f;
	[Range(0f, 100)]
	public float RecoilForceRotation = 20;
	public Transform GunSlider;
	[Range(0f, 0.1f)]
	public float SliderMovementOffset;
	[HideInInspector] public Vector3 SliderStartLocalPosition;


	[Header("Bullet Casing Emitter")]
	public GameObject BulletCasingPrefab;


	[Header("IK Settings")]
	public Transform IK_Position_LeftHand;


	[Header("Weapon Sounds")]
	public AudioClip ShootAudio;
	public AudioClip ReloadAudio;
	

	[Header("Gizmo View")]
	public GizmosSettings GizmosVisualizationSettings = new GizmosSettings();

	public enum WeaponFireMode { Auto, SemiAuto, BoltAction, Shotgun }
	public enum WeaponAimMode { None,CameraApproach, Scope}

	private void Start()
	{
		MyPivotCamera = FindObjectOfType<CamPivotController>();
		MyCamera = MyPivotCamera.mCamera.transform;
		WeaponAimPositions = FindObjectOfType<WeaponAimRotationCenter>();

		CrosshairHitMask = FindObjectOfType<ThirdPersonController>().CrosshairHitMask;

		if (GunSlider != null)
			SliderStartLocalPosition = GunSlider.localPosition;

		CurrentFireRateToShoot = Fire_Rate;
	}
    private void Update()
    {
		WeaponControl();

		if (GenerateProceduralAnimation == false) return;
		ProceduralAnimation();
    }
    // >>> Control
    private void WeaponControl()
	{
		// >>> Fire Rate Control
		if (CanShoot == false)
		{
			CurrentFireRateToShoot += Time.deltaTime;
			if (CurrentFireRateToShoot >= Fire_Rate)
			{
				CurrentFireRateToShoot = 0;
				CanShoot = true;
				IsShooted = false;
			}
		}
		// >>> Blocks the shot when bullets == 0
		if (BulletsAmounts == 0)
		{
			CanShoot = false;
			CurrentFireRateToShoot = 0;
		}

		// >>> Weapon Accuracy Control
		ShotErrorProbability = Mathf.Lerp(ShotErrorProbability, 0, Precision * Time.deltaTime);	
	}
	private void ProceduralAnimation()
	{
		// >>> Recoil Animation
		{
			//Get stored transform properties
			Vector3 stored_weapon_pos = WeaponAimPositions._storedLocalPositions[WeaponPositionID];
			Quaternion stored_weapon_rot = WeaponAimPositions._storedLocalRotations[WeaponPositionID];

			//Set transform position smoothed
			WeaponAimPositions.WeaponPositionTransform[WeaponPositionID].localPosition = Vector3.Lerp(
				WeaponAimPositions.WeaponPositionTransform[WeaponPositionID].localPosition, stored_weapon_pos, 20 * Time.deltaTime);
			//Set transform rotation smoothed
			WeaponAimPositions.WeaponPositionTransform[WeaponPositionID].localRotation = Quaternion.Lerp(
				WeaponAimPositions.WeaponPositionTransform[WeaponPositionID].localRotation, stored_weapon_rot, 20 * Time.deltaTime);
		}


		// >>> Weapon Slider/Bolt Animation
		if (GunSlider != null && BulletsAmounts > 0 && FireMode != WeaponFireMode.BoltAction)
		{
			GunSlider.transform.localPosition = Vector3.MoveTowards(GunSlider.transform.localPosition, SliderStartLocalPosition, 0.35f * Time.deltaTime);
		}
	}

	// >>> Actions
	public void PullTrigger()
    {
		// >>> FireModes
		if (CanShoot)
        {
			Shot();
			IsShooted = true;
        }
    }
	public void Shot()
	{
		RaycastHit CrosshairHit;

		if (FireMode != Weapon.WeaponFireMode.Shotgun)
		{
			//Generate Random Direction
			var BulletRotationPrecision = MyCamera.transform.forward;
			BulletRotationPrecision.x += Random.Range(-ShotErrorProbability / 2, ShotErrorProbability / 2);
			BulletRotationPrecision.y += Random.Range(-ShotErrorProbability / 2, ShotErrorProbability / 2);
			BulletRotationPrecision.z += Random.Range(-ShotErrorProbability / 2, ShotErrorProbability / 2);

			//If raycast collide, apply random direction
			if (Physics.Raycast(MyCamera.transform.position + MyCamera.transform.forward * MyPivotCamera.Distance / 2, BulletRotationPrecision, out CrosshairHit, 500, CrosshairHitMask))
			{
				Shoot_Position.LookAt(CrosshairHit.point);
				Debug.DrawLine(MyCamera.transform.position + MyCamera.transform.forward * MyPivotCamera.Distance, CrosshairHit.point, Color.red);
				ShotErrorProbability = ShotErrorProbability + LossOfAccuracyPerShot;
			}
			else
			{
				Shoot_Position.rotation = MyCamera.transform.rotation;
				ShotErrorProbability = ShotErrorProbability + LossOfAccuracyPerShot;
			}
			//Spawn bullet
			var bullet = (GameObject)Instantiate(BulletPrefab, Shoot_Position.position, Shoot_Position.rotation);
			bullet.GetComponent<Bullet>().FinalPoint = CrosshairHit.point;
			bullet.GetComponent<Bullet>().DestroyBulletRotation = CrosshairHit.normal;
			Destroy(bullet, 10f);
			if (FireMode == WeaponFireMode.Auto || FireMode == WeaponFireMode.SemiAuto)
			{
				EmitBulletCasing();
			}
		}
		else
		{
			// >>> Shotgun Shoots
			for (int i = 0; i < 6; i++)
			{
				//Generate Random Direction
				var BulletRotationPrecision = MyCamera.transform.forward;
				BulletRotationPrecision.x += Random.Range(-LossOfAccuracyPerShot, LossOfAccuracyPerShot);
				BulletRotationPrecision.y += Random.Range(-LossOfAccuracyPerShot, LossOfAccuracyPerShot);
				BulletRotationPrecision.z += Random.Range(-LossOfAccuracyPerShot, LossOfAccuracyPerShot);
				ShotErrorProbability = ShotErrorProbability + 5 * LossOfAccuracyPerShot;

				//If raycast collide, apply random direction
				if (Physics.Raycast(MyCamera.transform.position + MyCamera.transform.forward * MyPivotCamera.Distance / 2, BulletRotationPrecision, out CrosshairHit, 500, CrosshairHitMask))
				{
					Shoot_Position.LookAt(CrosshairHit.point);
					//Debug.DrawLine(MyCamera.transform.position + MyCamera.transform.forward * MyPivotCamera.Distance, CrosshairHit.point, Color.red);

					//Spawn bullet
					var bullet = (GameObject)Instantiate(BulletPrefab, Shoot_Position.position, Shoot_Position.rotation);
					bullet.GetComponent<Bullet>().FinalPoint = CrosshairHit.point;
					bullet.GetComponent<Bullet>().DestroyBulletRotation = CrosshairHit.normal;
					Destroy(bullet, 10f);
				}
				else
				{
					//Generate Random Direction
					var BulletRotationPrecisionShootgun = MyCamera.transform.rotation;
					BulletRotationPrecisionShootgun.x += Random.Range(-LossOfAccuracyPerShot, LossOfAccuracyPerShot);
					BulletRotationPrecisionShootgun.y += Random.Range(-LossOfAccuracyPerShot, LossOfAccuracyPerShot);
					BulletRotationPrecisionShootgun.z += Random.Range(-LossOfAccuracyPerShot, LossOfAccuracyPerShot);
					BulletRotationPrecisionShootgun.w += Random.Range(-LossOfAccuracyPerShot, LossOfAccuracyPerShot);

					//Spawn bullet
					var bullet = (GameObject)Instantiate(BulletPrefab, Shoot_Position.position, BulletRotationPrecisionShootgun);
					Destroy(bullet, 10f);
				}
			}
		}

		//Update shooting state
		IsShooted = true;

		//Reset Shoot Direction
		Shoot_Position.rotation = MyCamera.transform.rotation;

		//Spawn Muzzle Flash
		var muzzleflesh = (GameObject)Instantiate(MuzzleFlashParticlePrefab, Shoot_Position.position, Shoot_Position.rotation, transform);
		Destroy(muzzleflesh, 2);

		//Reset Fire Rate
		CurrentFireRateToShoot = 0;
		CanShoot = false;

		//Subtracts Ammunition
		BulletsAmounts -= 1;

		//Procedural Animation Trigger
		if (GenerateProceduralAnimation == true)
		{
			//Slider Animation
			if (GunSlider != null)
			{
				Vector3 RecoilSliderPosition = new Vector3(SliderStartLocalPosition.x, SliderStartLocalPosition.y, SliderStartLocalPosition.z - SliderMovementOffset);
				GunSlider.localPosition = RecoilSliderPosition;
			}
			//Recoil Animation
			Invoke("WeaponRecoil", 0.06f);
		}

		//Audio
		if (ShootAudio != null)
		{
			GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
			GetComponent<AudioSource>().PlayOneShot(ShootAudio);
		}
	}
	public void EmitBulletCasing()
    {
		//Spawn Bullet Casing
		if (BulletCasingPrefab != null)
		{
			var bulletcasing = (GameObject)Instantiate(BulletCasingPrefab, GunSlider.position, transform.rotation);
			bulletcasing.hideFlags = HideFlags.HideInHierarchy;
			Destroy(bulletcasing, 5f);
		}
	}
	private void WeaponRecoil()
	{
		//Apply recoil
		WeaponAimPositions.WeaponPositionTransform[WeaponPositionID].Translate(0, 0, -RecoilForce);
		WeaponAimPositions.WeaponPositionTransform[WeaponPositionID].Rotate(0, -RecoilForceRotation, 0);
		MyPivotCamera.Shake(20 * RecoilForce);
	}
	public void Reload()
    {
		//Reload
		if (BulletsAmounts < BulletsPerMagazine)
		{
			if (TotalBullets >= BulletsPerMagazine)
			{
				BulletsAmounts = BulletsPerMagazine;
				TotalBullets -= BulletsPerMagazine;
			}
			else
			{
				BulletsAmounts = TotalBullets;
				TotalBullets = 0;
			}
		}
		//Play reloading audio
		GetComponent<AudioSource>().PlayOneShot(ReloadAudio);
	}
	
#if UNITY_EDITOR
    private void LoadVisualizationAssets() {        
        GizmosVisualizationSettings.ResourcesPath = Application.dataPath + "/Julhiecio TPS Controller/Editor/Editor Resources/GizmosModels/";
        var path_to_load = GizmosVisualizationSettings.ResourcesPath;
        path_to_load = FileUtil.GetProjectRelativePath(path_to_load);
        
        GizmosVisualizationSettings.HandVisualizerWireMesh =
         AssetDatabase.LoadAssetAtPath<MeshFilter>(path_to_load + "Hand Visualizer Wire Model.fbx").sharedMesh;
        GizmosVisualizationSettings.HandVisualizerMesh =
         AssetDatabase.LoadAssetAtPath<MeshFilter>(path_to_load + "Hand Visualizer Model.fbx").sharedMesh;

		print("Loaded Visualization Meshes");
    }

	void OnDrawGizmos(){
		float mscale = .1f;
		if (GizmosVisualizationSettings.HandVisualizerMesh == null)
		{
			LoadVisualizationAssets();
		}
		if(GizmosVisualizationSettings.HandVisualizerMesh != null && IK_Position_LeftHand != null){
			// DRAW HAND PREVIEW
			var pos = IK_Position_LeftHand.position + IK_Position_LeftHand.forward * 0.1f + IK_Position_LeftHand.up * -0.03f;
			var rot = IK_Position_LeftHand.rotation;

			Gizmos.color = new Color(0.9F, 0.6F, 0.3F, 1F);
			Gizmos.DrawWireMesh(GizmosVisualizationSettings.HandVisualizerWireMesh, pos, rot, new Vector3(-mscale, mscale, mscale));
			Gizmos.color = new Color(0, 0, 0, .3F);
			Gizmos.DrawMesh(GizmosVisualizationSettings.HandVisualizerMesh, pos, rot, new Vector3(-mscale, mscale, mscale));
		}
		if (Shoot_Position != null) 
		{
			//Gizmos.color = Color.red;
			//Gizmos.DrawWireSphere (Shoot_Position.position, 0.02f);
			//Gizmos.DrawLine (Shoot_Position.position, Shoot_Position.position + Shoot_Position.forward * 0.5f);
		}
	}
    private void OnDrawGizmosSelected()
    {
		if (GunSlider != null)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(GunSlider.position, GunSlider.position - GunSlider.forward * SliderMovementOffset);
		}
		if (AimMode != WeaponAimMode.None)
		{
			Gizmos.DrawWireSphere(transform.position + transform.right * CameraAimingPosition.x + transform.up * CameraAimingPosition.y + transform.forward * CameraAimingPosition.z, 0.01f);
		}
	}
#endif
}