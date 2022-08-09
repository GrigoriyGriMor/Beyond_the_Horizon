using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[AddComponentMenu("JU TPS/Physics/Destructible")]
public class DestructibleObject : MonoBehaviour
{
    [Header("Destructible Settings")]

    [Range(0,50)]
    public float Strength;
    public GameObject FracturedObject;
    public Vector3 PositionOffset;
    public float TimeToDestroy = 15;
    private bool IsFractured = false;
    [Header("Destroy Events")]
    public bool DoSlowmotionWhenDestroy;
    public bool DoSlowmotionWhenPlayerIsJumping; // (Bullet time system)
    IEnumerator _DestroyObject()
    {
        if (IsFractured == false)
        {
            if (FracturedObject != null)
            {
                var fractured_obj = (GameObject)Instantiate(FracturedObject, transform.position + PositionOffset, transform.rotation);
                Destroy(this.gameObject, 0.01f);
                Destroy(fractured_obj, TimeToDestroy);
                IsFractured = true;
            }
            else
            {
                Debug.LogWarning("There is no 'Fractured Object' linked in " + gameObject.name);
            }
            if (DoSlowmotionWhenDestroy)
            {
                var cam = FindObjectOfType<CamPivotController>();
                cam.DoSlowMotion(0.1f, 5f);
            }
            if (DoSlowmotionWhenPlayerIsJumping && FindObjectOfType<ThirdPersonController>().IsJumping)
            {
                var cam = FindObjectOfType<CamPivotController>();
                cam.DoSlowMotion(0.1f, 5f);
            }
        }
        yield return new WaitForEndOfFrame();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            StartCoroutine(_DestroyObject());
        }

    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            StartCoroutine(_DestroyObject());
        }
        if (other.gameObject.TryGetComponent(out Rigidbody rb))
        {
            if (rb.velocity.magnitude > 5f)
            {
                StartCoroutine(_DestroyObject());
            }
        }
    }
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            StartCoroutine(_DestroyObject());
        }
    }
}
