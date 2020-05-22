using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour, IPointerClickHandler
{
    private Weapon item;
    private Image slotPanel;
    [SerializeField] private Image icon;
    [SerializeField] private int slotId;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            EquipWeapon();
        else if (eventData.button == PointerEventData.InputButton.Right)
            RemoveFromWeapon();
    }

    private void Awake()
    {
        slotPanel = GetComponent<Image>();
        //icon.enabled = false;
        //slotPanel.enabled = false;
    }

    private void EquipWeapon()
    {
        Debug.Log("EquipWeapon");
        if (item != null)
        {
            Debug.Log("norrm!");
            item.EquipWeapon();

        }
        else
        {
            Debug.Log("Bug!");
        }
    }

    private void RemoveFromWeapon()
    {
        Debug.Log("RemoveFromWeapon");
        if (item != null)
            item.RemoveFromWeapons();
    }



    public void AddItem(Weapon newItem)
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
        SceneController.instance.playerInventory.RemoveWeapon(item);
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
