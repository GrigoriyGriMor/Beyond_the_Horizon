using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemInInventory : MonoBehaviour
{
    private InventoryController controller;

    [Header("Type cell")]
    public SupportClass.InventorySlotType slotType = SupportClass.InventorySlotType.mainCell;

    [SerializeField] private GameObject emptyCell;
    [SerializeField] private GameObject filledCell;

    [SerializeField] private Image itemImage;
    [SerializeField] private TMPro.TMP_Text countText;
    [SerializeField] private int currentCount = 0;

    [SerializeField] private GameObject newItemTag;
    [SerializeField] private Image selectItem;

    public ItemBaseParametrs itemInfo;

    [Header("Drag Image")]
    Coroutine dragCoroutine = null;
    [SerializeField] private RectTransform dragImage;
    [HideInInspector] public bool drag = false;

    [Header("Картинка отказа")]
    [SerializeField] private Sprite cancelImage;

    [HideInInspector] public bool selectble = false;

    #region Click system
    public void PickDown() {
        if (itemInfo == null) return;

        if (dragCoroutine != null) {
            StopCoroutine(dragCoroutine);
            dragCoroutine = null;
        }

        dragCoroutine = StartCoroutine(StartPick());
    }
    public void PickUp() {
        if (itemInfo == null) return;
        if (dragCoroutine != null) EndPick(false);
    }

    public void PickEnter() {
        DeactiveTag();
    }

    public void PickExit() {
        if (itemInfo == null) return;

        if (dragCoroutine != null) EndPick(true); 
    }

    private IEnumerator StartPick() {
        drag = false;
        yield return new WaitForSeconds(0.1f);
        //обязательно останавливать корутину

        dragImage.gameObject.SetActive(true);
        emptyCell.SetActive(true);
        filledCell.SetActive(false);
        dragImage.GetComponent<Image>().sprite = itemInfo.GetItemIcon_1x1();
        drag = true;
        controller.dragbleItem = this;

        while (drag) {
            dragImage.transform.position = Input.mousePosition;
            yield return new WaitForFixedUpdate();
        }

        dragCoroutine = null;
    }

    private RaycastResult CheckUI() {//check ui with itemInInventory class on mouseposition
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;

        EventSystem.current.RaycastAll(data, results);

        return results[0];
    }

    private void EndPick(bool pointerExit) {
        if (!pointerExit) {
            if (drag) {
                RaycastResult result = CheckUI();

                switch (result.gameObject.GetComponent<MonoBehaviour>()) {
                    case ItemInInventory obj:
                        AfterDrag(obj);
                        break;
                    case DropInventoryItem obj1:
                        controller.DropItemToScene();
                        break;
                    case DeleteInventoryItem obj2:
                        controller.DeleteItem(); 
                        break;
                    default:
                        ResetCell();
                        break;
                }
            }
            else {
                if (dragCoroutine != null) StopCoroutine(dragCoroutine);

                if (!selectble) {
                    SelectItem();
                    selectble = true;
                }
                else {
                    DeactiveSelect();
                }
            }
        }
        else {
            if (drag) return;

            if (dragCoroutine != null) {
                StopCoroutine(dragCoroutine);
                dragCoroutine = null;
            }
            dragImage.gameObject.SetActive(false);
        }
    }
    #endregion

    public void SelectItem() {
        if (controller != null && itemInfo != null) {
            controller.SetNewDiscription(this);
            selectItem.gameObject.SetActive(true);
        }
    }

    public void DeactiveSelect() {
        selectble = false;
        selectItem.gameObject.SetActive(false);
        controller.ClearDiscription();
    }

    public void SetInventoryController(InventoryController _controller) {
        controller = _controller;
    }

    private void DeactiveTag() {
        if (newItemTag.activeInHierarchy)
            newItemTag.SetActive(false);
    }

    #region item info get set
    public ItemTransaction GetItemInfo() {
        ItemTransaction info = new ItemTransaction();
        info.item = itemInfo;
        info.itemCount = currentCount;

        return info;
    }

    public void SetItemInfo(ItemTransaction newItem, bool newObj = true) {
        itemInfo = newItem.item;

        emptyCell.SetActive(false);
        filledCell.SetActive(true);

        switch (slotType) {
            case SupportClass.InventorySlotType.mainCell:
                itemImage.sprite = newItem.item.GetItemIcon_1x1();
                break;
            case SupportClass.InventorySlotType.weaponCell:
                itemImage.sprite = newItem.item.GetItemIcon_2x1();
                controller.SetNewWeapon(itemInfo, (GetComponent<WeaponInventorySlot>() != null) ? GetComponent<WeaponInventorySlot>().weaponNumber : 0);
                break;
            default:
                break;
        }


        if (currentCount < 1) {
            currentCount += (newItem.itemCount <= 1) ? 1 : newItem.itemCount;
            countText.text = (currentCount <= 1) ? "" : currentCount.ToString();
            if (newObj) newItemTag.SetActive(true);
        }
        else {
            currentCount += (newItem.itemCount <= 1) ? 1 : newItem.itemCount; 
            countText.text = currentCount.ToString();
        }
    }

    public void ClearCell() {
        itemInfo = null;

        drag = false;
        selectble = false;
        currentCount = 0;
        countText.text = "";

        selectItem.gameObject.SetActive(false);
        emptyCell.SetActive(true);
        filledCell.SetActive(false);
        dragImage.gameObject.SetActive(false);

        if (slotType == SupportClass.InventorySlotType.weaponCell)
            controller.SetNewWeapon(null, (GetComponent<WeaponInventorySlot>() != null) ? GetComponent<WeaponInventorySlot>().weaponNumber : 0);
    }

    public void ResetCell() {
        drag = false;
        dragImage.gameObject.SetActive(false);
        if (dragCoroutine != null) StopCoroutine(dragCoroutine);
        controller.dragbleItem = null;

        emptyCell.SetActive(false);
        filledCell.SetActive(true);
    }
    #endregion

    private void AfterDrag(ItemInInventory pickItem) {
        if (pickItem.itemInfo != null) {
            if (pickItem != this) {
                if (pickItem.itemInfo.canBeStack && pickItem.itemInfo == itemInfo) {
                    pickItem.SetItemInfo(GetItemInfo(), false);
                    ClearCell();
                    controller.ClearDiscription();
                }
                else
                    ResetCell();
            }
            else
                ResetCell();
        }
        else {
            switch (pickItem.slotType) {
                case SupportClass.InventorySlotType.mainCell:
                    pickItem.SetItemInfo(GetItemInfo(), false);
                    ClearCell();
                    break;
                case SupportClass.InventorySlotType.weaponCell:
                    if (itemInfo.GetItemType() == SupportClass.ItemType.weapon) {
                        pickItem.SetItemInfo(GetItemInfo(), false);
                        ClearCell();
                    }
                    else
                        ResetCell();
                    break;
                default:
                    break;
            }
        }

        controller.dragbleItem = null;
    }

    //solo
    public void SetItemCount(int itemCount)
    {
        currentCount = itemCount;
        countText.text = (currentCount <= 1) ? "" : currentCount.ToString();
    }
    public int GetItemCount() {
        return currentCount;
    }
}
