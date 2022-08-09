using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBarFollowObject : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;
    [SerializeField]
    private RectTransform rectTransform;
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private float maxDistanceVisiblePlayer = 10.0f;
    [SerializeField]
    private float deltaOnAlfaUI = 2.0f;
    private PlayerController playerController;
    private RaycastHit hit;
    private bool isAbstacle = false;
    private bool visible;


    void Start()
    {
        rectTransform.gameObject.SetActive(false);
        StartCoroutine(CheckObstacle());
    }

    private void OnBecameVisible()
    {
        visible = true;
    }

    void OnBecameInvisible()
    {
        visible = false;
        rectTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!playerController && PlayerParameters.Instance) playerController = PlayerParameters.Instance.GetPlayerController(); //crutch
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (playerController)
        {
            if (visible)
            {
                {
                    float tempDistance = Vector3.Distance(playerController.transform.position, transform.position);

                    if (tempDistance < maxDistanceVisiblePlayer)
                    {
                        rectTransform.gameObject.SetActive(true);

                    }
                    else
                    {
                        rectTransform.gameObject.SetActive(false);
                    }

                    if (targetTransform)
                    {
                        rectTransform.position = Camera.main.WorldToScreenPoint(targetTransform.position);
                    }
                    else
                    {
                        Debug.LogError(" Not ref targetTransform ");
                    }
                }
                if (isAbstacle) rectTransform.gameObject.SetActive(false);
            }
        }
    }


    private IEnumerator CheckObstacle()
    {
        while (true)
        {
            //print("Go");
            if (playerController)
            {
                Color colorRay = Color.red;

                Vector3 dir = new Vector3(playerController.transform.position.x, playerController.transform.position.y + 0.5f, playerController.transform.position.z);
                Ray ray = new Ray(transform.position, dir - transform.position);
                //Debug.DrawRay(transform.position, dir - transform.position, colorRay, 1.0f);
                if (Physics.Raycast(ray, out hit))
                {
                    if (!hit.transform.GetComponent<PlayerController>())
                    {
                        isAbstacle = true;
                        //print(hit.transform.name);
                    }
                    else
                    {
                        isAbstacle = false;
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
