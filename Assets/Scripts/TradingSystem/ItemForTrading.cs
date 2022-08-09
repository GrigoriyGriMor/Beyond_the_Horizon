using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[RequireComponent(typeof(EventTrigger))]
public class ItemForTrading : MonoBehaviour
{
    private NpcTrading npcTrading;

    [SerializeField] private GameObject emptyCell;
    [SerializeField] private GameObject filledCell;

    [SerializeField] private Image itemImage;
    [SerializeField] private TMPro.TMP_Text countText;
    [SerializeField] private int currentCount = 0;

    [SerializeField] private GameObject newItemTag;
    [SerializeField] private Image selectItem;

    public ItemInInventory currentItemInInventory;
    public ItemBaseParametrs itemInfo;
    public int thisIndex; 

    [Header("Drag Image")]
    Coroutine dragCoroutine = null;
    [SerializeField] private RectTransform dragImage;
    [HideInInspector] public bool drag = false;

    [Header("Картинка отказа")]
    [SerializeField] private Sprite cancelImage;

    [HideInInspector] public bool selectble = false;

    #region Click system
    public void PickDown()
    {
        if (itemInfo == null) return;

        if (dragCoroutine != null)
        {
            StopCoroutine(dragCoroutine);
            //dragCoroutine = null;
        }

        dragCoroutine = StartCoroutine(StartPick());    
    }

    public void PickUp()
    {
        if (itemInfo == null) return;
        if (dragCoroutine != null) EndPick(false);
    }

    public void PickEnter()
    {
        DeactiveTag();
    }

    public void PickExit()
    {
        if (itemInfo == null) return;

        if (dragCoroutine != null) EndPick(true);
    }

    private IEnumerator StartPick()
    {
        drag = false;
        yield return new WaitForSeconds(0.1f);
        ////обязательно останавливать корутину
        ///
        //dragImage.gameObject.SetActive(true);
        //emptyCell.SetActive(true);
        //filledCell.SetActive(false);
        //dragImage.GetComponent<Image>().sprite = itemInfo.GetItemIcon_1x1();
        //drag = true;
        ////npcTrading.dragbleItem = this;      ////////////////

        //while (drag)
        //{
        //    dragImage.transform.position = Input.mousePosition;
        //    yield return new WaitForFixedUpdate();
        //}

        //dragCoroutine = null;
    }

    private RaycastResult CheckUI()
    {//check ui with itemInInventory class on mouseposition
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;

        EventSystem.current.RaycastAll(data, results);

        return results[0];
    }

    private void EndPick(bool pointerExit)
    {
        if (!pointerExit)
        {
            if (drag)
            {
                RaycastResult result = CheckUI();

                switch (result.gameObject.GetComponent<MonoBehaviour>())
                {
                    case ItemInInventory obj:
                        AfterDrag(obj);
                        break;
                    case DropInventoryItem obj1:
                        npcTrading.DropItemToScene();
                        break;
                    case DeleteInventoryItem obj2:
                        npcTrading.DeleteItem();
                        break;
                    case WeaponInventorySlot obj3:
                        Debug.LogError("Weapon");
                        if (itemInfo.GetItemType() == SupportClass.ItemType.weapon)
                            npcTrading.SetNewWeapon(itemInfo, obj3.weaponNumber);
                        else
                            ResetCell();
                        break;
                    default:
                        ResetCell();
                        break;
                }
            }
            else
            {
                if (dragCoroutine != null) StopCoroutine(dragCoroutine);

                if (!selectble)
                {
                    SelectItem();
                    selectble = true;
                }
                else
                {
                    DeactiveSelect();
                }
            }
        }
        else
        {
            if (drag) return;

            if (dragCoroutine != null)
            {
                StopCoroutine(dragCoroutine);
                dragCoroutine = null;
            }
            dragImage.gameObject.SetActive(false);
        }
    }
    #endregion

    public void SelectItem()
    {
        if (npcTrading != null && itemInfo != null)
        {
            npcTrading.SetNewDiscription(this);
            selectItem.gameObject.SetActive(true);
        }
    }

    public void DeactiveSelect()
    {
        selectble = false;
        selectItem.gameObject.SetActive(false);
        npcTrading.ClearDiscription();
    }

    public void SetNpcTrading(NpcTrading npcTrading)
    {
        this.npcTrading = npcTrading;
    }

    private void DeactiveTag()
    {
        if (newItemTag.activeInHierarchy)
            newItemTag.SetActive(false);
    }

    #region item info get set
    public ItemTransaction GetItemInfo()
    {
        ItemTransaction info = new ItemTransaction();
        info.item = itemInfo;
        info.itemCount = currentCount;

        return info;
    }

    public void SetItemInfo(ItemTransaction newItem, bool newObj = true)
    {
        itemInfo = newItem.item;

        emptyCell.SetActive(false);
        filledCell.SetActive(true);

        itemImage.sprite = newItem.item.GetItemIcon_1x1();

        if (currentCount < 1)
        {
            currentCount += (newItem.itemCount <= 1) ? 1 : newItem.itemCount;
            countText.text = (currentCount <= 1) ? "" : currentCount.ToString();
            //if (newObj) newItemTag.SetActive(true);
        }
        else
        {
            currentCount += (newItem.itemCount <= 1) ? 1 : newItem.itemCount;
            countText.text = currentCount.ToString();
        }
    }

    public void ClearCell()
    {
        currentItemInInventory = null;
        itemInfo = null;
        //thisIndex = 0;
        drag = false;
        selectble = false;
        currentCount = 0;
        countText.text = "";

        selectItem.gameObject.SetActive(false);
        emptyCell.SetActive(true);
        filledCell.SetActive(false);
        dragImage.gameObject.SetActive(false);
    }

    public void ResetCell()
    {
        drag = false;
        dragImage.gameObject.SetActive(false);
        if (dragCoroutine != null) StopCoroutine(dragCoroutine);
        npcTrading.dragbleItem = null;

        emptyCell.SetActive(false);
        filledCell.SetActive(true);
    }
    #endregion

    private void AfterDrag(ItemInInventory pickItem)
    {
        if (pickItem.itemInfo != null)
        {
            if (pickItem != this)
            {
                if (pickItem.itemInfo.canBeStack && pickItem.itemInfo == itemInfo)
                {
                    pickItem.SetItemInfo(GetItemInfo(), false);
                    ClearCell();
                    npcTrading.ClearDiscription();
                }
                else
                {
                    ItemTransaction newT = pickItem.GetItemInfo();
                    pickItem.ClearCell();
                    pickItem.SetItemInfo(GetItemInfo(), false);

                    ClearCell();
                    SetItemInfo(newT, false);

                    npcTrading.ClearDiscription();
                }
            }
            else
                ResetCell();
        }
        else
        {
            pickItem.SetItemInfo(GetItemInfo(), false);
            ClearCell();
        }

        npcTrading.dragbleItem = null;
    }

    public int GetItemCount()
    {
        return currentCount;
    }

    public void SetItemCount(int itemCount)
    {
        currentCount = itemCount;
    }


    public void SetActiveCountText(bool isActive)
    {
        countText.gameObject.SetActive(isActive);
    }
}
