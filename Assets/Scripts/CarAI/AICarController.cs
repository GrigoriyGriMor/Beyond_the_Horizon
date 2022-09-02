using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarController : CarMove
{
    [Header("Объект с чек поинтами")]
    [SerializeField] private Transform checkPointAsset;
    private List<Transform> checkPoints = new List<Transform>();

    private Vector3 nextTarget;
    [SerializeField] private GameObject targetController;
    [SerializeField] private float minDistance = 2;
    private int pointNomber;
    private float rotateAngle = 0;

    private bool canMove = true;

    private void Start()
    {
        if (checkPointAsset == null) return;

        for (int i = 0; i < checkPointAsset.childCount; i++)
            checkPoints.Add(checkPointAsset.GetChild(i));

        pointNomber = 0;
        nextTarget = checkPoints[pointNomber].position;
    }

    public void FixedUpdate()
    {
        if (!canMove)
        {
            StopCar();
            return;
        }

        TargetControl();
        
        Move(1f, rotateAngle);
    }

    private void TargetControl()
    {
        if (Vector3.Distance(targetController.transform.position, new Vector3(nextTarget.x, targetController.transform.position.y, nextTarget.z)) < minDistance)
        {
            pointNomber += 1;
            if (pointNomber < checkPoints.Count)
                nextTarget = checkPoints[pointNomber].transform.position;
            else
            {
                pointNomber = 0;
                nextTarget = checkPoints[pointNomber].transform.position;
            }
        }

        targetController.transform.LookAt(nextTarget);

        rotateAngle = targetController.transform.localEulerAngles.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            canMove = false;
            StartCoroutine(SignalForPlayer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            canMove = true;
            StopCoroutine(SignalForPlayer());
        }
    }

    private IEnumerator SignalForPlayer()
    {
        DeactivatorLight();
        yield return new WaitForSeconds(0.5f);

        ActivatorLight();
        yield return new WaitForSeconds(0.5f);

        DeactivatorLight();
        yield return new WaitForSeconds(0.5f);

        ActivatorLight();
        yield return new WaitForSeconds(0.5f);

        DeactivatorLight();
        yield return new WaitForSeconds(0.5f);

        ActivatorLight();
        yield return new WaitForSeconds(0.5f);

        DeactivatorLight();
        yield return new WaitForSeconds(0.5f);
    }
}
