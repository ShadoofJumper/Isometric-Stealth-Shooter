using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Equipment", fileName = "Equip data")]
public class EquipmentSettings : ItemSettings
{
    [SerializeField] private EquipmentType equipType;
    [SerializeField] private MeshRegion[] meshRegions;
    [SerializeField] private bool isDefault;

    public EquipmentType EquipType  { get { return equipType; } }
    public MeshRegion[] MeshRegions { get { return meshRegions; } }
    public bool IsDefault { get { return isDefault; } }

}

public enum EquipmentType { Weapon, Head, Body, Legs }
public enum MeshRegion { Legs, Arms, Torso }
