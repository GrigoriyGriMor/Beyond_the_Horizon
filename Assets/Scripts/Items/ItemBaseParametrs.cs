using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Base Data", menuName = "Item Base", order = 100)]
public class ItemBaseParametrs : ScriptableObject
{
    [SerializeField] private SupportClass.ItemType itemType;

    [SerializeField] private string itemName = "";

    [SerializeField] private string discription;

    [SerializeField] private GameObject ObjectWithClass;
    [SerializeField] private GameObject SceneObjectVisual;

    [SerializeField] private Sprite sprite_1x1;
    [SerializeField] private Sprite sprite_2x1;
    [SerializeField] private Sprite sprite_1x2;

    [SerializeField] private int grade = 0;
    [SerializeField] private int itemBasePrice = 50;
    [SerializeField] private float ItemWeight = 0.01f;

    public bool canBeStack = false;

    [SerializeField] private ItemParametrsArray[] itemParam = new ItemParametrsArray[0];

    public GameObject GetVisualForPlayer()
    {
        return ObjectWithClass;
    }

    public GameObject GetVisualForScene() {
        return SceneObjectVisual;
    }

    public SupportClass.ItemType GetItemType()
    {
        return itemType;
    }

    public string GetItemName()
    {
        return itemName;
    }

    public Sprite GetItemIcon_1x1()
    {
        return sprite_1x1;
    }
    public Sprite GetItemIcon_2x1() 
    {
        return sprite_2x1;
    }

    public float GetItemWeight()
    {
        return ItemWeight;
    }

    public string GetItemDiscription() {
        return discription;
    }

    public ItemParametrsArray[] GetItemParameters() {
        return itemParam;
    }

    //solo
    public int GetItemBasePrice()
    {
        return itemBasePrice;
    }
}
