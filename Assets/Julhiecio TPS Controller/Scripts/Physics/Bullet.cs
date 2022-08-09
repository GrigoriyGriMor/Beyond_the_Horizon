using UnityEngine;

[AddComponentMenu("JU TPS/Physics/Bullet")]
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
	private Rigidbody rb;
	[Header("Bullet Settings")]
	public float BulletVelocity;
	public GameObject DestroyBulletParticle;
	public GameObject BulletHole;
	[HideInInspector]
	public Vector3 DestroyBulletRotation;

	[Header("Physics: It is calculated by physics")]
	[Header("Calculated: Follow a path between two points")]
	[Header("Teleport: It is teleported to the hit point.")]
	public BulletMovementType MovementType;

	[HideInInspector]
	public Vector3 FinalPoint;  //It's the camera raycast hit position
	void Start()
    {
		rb = GetComponent<Rigidbody> ();
		rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

		//If movement is physic type
		if (MovementType == BulletMovementType.Physics) {
			rb.velocity = transform.forward * BulletVelocity;
        }
        else
        {
			rb.useGravity = false;
        }
		//If movement is teleport type
		if (MovementType == BulletMovementType.Teleport && FinalPoint != Vector3.zero) {
			transform.position = FinalPoint;
		}
    }
	void FixedUpdate(){
		//If movement is calculated type
		if (MovementType == BulletMovementType.Calculated && FinalPoint != Vector3.zero) {
			transform.position = Vector3.MoveTowards (transform.position, FinalPoint, BulletVelocity * Time.deltaTime);
		} 
		if(FinalPoint == Vector3.zero) {
			transform.Translate (0,0, BulletVelocity * Time.deltaTime);
		}
		if(transform.position == FinalPoint && IsInvoking("DestroyBullet") == false){
			Invoke("DestroyBullet", 0.2f);
			rb.velocity = transform.forward * BulletVelocity;
		}
	}
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag != "Bullet" && col.gameObject.tag != "Player")
		{
			//Instantiate and destroy Bullet Collision Particle
			if (FinalPoint == Vector3.zero)
				FinalPoint = transform.position;

			var defaultcollisionparticle = (GameObject)Instantiate(DestroyBulletParticle, FinalPoint, Quaternion.FromToRotation(transform.forward, DestroyBulletRotation) * transform.rotation);
			Destroy(defaultcollisionparticle, 2f);

			//Instantiate and Destroy Bullet Hole
			if (MovementType != BulletMovementType.Physics)
			{
				var bullethole = (GameObject)Instantiate(BulletHole, FinalPoint, Quaternion.FromToRotation(transform.up, DestroyBulletRotation) * transform.rotation);
				bullethole.transform.position = bullethole.transform.position + bullethole.transform.up * 0.001f;
				bullethole.transform.SetParent(col.collider.gameObject.transform);
				Destroy(bullethole, 10f);
			}
			//Destroy bullet
			DestroyBullet();
		}
	}
	public void DestroyBullet(){
		Destroy(gameObject);
	}
	public enum BulletMovementType{
		Physics,
		Calculated,
		Teleport,
	}
}
