using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Equipment
{
    [SerializeField] private WeaponShooter weaponShooter;
    #region properties
    public WeaponShooter WeaponShooter => weaponShooter;
    public WeaponSettings WeaponSettings => settings as WeaponSettings;
    #endregion

    private void Start()
    {
        Debug.Log("Weapon start: " + settings.Name);
    }

    public void CreateWeaponOnScene()
    {
        Debug.Log("Create: "+ settings.Name);
    }

    public override void Use()
    {
        if (inventory != null)
        {
            CharacterInventory _inventory = inventory;
            inventory.RemoveItem(this);
            _inventory.AddWeapon(this);
        }
    }

    public void RemoveFromWeapons()
    {
        Debug.Log("RemoveFromWeapons: " + settings.Name);
        if (inventory!=null)
        {
            CharacterInventory _inventory = inventory;
            inventory.RemoveWeapon(this);
            _inventory.AddItem(this);
        }
    }

    public void EquipWeapon()
    {
        Debug.Log("Weapon. EquipWeapon");
        if (inventory != null)
        {
            Debug.Log("Weapon. SetMainWeapon");
            inventory.SetMainWeapon(WeaponSettings.WeaponPlaceNumber);
        }
    }
}
