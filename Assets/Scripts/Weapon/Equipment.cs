using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Item
{
    public EquipmentSettings EquipSettings => settings as WeaponSettings;
    private int equipItem = 2;

    public override void Use()
    {
        Debug.Log("Use equipment! "+ EquipSettings.Name);
    }
}
