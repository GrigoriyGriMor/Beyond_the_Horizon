using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractIO : MonoBehaviour
{
    [Header("имя и действие")]
    [SerializeField] private string _name;
    [SerializeField] private SupportClass.itemEvents _eventName;
    [SerializeField] private SupportClass.interactiveItemType type;

    public string GetName()
    {
        return _name;
    }

    public void SetName(string name) {
        _name = name;
    }

    public SupportClass.itemEvents GetEvent()
    {
        return _eventName;
    }

    public SupportClass.interactiveItemType GetItemType() {
        return type;
    }
}
