using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APCameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraIdlePos;
    [SerializeField] private Transform cameraMovePos;

    private Transform cameraPos;
    [SerializeField] private Transform target;

    [SerializeField] private float moveSpeed = 5;
    private float moveS = 0;

    private void Start()
    {
        cameraPos = cameraIdlePos;
        moveS = moveSpeed / 5;
    }

    private void FixedUpdate()
    {
        gameObject.transform.position = Vector3.Lerp(transform.position, cameraPos.position, moveS * Time.deltaTime);
        gameObject.transform.LookAt(target);
    }

    private Coroutine coroutine;

    public void StartMove()
    {
        if (cameraPos == cameraMovePos) return;

        cameraPos = cameraMovePos;

        if (coroutine == null)
            coroutine = StartCoroutine(SpeedUp());
    }

    private IEnumerator SpeedUp()
    {
        while (moveS < moveSpeed)
        {
            moveS += 1 * Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }


    public void StopMove()
    {
        if (cameraPos == cameraIdlePos) return;

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        
        cameraPos = cameraIdlePos;
        moveS = moveSpeed / 5;
    }
}
