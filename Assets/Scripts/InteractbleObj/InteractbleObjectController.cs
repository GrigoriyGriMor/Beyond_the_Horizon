using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractbleObjectController : MonoBehaviour
{
    private string _name;
    private SupportClass.itemEvents _event;
    [SerializeField] private AbstractIO mainContoller;

    [HideInInspector] public SupportClass.interactiveItemType itemType;

    void Start()
    {
        _name = mainContoller.GetName();
        _event = mainContoller.GetEvent();
        itemType = mainContoller.GetItemType();
    }

    public void UseActivity(PlayerController playerController)
    {
       /* switch (itemType)
        {
            case SupportClass.interactiveItemType.NPC:
                if (mainContoller.GetComponent<NpcController>()) mainContoller.GetComponent<NpcController>().StartDialog(playerController);
                break;
            case SupportClass.interactiveItemType.Car:
                if (mainContoller.GetComponent<CarBase>()) mainContoller.GetComponent<CarBase>().StartCar(playerController);
                    break;
            case SupportClass.interactiveItemType.Other:
                if (mainContoller.GetComponent<ItemAtSceneController>()) mainContoller.GetComponent<ItemAtSceneController>().StartPickUpItem(playerController);
                break;
            default:
                break;
        }*/
    }

    public string Get_name()
    {
        return _name;
    }

    public SupportClass.itemEvents Get_event()
    {
        return _event;
    }

}
