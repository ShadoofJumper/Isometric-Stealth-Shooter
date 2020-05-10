using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private protected ItemSettings settings;
    private protected CharacterInventory inventory;

    public ItemSettings ItemSettings => settings;
    public CharacterInventory Inventory { get { return inventory; } set { inventory = value; } }

    public virtual void Use()
    {
        Debug.Log("Use item!: " + settings.Name);
    }

    public virtual void RemoveItem()
    {
        Debug.Log("RemoveItem");
    }
}
