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
        //unequip old item
        Equipment oldEquipment = Unequip(equipmentId);
        //must be after unequip
        currentEquipment[equipmentId] = equipment;
        if (onEquipmentChanged != null)
            onEquipmentChanged.Invoke(oldEquipment, equipment);
    }

    private bool CheckIsEquipmentWeapon(int slotId)
    {
        WeaponSettings weaponSettings = currentEquipment[slotId].EquipSettings as WeaponSettings;
        return weaponSettings != null ? true : false;
    }

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
