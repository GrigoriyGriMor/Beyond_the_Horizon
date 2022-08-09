using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    [SerializeField] private float speed = 15.0f;

    void Update()
    {
        Vector3 direction = transform.right * Input.GetAxis("Horizontal");
        direction = direction + transform.forward * Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.E))
            direction = new Vector3(direction.x, direction.y + speed / 2 * Time.deltaTime, direction.z);

        if (Input.GetKey(KeyCode.Q))
            direction = new Vector3(direction.x, direction.y - speed / 2 * Time.deltaTime, direction.z);

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
    }

}
