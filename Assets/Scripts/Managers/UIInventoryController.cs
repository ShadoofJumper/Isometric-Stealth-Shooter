using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryController : MonoBehaviour
{
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform weaponSlotParent;
    [SerializeField] private GameObject uiInventory;

    private CharacterInventory characterInventory;

    private InventorySlot[] inventorySlots;
    private WeaponSlot[] weaponSlots;
    private bool isShowInventory = true;
    [SerializeField] private Color disableWeaponColor;
    [SerializeField] private Color enableWeaponColor;

    private void Awake()
    {
        characterInventory = SceneController.instance.playerInventory;

        characterInventory.onModifyInventory += UpdateUIInventory;
        characterInventory.onModifyInventory += UpdateUIMainWeapon;

        inventorySlots = inventorySlotParent.GetComponentsInChildren<InventorySlot>();
        weaponSlots = weaponSlotParent.GetComponentsInChildren<WeaponSlot>();
    }


    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            isShowInventory = !isShowInventory;
            uiInventory.SetActive(isShowInventory);
        }
    }

    public void UpdateUIMainWeapon()
    {
        //update weapon
        for (int j = 0; j < weaponSlots.Length; j++)
        {
            if (characterInventory.weapons[j] != null)
            {
                if (j == SceneController.instance.playerInventory.CurrentMainWeapon)
                {
                    weaponSlots[j].SetMainSlot(enableWeaponColor);
                }
                else
                {
                    weaponSlots[j].SetDefaultSlot(disableWeaponColor);
                }
            }
            else
            {
                weaponSlots[j].SetDefaultSlot(disableWeaponColor);
            }
        }
    }

    private void UpdateUIInventory()
    {
        //update inventory slots
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < characterInventory.items.Count)
            {
                inventorySlots[i].AddItem(characterInventory.items[i]);
            }
            else
            {
                inventorySlots[i].ClearSlot();
            }
        }
        //update weapon
        for (int j = 0; j < weaponSlots.Length; j++)
        {
            if (characterInventory.weapons[j]!=null)
            {
                Weapon item = characterInventory.weapons[j];
                weaponSlots[j].AddItem(item);
            }
            else
            {
                weaponSlots[j].ClearSlot();
            }
        }
    }
}
