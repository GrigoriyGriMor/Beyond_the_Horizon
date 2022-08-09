using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepController : MonoBehaviour
{
    [SerializeField] private Transform footObj;
    [SerializeField] private Transform footBone;

    [SerializeField] private float maxHight = 0.5f;
    [SerializeField] private float minDistanceToGround = 0.1f;
    [SerializeField] private float maxDistanceToGround = 0.15f;
    [SerializeField] private int layerMaskNumber = 31;

    private void FixedUpdate()
    {
        RaycastHit hit;
        Physics.Raycast(footBone.position, Vector3.up * -1, out hit, 5f/*, layerMaskNumber*/);

        if (hit.collider != null)
        {
            float dist = Vector3.Distance(hit.point, footBone.position);
            Debug.LogError(dist);
            if (dist < minDistanceToGround)
            {
                float dist2 = minDistanceToGround - dist;
                footObj.position = new Vector3(footBone.position.x, Mathf.Clamp(footObj.localPosition.y + dist2, -0.01f, 0.05f), footBone.position.z);
            }
            else
                if (dist > maxDistanceToGround)
            {
                float dist2 = dist - minDistanceToGround;
                footObj.position = new Vector3(footBone.position.x, Mathf.Clamp(footObj.localPosition.y - dist2, -0.01f, 0.05f), footBone.position.z);
            }
        }
    }
}
