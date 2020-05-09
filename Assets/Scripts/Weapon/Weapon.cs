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

    public override void Use()
    {
        Debug.Log("Use weapon! "+ WeaponSettings.Name);
        if (inventory != null)
        {
            CharacterInventory _inventory = inventory;
            inventory.RemoveItem(this);
            _inventory.AddWeapon(this);
        }
    }

}
