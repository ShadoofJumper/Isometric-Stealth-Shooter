using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    private ItemSettings item;
    private Image slotPanel;
    [SerializeField] private Image icon;
    [SerializeField] private int slotId;

    private void Awake()
    {
        slotPanel = GetComponent<Image>();
        //icon.enabled = false;
        //slotPanel.enabled = false;
    }

    public void AddItem(ItemSettings newItem)
    {
        item = newItem;
        icon.enabled = true;
        //slotPanel.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.enabled = false;
        //slotPanel.enabled = false;
    }

    public void RemoveWeapon()
    {
        Debug.Log("Drop item on ground!");
        SceneController.instance.playerInventory.RemoveWeapon(slotId);
    }

    public void SetMainSlot(Color mainColor)
    {
        slotPanel.color = mainColor;
    }

    public void SetDefaultSlot(Color defaulrColor)
    {
        slotPanel.color = defaulrColor;
    }

}
