using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ItemSettings", fileName = "Item data")]
public class ItemSettings : ScriptableObject
{
    [SerializeField] new private string name;
    [SerializeField] private Sprite icon;
    public string Name { get { return name; } }
    public Sprite Icon { get { return icon; } }
}

