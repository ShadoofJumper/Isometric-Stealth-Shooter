using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CharacterEquipment))]
public class CharacterInventory : MonoBehaviour
{
    public int itemsMaxNumber = 5;
    // weapons paramms
    public Weapon[] weapons = new Weapon[3];
    public List<Item> items = new List<Item>();

    public delegate void OnModifyInventory();
    public OnModifyInventory    onModifyInventory;
    private int                 currentMainWeapon = 99;
    private CharacterEquipment  characterEquipment;

    public int CurrentMainWeapon                    => currentMainWeapon;
    public CharacterEquipment CharacterEquipment    => characterEquipment;

    private void Awake()
    {
        characterEquipment  = GetComponent<CharacterEquipment>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetMainWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetMainWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetMainWeapon(2);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            DropWeapon();
        }
    }

    public bool AddItem(Item item)
    {
        //if (CheckCanPutItemInWeapon(item))
        //{
        //    AddWeapon(item as Weapon);
        //    item.Inventory = this;
        //    return true;
        //}

        if (items.Count < itemsMaxNumber)
        {
            items.Add(item);
            item.Inventory = this;
            if (onModifyInventory != null)
                onModifyInventory.Invoke();
            return true;
        }
        return false;
    }

    public void RemoveItem(Item item)
    {
        item.Inventory = null;
        items.Remove(item);
        if (onModifyInventory != null)
            onModifyInventory.Invoke();
    }

    public void AddWeapon(Weapon weapon)
    {
        weapon.Inventory = this;
        int weaponPlaceNumber = weapon.WeaponSettings.WeaponPlaceNumber;
        Weapon oldWeapon = null;
        if (weapons[weaponPlaceNumber]!=null)
        {
            oldWeapon = weapons[weaponPlaceNumber];
            AddItem(oldWeapon);
        }
        weapons[weaponPlaceNumber] = weapon;
        UpdateMainWeapon();
        if (onModifyInventory != null)
            onModifyInventory.Invoke();
    }

    public void RemoveWeapon(Weapon weapon)
    {
        int weaponPlaceNumber = weapon.WeaponSettings.WeaponPlaceNumber;
        //set second 
        weapon.Inventory = null;
        weapons[weaponPlaceNumber] = null;
        UpdateMainWeapon();
        if (onModifyInventory != null)
            onModifyInventory.Invoke();
    }

    // check if item is weapon and slot is empty
    private bool CheckCanPutItemInWeapon(Item newItem)
    {
        Weapon weapon = newItem as Weapon;
        if (weapon!=null)
        {
            int weaponType = weapon.WeaponSettings.WeaponPlaceNumber;
            if (weapons[weaponType]==null)
            {
                return true;
            }
        }
        return false;
    }

    //set as main weapon weapos with bigger priority, (min number -> bigger priority)
    private void UpdateMainWeapon()
    {
        Debug.Log("UpdateMainWeapon");
        int mainWeaponNumber = 99;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                int weeaponNumber = weapons[i].WeaponSettings.WeaponPlaceNumber;
                if (weeaponNumber < mainWeaponNumber)
                    mainWeaponNumber = weeaponNumber;
            }
 
        }
        if (mainWeaponNumber != currentMainWeapon)
        {
            SetMainWeapon(mainWeaponNumber);
        }
    }

    public void SetMainWeapon(int fromSlot)
    {
        Debug.Log("SetMainWeapon: "+ fromSlot);
        if (fromSlot == 99)
        {
            currentMainWeapon = fromSlot;
            if (onModifyInventory != null)
                onModifyInventory.Invoke();
            return;
        }

        if (weapons[fromSlot] != null)
        {
            currentMainWeapon = fromSlot;
            //equip in slot new weapon
            characterEquipment.Equip(weapons[currentMainWeapon]);
        }
        if (onModifyInventory != null)
            onModifyInventory.Invoke();
    }

    private Item FindWeaponReplacement(int weaponType)
    {
        foreach (Item item in items)
        {
            WeaponSettings weaponSettings = item.ItemSettings as WeaponSettings;
            if (weaponSettings && weaponSettings.WeaponPlaceNumber == weaponType)
            {

                return item;
            }
        }
        return null;
    }

    private void DropWeapon()
    {
        int currentWeaponType = weapons[currentMainWeapon].WeaponSettings.WeaponPlaceNumber;
        //TO DO

        //drop weapon in main slot
        RemoveWeapon(weapons[currentMainWeapon]);
        //check in inventory the same type of weapon and add if have
        Item weaponFromInventory = FindWeaponReplacement(currentWeaponType);
        if (weaponFromInventory!=null)
        {
            AddWeapon(weaponFromInventory as Weapon);
            RemoveItem(weaponFromInventory);
        }
    }
}
