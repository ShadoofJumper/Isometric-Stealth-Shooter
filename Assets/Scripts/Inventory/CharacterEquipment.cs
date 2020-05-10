using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipment : MonoBehaviour
{

    #region Singlton
    public static CharacterEquipment instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public Equipment[] currentEquipment;
    [SerializeField] private SkinnedMeshRenderer playerBodySkin;
    private CharacterInventory inventory;

    public delegate void OnEquipmentChanged(Equipment oldItem, Equipment newItem);
    public OnEquipmentChanged onEquipmentChanged;

    // Start is called before the first frame update
    void Start()
    {
        // get lenght of equipment
        int numSlots        = System.Enum.GetNames(typeof(EquipmentType)).Length;
        currentEquipment    = new Equipment[numSlots];
        inventory           = GetComponent<CharacterInventory>();
    }

    
    public void Equip(Equipment equipment)
    {
        int equipmentId = (int)equipment.EquipSettings.EquipType;
        if (equipment == currentEquipment[equipmentId])
        {
            Debug.Log("Put the same equip!");
            return;
        }
        //unequip old item
        Equipment oldEquipment = Unequip(equipmentId);
        if(oldEquipment!=null)
            Debug.Log("old equip: " + oldEquipment.ItemSettings.Name);
        Debug.Log("new equip: " + equipment.ItemSettings.Name);

        //must be after unequip
        currentEquipment[equipmentId] = equipment;
        if (onEquipmentChanged != null)
            onEquipmentChanged.Invoke(oldEquipment, equipment);
    }

    private bool CheckIsEquipmentWeapon(int slotId)
    {
        return slotId == (int)EquipmentType.Weapon;
    }

    //isSwaped mean this equip is swapped by other
    public Equipment Unequip(int slotId)
    {
        if (currentEquipment[slotId] !=null)
        {
            Equipment itemInEquip = currentEquipment[slotId];
            // add item back to inventory if current item not weapon
            if (!CheckIsEquipmentWeapon(slotId))
                inventory.AddItem(itemInEquip);

            currentEquipment[slotId] = null;
            if (onEquipmentChanged != null)
                onEquipmentChanged.Invoke(null, itemInEquip);
            return itemInEquip;
        }
        return null;
    }

    public void UnequipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
            if(CheckIsEquipmentWeapon(i))
                inventory.SetMainWeapon(99);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UnequipAll();
        }
    }
}
