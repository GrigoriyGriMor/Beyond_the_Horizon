using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// проверка interactbleObjectController висит на плеере
/// </summary>
/// 
public class ChecknteractbleObj : MonoBehaviour
{
    Transform thisTransform;
    RaycastHit hit;

    [SerializeField]
    PlayerController playerController;

    [Header("Мах расстояние до обьекта")]
    [SerializeField]
    private float maxDistationInteractbleObj = 1.0f;

    private int currentIdObject;

    private int lastCurrentObject;
    bool isActive = false;
    Transform _camera;

    public void Init(Camera c)
    {
        _camera = c.transform;
        thisTransform = GetComponent<Transform>();
        isActive = true;
    }

    private void FixedUpdate()
    {
        if (!isActive) return;

        CheckObj();
    }

    private void CheckObj()
    {
        //Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward * 50, out hit))
        {
            if (hit.transform.TryGetComponent<InteractbleObjectController>(out InteractbleObjectController interactbleObjectController))
            {
                float dist = Vector3.Distance(thisTransform.position, hit.transform.position);

                if (dist < maxDistationInteractbleObj)
                {
                    currentIdObject = interactbleObjectController.GetInstanceID();
                    if (lastCurrentObject != currentIdObject)
                        CanUseObject(interactbleObjectController);

                    lastCurrentObject = currentIdObject;
                }
                else
                {
                    if (hit.collider.GetComponent<BilboardController>()) {
                        currentIdObject = interactbleObjectController.GetInstanceID();
                        if (lastCurrentObject != currentIdObject)
                            CanUseObject(interactbleObjectController);

                        lastCurrentObject = currentIdObject;
                    }
                    else {
                        currentIdObject = 0;
                        lastCurrentObject = currentIdObject;
                        OutUseObject();
                    }
                }
            }
            else
            {
                currentIdObject = 0;
                lastCurrentObject = currentIdObject;
                OutUseObject();
            }
        }
    }

    private void CanUseObject(InteractbleObjectController interactbleObjectController)
    {
        playerController.CanUseObject(interactbleObjectController.Get_name(), interactbleObjectController.Get_event().ToString(), interactbleObjectController);
    }

    private void OutUseObject()
    {
        playerController.OutUseObject();
    }
}
