using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryController : CharacterBase
{
    [Header("Player Visual Block")]
    [Header("Position player visual")]
    [SerializeField] private GameObjectData prefabsData;
    [SerializeField] private Transform modelPosition;

    [SerializeField] private ItemInInventory I_Weapon_1;
    [SerializeField] private ItemInInventory I_Weapon_2;

    [SerializeField] private Image I_UseObj_1;
    [SerializeField] private Image I_UseObj_2;

    [SerializeField] private Image I_Module_1;
    [SerializeField] private Image I_Module_2;
    [SerializeField] private Image I_Module_3;
    [SerializeField] private Image I_Module_4;
    [SerializeField] private Image I_Module_5;

    [SerializeField] private Image I_Skin;



    [Header("Inventory Main Block")]
    [SerializeField] private RectTransform itemsBlock;
    [SerializeField] private List<ItemInInventory> inventoryCells = new List<ItemInInventory>();

    [SerializeField] private TMP_Text weightText;
    [SerializeField] private int maxWeight;
    [SerializeField] private int currentWeight;



    [Header("Discription Block")]
    [SerializeField] private GameObject descriptionActive;

    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemCount;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text mainDiscription;

    [SerializeField] private RectTransform paramMain;
    [SerializeField] private GameObject itemParamPanelObject;
    private List<ItemParamPanel> param = new List<ItemParamPanel>();

    [SerializeField] private Button dropButton;
    [SerializeField] private Button deleteButton;

    [Header("Coins")]
    [SerializeField] private TMP_Text coinsVisual;
    [SerializeField] private ItemBaseParametrs coinsItemBaseParam;
    private int currentCoins;

    private void Awake() {        
        for (int i = 0; i < itemsBlock.childCount; i++) {
            if (itemsBlock.GetChild(i).GetComponent<ItemInInventory>()) {
                inventoryCells.Add(itemsBlock.GetChild(i).GetComponent<ItemInInventory>());
                inventoryCells[inventoryCells.Count - 1].SetInventoryController(this);
            }
        }

        I_Weapon_1.SetInventoryController(this);
        I_Weapon_2.SetInventoryController(this);

        for (int j = 0; j < 10; j++) {
            ItemParamPanel obj = Instantiate(itemParamPanelObject, paramMain).GetComponent<ItemParamPanel>();
            param.Add(obj);
            obj.gameObject.SetActive(false);
        }

        descriptionActive.SetActive(false);

        deleteButton.onClick.AddListener(() => {
            if (descriptionActive.activeInHierarchy && currentItemInfo != null)
                DeleteItem();
        });

        dropButton.onClick.AddListener(() => {
            if (descriptionActive.activeInHierarchy && currentItemInfo != null)
                DropItemToScene();
        });

        currentCoins = 0;
        coinsVisual.text = currentCoins.ToString();
    }

    void Start() {
        try {
            Instantiate(prefabsData.playerObjects[_player.body_type].playerPrefabForUI, modelPosition.position, modelPosition.rotation, modelPosition.transform);
        }
        catch (System.Exception ex) {
            Debug.LogError(ex.ToString());
        }

        if (PlayerPrefs.HasKey("Coins")) {
            SetCoins(PlayerPrefs.GetInt("Coins"));
        }
        else
            PlayerPrefs.SetInt("Coins", currentCoins);
    }

    [HideInInspector] public ItemInInventory currentItemInfo;
    [HideInInspector] public ItemInInventory dragbleItem;
    public void SetNewDiscription(ItemInInventory item) {
        if (currentItemInfo != null) {
            currentItemInfo.DeactiveSelect();
            currentItemInfo = null;
        }

        currentItemInfo = item;
        descriptionActive.SetActive(true);
        itemImage.sprite = currentItemInfo.itemInfo.GetItemIcon_2x1();
        itemCount.text = currentItemInfo.GetItemCount().ToString();
        itemName.text = currentItemInfo.itemInfo.GetItemName();
        mainDiscription.text = currentItemInfo.itemInfo.GetItemDiscription();

        ItemParametrsArray[] parametrs = currentItemInfo.itemInfo.GetItemParameters();

        if (parametrs.Length > param.Count) {
            int newPanelsCount = parametrs.Length - param.Count;
            for (int i = 0; i < newPanelsCount; i++) {
                ItemParamPanel obj = Instantiate(itemParamPanelObject, paramMain).GetComponent<ItemParamPanel>();
                param.Add(obj);
                obj.gameObject.SetActive(false);
            }
        }

        for (int j = 0; j < parametrs.Length; j++) {
            param[j].gameObject.SetActive(true);
            param[j].SetParam(parametrs[j].parameterName, parametrs[j].parameterValue);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)descriptionActive.transform);
    }

    public void ClearDiscription() {
        currentItemInfo = null;
        descriptionActive.SetActive(false);

        for (int i = 0; i < param.Count; i++) {
            param[i].SetParam("", "");
            param[i].gameObject.SetActive(false);
        }
    }

    private void OnDisable() {
        if (currentItemInfo != null) {
            currentItemInfo.DeactiveSelect();
            currentItemInfo = null;
        }
    }

    public void SetCoins(int newCoins) {
        ItemTransaction newData = new ItemTransaction();
        newData.item = coinsItemBaseParam;
        newData.itemCount = newCoins;

        SetNewItem(newData);
    }

    public int GetCoinsCount() {
        return currentCoins;
    }

    public void SetNewItem(ItemTransaction itemData) {
        if (itemData.item.canBeStack) {
            for (int i = 0; i < inventoryCells.Count; i++)
                if (inventoryCells[i].itemInfo != null && inventoryCells[i].itemInfo == itemData.item) {
                    if (itemData.item.GetItemType() == SupportClass.ItemType.coins) {
                        currentCoins += itemData.itemCount;
                        currentCoins = Mathf.Clamp(currentCoins, 0, 1000000000);
                        coinsVisual.text = currentCoins.ToString();

                        PlayerPrefs.SetInt("Coins", currentCoins);

                        if (currentCoins == 0) { 
                            inventoryCells[i].ClearCell();
                            return;
                        }
                    }

                    inventoryCells[i].SetItemCount(inventoryCells[i].GetItemCount() + itemData.itemCount);
                    return;
                }
        }

        for (int i = 0; i < inventoryCells.Count; i++)
            if (inventoryCells[i].itemInfo == null) {
                if (itemData.item.GetItemType() == SupportClass.ItemType.coins) {
                    currentCoins += itemData.itemCount;
                    currentCoins = Mathf.Clamp(currentCoins, 0, 1000000000);
                    coinsVisual.text = currentCoins.ToString();

                    PlayerPrefs.SetInt("Coins", currentCoins);

                    if (currentCoins == 0) {
                        inventoryCells[i].ClearCell();
                        return;
                    }
                }

                inventoryCells[i].SetItemInfo(itemData);
                return;
            }
    }

    public void SetNewWeapon(ItemBaseParametrs _weapon, int wNumber) {
        weaponModule.SetWeapon(wNumber, _weapon);

        if (_weapon != null) dragbleItem.ClearCell();
        dragbleItem = null;
    }

    public void DropItemToScene() {
        if (dragbleItem != null) {
            ItemAtSceneController item = Instantiate(dragbleItem.itemInfo.GetVisualForScene(), new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation).GetComponent<ItemAtSceneController>();
            item.SetItemCount(dragbleItem.GetItemCount());

            item.GetComponent<Rigidbody>().AddForce(transform.forward + (new Vector3(0.25f, 1, 0)), ForceMode.Impulse);

            if (dragbleItem.itemInfo.GetItemType() == SupportClass.ItemType.coins) {
                currentCoins = 0;
                coinsVisual.text = currentCoins.ToString();
                PlayerPrefs.SetInt("Coins", currentCoins);
            }

            dragbleItem.ClearCell();

            if (dragbleItem == currentItemInfo) ClearDiscription();
            dragbleItem = null;
            return;
        }
        else
        if (currentItemInfo != null) {
            ItemAtSceneController item = Instantiate(currentItemInfo.itemInfo.GetVisualForScene(), new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation).GetComponent<ItemAtSceneController>();
            item.SetItemCount(currentItemInfo.GetItemCount());

            item.GetComponent<Rigidbody>().AddForce(transform.forward + (new Vector3(0.25f, 1, 0)), ForceMode.Impulse);


            if (currentItemInfo.itemInfo.GetItemType() == SupportClass.ItemType.coins) {
                currentCoins = 0;
                coinsVisual.text = currentCoins.ToString();
                PlayerPrefs.SetInt("Coins", currentCoins);
            }

            currentItemInfo.ClearCell();
            ClearDiscription();
        }
    }

    public void DeleteItem() {
        if (dragbleItem != null) {
            if (dragbleItem.itemInfo.GetItemType() == SupportClass.ItemType.coins) {
                currentCoins = 0;
                coinsVisual.text = currentCoins.ToString();
                PlayerPrefs.SetInt("Coins", currentCoins);
            }

            dragbleItem.ClearCell();

            if (dragbleItem == currentItemInfo) ClearDiscription();
            dragbleItem = null;
            return; 
        }
        else
        if (currentItemInfo != null) {
            if (currentItemInfo.itemInfo.GetItemType() == SupportClass.ItemType.coins) {
                currentCoins = 0;
                coinsVisual.text = currentCoins.ToString();
                PlayerPrefs.SetInt("Coins", currentCoins);
            }

            currentItemInfo.ClearCell();

            ClearDiscription();
        }
        // отправка на сервер данных о том, что объект был уничтожен
    }
    
    //Solo
    public void SetCurrentItemInfo(ItemInInventory currentItemInfo)
    {
        this.currentItemInfo = currentItemInfo;
    }

    //Solo
    public List<ItemInInventory> GetItemsInInventory()
    {
        return inventoryCells;
    }

}
