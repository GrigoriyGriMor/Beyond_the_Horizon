using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 startPos;
    [SerializeField] private float minRayDistance = 0.1f;

    [SerializeField] private Transform raycastPoint;
    [SerializeField] private float rayDistance = 1;
    [SerializeField] private float moveCamSpeed = 1;

    private void Awake()
    {
        startPos = transform.localPosition;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        float target;
        Physics.Raycast(raycastPoint.position, raycastPoint.forward * -1, out hit, rayDistance);

        if (hit.collider != null)
        {
            target = (Vector3.Distance(raycastPoint.position, hit.point) > minRayDistance) ? (raycastPoint.localPosition.z - (Vector3.Distance(raycastPoint.position, hit.point) - 0.05f)) 
                : (raycastPoint.localPosition.z - minRayDistance);
        }       
        else
            target = startPos.z;

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.LerpUnclamped(transform.localPosition.z, target, moveCamSpeed * Time.deltaTime));
    }
}
