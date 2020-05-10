using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    private Item item;
    [SerializeField] private Image icon;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            UseItem();
        else if (eventData.button == PointerEventData.InputButton.Right)
            DropItem();
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = newItem.ItemSettings.Icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void UseItem()
    {
        if(item!=null)
            item.Use();
    }

    public void DropItem()
    {
        //TO DO
        Debug.Log("Drop item on ground!");
        if (item != null)
            SceneController.instance.playerInventory.RemoveItem(item);
    }

}
