using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class NpcTrading : MonoBehaviour
{
    [SerializeField]
    private GameObjectData gameObjectData;
    [SerializeField]
    private List<ItemTrade> itemTrades;
    [SerializeField]
    private float npcTradingMultiply = 1.0f;
    [SerializeField]
    private NpcController npcController;
    [SerializeField]
    private Transform currentPlayer;
    [SerializeField]
    private InventoryController inventoryController;
    [SerializeField]
    private TMP_Text topBarTextCoint;
    [SerializeField]
    private GameObject panelTrading;
    [SerializeField]
    private Toggle tabBuy;
    [SerializeField]
    private Toggle tabSell;
    [SerializeField]
    private Button buttonBuy;
    [SerializeField]
    private Button buttonSell;
    [SerializeField]
    private Button buttonExit;
    [SerializeField]
    private TMP_Text buyPriceText;
    [SerializeField]
    private TMP_Text sellPriceText;
    [SerializeField]
    private GameObject countSlider;
    [SerializeField]
    private InputField textInputField;
    [SerializeField]
    private Slider sliderItemCount;
    private enum Trading
    {
        Buy,
        Sell
    }
    [SerializeField]
    private Trading trading;


    [Header("Inventory Main Block")]
    [Header("_______________________________________")]
    [SerializeField] private RectTransform itemsBlock;
    [SerializeField] private List<ItemForTrading> tradingCells = new List<ItemForTrading>();

    [Header("Discription Block")]
    [Header("_______________________________________")]
    [SerializeField] private GameObject descriptionActive;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemCount;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text mainDiscription;
    [SerializeField] private RectTransform paramMain;
    [SerializeField] private GameObject itemParamPanelObject;

    [Header("Inventory Main Block")]
    private List<ItemParamPanel> param = new List<ItemParamPanel>();

    [Header("Messege Block")]
    [Header("_______________________________________")]

    [SerializeField]
    private GameObject messageYesNo;
    [SerializeField]
    private Button buttonMessageOK;
    [SerializeField]
    GameObject messageNotCoins;


    public ItemForTrading currentItemInfo;
    public int currentIndexItem;
    public ItemForTrading dragbleItem;
    public ItemInInventory currentItemInInventory;
    public int currentCoints;
    [SerializeField]
    private int maxValueItem = 1;


    private void Awake()
    {
        if (itemsBlock)
        {
            for (int i = 0; i < itemsBlock.childCount; i++)
            {
                if (itemsBlock.GetChild(i).GetComponent<ItemForTrading>())
                {
                    tradingCells.Add(itemsBlock.GetChild(i).GetComponent<ItemForTrading>());
                    tradingCells[tradingCells.Count - 1].SetNpcTrading(this);
                    tradingCells[tradingCells.Count - 1].thisIndex = tradingCells.Count - 1;
                }
            }
        }

        for (int j = 0; j < 10; j++)
        {
            ItemParamPanel obj = Instantiate(itemParamPanelObject, paramMain).GetComponent<ItemParamPanel>();
            param.Add(obj);
            obj.gameObject.SetActive(false);
        }

        if (descriptionActive) descriptionActive.SetActive(false);
        if (buttonExit) buttonExit.onClick.AddListener(() => CloseTrading());

        if (tabBuy) tabBuy.onValueChanged.AddListener((value) => ActiveBuy(value));
        if (tabSell) tabSell.onValueChanged.AddListener((value) => ActiveSell(value));


        if (buttonBuy) buttonBuy.onClick.AddListener(() => MessageBuy());
        if (buttonSell) buttonSell.onClick.AddListener(() => MessageSell());

        if (textInputField) textInputField.onValueChanged.AddListener(ChangedValue);
        if (textInputField) textInputField.onEndEdit.AddListener(EndValue);

        if (sliderItemCount)
        {
            sliderItemCount.onValueChanged.AddListener(delegate { OnValueChanged(); });
        }

        if (buttonMessageOK) buttonMessageOK.onClick.AddListener(() => CompleteTheTransaction());
    }

    private void ChangedValue(string value)
    {
        if (int.TryParse(textInputField.text, out int number))
        {
            number = Mathf.Clamp(number, 1, maxValueItem);
            textInputField.text = number.ToString();
            sliderItemCount.value = number;
            //Debug.Log("Введено» +значение");
        }
        else
        {
            textInputField.text = "1";
        }
    }

    private void ActiveBuy(bool isOn)
    {
        if (isOn)
        {
            //print("ActiveBuy");
            tabBuy.isOn = isOn;
            OnValueChanged();
            ShowItemBuy();
            trading = Trading.Buy;
        }
    }

    private void ActiveSell(bool isOn)
    {
        if (isOn)
        {
            //print("ActiveSell");
            tabSell.isOn = isOn;
            OnValueChanged();
            ShowItemSell();
            trading = Trading.Sell;
        }
    }

    private void EndValue(string value)
    {
        if (int.TryParse(textInputField.text, out int number))
        {
            number = Mathf.Clamp(number, 1, maxValueItem);
            textInputField.text = number.ToString();
            sliderItemCount.value = number;
            //Debug.Log("Конечный контент» +значение");
        }
        else
        {
            textInputField.text = "1";
        }
    }

    private void Update()
    {
        UpdateButton();
    }

    public void ActivateTrading()
    {
        if (panelTrading)
        {
            panelTrading.SetActive(true);
        }

        if (npcController)
        {
            currentPlayer = npcController.currentPlayer;
        }
        else
        {
            //print("Not npcController");
        }

        if (currentPlayer)
        {
            inventoryController = currentPlayer.GetComponent<InventoryController>();
            currentCoints = inventoryController.GetCoinsCount();
            if (topBarTextCoint) topBarTextCoint.text = ($"COINTS: {currentCoints}");
        }
        else
        {
            //print("Not currentPlayer");
        }
        ActiveBuy(true);
    }

    private void ShowItemBuy()
    {
        ClearDiscription();
        for (int i = 0; i < tradingCells.Count; i++)
        {
            tradingCells[i].itemInfo = null;
            tradingCells[i].ClearCell();
            tradingCells[i].SetActiveCountText(false);
        }

        ItemTransaction itemData = new ItemTransaction();
        for (int index = 0; index < itemTrades.Count; index++)
        {
            if (gameObjectData.otherObjects.Count > itemTrades[index].IDItem)
            {
                if (gameObjectData.otherObjects[itemTrades[index].IDItem].GetItemType() != SupportClass.ItemType.coins) {
                    itemData.item = gameObjectData.otherObjects[itemTrades[index].IDItem];
                    SetNewItem(itemData, index);
                }
            }
        }
        countSlider.SetActive(false);
        buttonBuy.gameObject.SetActive(false);

        currentCoints = inventoryController.GetCoinsCount();
        if (topBarTextCoint) topBarTextCoint.text = ($"COINTS: {currentCoints}");
        //maxValueItem = 100;
    }

    private void ShowItemSell()
    {
        ClearDiscription();
        for (int i = 0; i < tradingCells.Count; i++)
        {
            tradingCells[i].itemInfo = null;
            tradingCells[i].ClearCell();
            tradingCells[i].SetActiveCountText(true);

        }

        ItemTransaction itemData = new ItemTransaction();

        if (inventoryController)
        {
            //print("inventoryController.GetItemsInInventory().Count " + inventoryController.GetItemsInInventory().Count);

            for (int index = 0; index < inventoryController.GetItemsInInventory().Count; index++)
            {
                itemData.item = inventoryController.GetItemsInInventory()[index].itemInfo;
                itemData.itemCount = inventoryController.GetItemsInInventory()[index].GetItemCount();

                if (itemData.item != null)
                {
                    if (itemData.item.GetItemType() != SupportClass.ItemType.coins)
                        SetNewItem(itemData, index);
                }
            }
        }
        else
        {
            print("Not inventoryController");
        }

        countSlider.SetActive(false);
        buttonSell.gameObject.SetActive(false);

        currentCoints = inventoryController.GetCoinsCount();
        if (topBarTextCoint) topBarTextCoint.text = ($"COINTS: {currentCoints}");
    }

    private void OnValueChanged()
    {
        textInputField.text = sliderItemCount.value.ToString();
        SetButtonPriceText();
    }

    public void SetNewItem(ItemTransaction itemData, int index)
    {
        if (itemData.item.canBeStack)
        {
            for (int i = 0; i < tradingCells.Count; i++)
                if (tradingCells[i].itemInfo != null && tradingCells[i].itemInfo == itemData.item)
                {
                    tradingCells[i].SetItemInfo(itemData);
                    return;
                }
        }

        for (int i = 0; i < tradingCells.Count; i++)
            if (tradingCells[i].itemInfo == null)
            {
                tradingCells[i].SetItemInfo(itemData);
                tradingCells[i].currentItemInInventory = inventoryController.GetItemsInInventory()[index];
                return;
            }
    }

    public void SetNewDiscription(ItemForTrading item)
    {
        if (currentItemInfo != null)
        {
            currentItemInfo.DeactiveSelect();
            currentItemInfo = null;
        }

        currentItemInfo = item;
        currentIndexItem = item.thisIndex;

        descriptionActive.SetActive(true);
        itemImage.sprite = currentItemInfo.itemInfo.GetItemIcon_2x1();
        itemCount.text = currentItemInfo.GetItemCount().ToString();
        itemName.text = currentItemInfo.itemInfo.GetItemName();
        mainDiscription.text = currentItemInfo.itemInfo.GetItemDiscription();

        ItemParametrsArray[] parametrs = currentItemInfo.itemInfo.GetItemParameters();

        if (parametrs.Length > param.Count)
        {
            int newPanelsCount = parametrs.Length - param.Count;
            for (int i = 0; i < newPanelsCount; i++)
            {
                ItemParamPanel obj = Instantiate(itemParamPanelObject, paramMain).GetComponent<ItemParamPanel>();
                param.Add(obj);
                obj.gameObject.SetActive(false);
            }
        }

        for (int j = 0; j < parametrs.Length; j++)
        {
            param[j].gameObject.SetActive(true);
            param[j].SetParam(parametrs[j].parameterName, parametrs[j].parameterValue);
        }

        currentItemInInventory = tradingCells[currentIndexItem].currentItemInInventory;  ////

        if (trading == Trading.Buy)
        {
            maxValueItem = 100;
        }
        else
        {
            if (currentItemInfo)
            {
                if (currentItemInfo.GetItemCount() > 0)
                {
                    maxValueItem = currentItemInfo.GetItemCount();
                }
                else
                {
                    maxValueItem = 1;
                }
            }
        }

        ActiveSlider();
        SetButtonPriceText();

        if (trading == Trading.Buy)
        {
            buttonBuy.gameObject.SetActive(true);
        }
        else
        {
            buttonSell.gameObject.SetActive(true);
        }

        topBarTextCoint.text = ($"COINTS: {inventoryController.GetCoinsCount()}");

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)descriptionActive.transform);
    }

    private void ActiveSlider()
    {
        sliderItemCount.value = 0;
        if (trading == Trading.Buy)
        {
            bool canBeStack = gameObjectData.otherObjects[itemTrades[currentIndexItem].IDItem].canBeStack;
            countSlider.SetActive(canBeStack);
        }
        else
        {
            bool canBeStack = currentItemInInventory.GetItemInfo().item.canBeStack;
            countSlider.SetActive(canBeStack);
        }
    }

    private void SetButtonPriceText()
    {

        if (trading == Trading.Buy)
        {
            buyPriceText.text = ($"PRICE: {(int)sliderItemCount.value * itemTrades[currentIndexItem].priceItem}");
        }
        else
        {
            int price = 0;

            if (currentItemInInventory)
            {
                if (currentItemInInventory.itemInfo)
                {
                    price = (int)(npcTradingMultiply * currentItemInInventory.itemInfo.GetItemBasePrice());
                }
            }

            sellPriceText.text = ($"PRICE: {(int)sliderItemCount.value * price}");
        }
    }

    public void ClearDiscription()
    {
        currentItemInfo = null;
        descriptionActive.SetActive(false);

        for (int i = 0; i < param.Count; i++)
        {
            param[i].SetParam("", "");
            param[i].gameObject.SetActive(false);
        }

        currentIndexItem = 0;

        countSlider.SetActive(false);
        buttonBuy.gameObject.SetActive(false);
        buttonSell.gameObject.SetActive(false);
    }

    public void CompleteTheTransaction()
    {
        if (trading == Trading.Buy)
        {
            Buy();
        }
        else
        {
            Sell();
        }
    }

    private void MessageBuy()
    {
        int tempPrice = currentCoints - (int)sliderItemCount.value * itemTrades[currentIndexItem].priceItem;

        if (tempPrice >= 0)
        {
            messageYesNo.SetActive(true);
        }
        else
        {
            messageNotCoins.SetActive(true);
        }
    }

    private void Buy()
    {
        int tempPrice = currentCoints - (int)sliderItemCount.value * itemTrades[currentIndexItem].priceItem;

        if (tempPrice >= 0)
        {
            //currentItemInfo = null;

            ItemTransaction newData = new ItemTransaction();
            newData.item = gameObjectData.otherObjects[itemTrades[currentIndexItem].IDItem];
            newData.itemCount = (int)sliderItemCount.value;
            if (inventoryController) { 
                inventoryController.SetNewItem(newData);
                inventoryController.SetCoins((-1) * (int)sliderItemCount.value * itemTrades[currentIndexItem].priceItem);
            }
            currentCoints = tempPrice;
            if (topBarTextCoint) topBarTextCoint.text = ($"COINTS: {inventoryController.GetCoinsCount()}");
            //print("BUY");
            messageYesNo.SetActive(false);
        }
    }

    private void MessageSell()
    {
        messageYesNo.SetActive(true);
    }

    private void Sell()
    {
        if (currentItemInfo)
        {
            int currentSliderValue = (int)sliderItemCount.value;
            int tempSliderValue = currentItemInfo.GetItemCount() - currentSliderValue;
            int currentPrice = 0;
            if (tempSliderValue < 1) tempSliderValue = 1;

            if (currentSliderValue == currentItemInfo.GetItemCount() && currentItemInInventory)
            {
                DeleteItem();

                currentPrice = currentItemInInventory.itemInfo.GetItemBasePrice();
                if (inventoryController) inventoryController.SetCurrentItemInfo(currentItemInInventory);
                if (inventoryController) inventoryController.DeleteItem();
            }
            else
            {
                currentPrice = currentItemInInventory.itemInfo.GetItemBasePrice();
                currentItemInInventory.SetItemCount(tempSliderValue);
                currentItemInInventory.GetItemInfo().itemCount = tempSliderValue;
            }

            currentCoints += (int)(npcTradingMultiply * currentPrice) * currentSliderValue;
            inventoryController.SetCoins((int)(npcTradingMultiply * currentPrice) * currentSliderValue);
            if (topBarTextCoint) topBarTextCoint.text = ($"COINTS: {inventoryController.GetCoinsCount()}");
            //print("SELL");
            messageYesNo.SetActive(false);
            ShowItemSell();
        }
        else
        {
            print("Not currentItemInfo ");
        }

    }

    public void DropItemToScene()
    {
        if (dragbleItem != null)
        {
            ItemAtSceneController item = Instantiate(dragbleItem.itemInfo.GetVisualForScene(), new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation).GetComponent<ItemAtSceneController>();
            item.SetItemCount(dragbleItem.GetItemCount());

            item.GetComponent<Rigidbody>().AddForce(transform.forward + (new Vector3(0.25f, 1, 0)), ForceMode.Impulse);

            dragbleItem.ClearCell();

            if (dragbleItem == currentItemInfo) ClearDiscription();
            dragbleItem = null;
            return;
        }
        else
        if (currentItemInfo != null)
        {
            ItemAtSceneController item = Instantiate(currentItemInfo.itemInfo.GetVisualForScene(), new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation).GetComponent<ItemAtSceneController>();
            item.SetItemCount(currentItemInfo.GetItemCount());

            item.GetComponent<Rigidbody>().AddForce(transform.forward + (new Vector3(0.25f, 1, 0)), ForceMode.Impulse);

            currentItemInfo.ClearCell();
            ClearDiscription();
        }
    }

    public void DeleteItem()
    {
        if (dragbleItem != null)
        {

            if (dragbleItem == currentItemInfo) ClearDiscription();
            dragbleItem = null;
            return;
        }
        else
        if (currentItemInfo != null)
        {
            currentItemInfo.ClearCell();

            ClearDiscription();
        }
        // отправка на сервер данных о том, что объект был уничтожен
    }

    public void SetNewWeapon(ItemBaseParametrs _weapon, int wNumber)
    {
        Debug.LogError("SetNewWeapon In Inventory");

        //weaponModule.SetWeapon(wNumber, _weapon);
        // if (wNumber == 0)
        //  I_Weapon_2.sprite = _weapon.GetItemIcon_2x1();
        // else
        //  I_Weapon_1.sprite = _weapon.GetItemIcon_2x1();

        //  dragbleItem.ClearCell();
        // dragbleItem = null;
    }

    private void UpdateButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
        {
            CloseTrading();
        }
    }

    private void CloseTrading()
    {
        currentPlayer = null;
        inventoryController = null;
        currentItemInfo = null;
        currentIndexItem = 0;
        panelTrading.SetActive(false);
    }
}


[System.Serializable]

public class ItemTrade
{
    public int IDItem;
    public int priceItem;
}