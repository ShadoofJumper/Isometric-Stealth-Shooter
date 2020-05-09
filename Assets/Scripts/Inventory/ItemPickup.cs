using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{
    [SerializeField] private Item item;

    public override bool Interact()
    {
        return Pickup();
    }

    private bool Pickup()
    {
        Debug.Log("We get item: " + item.ItemSettings.Name);
        bool isAdd = SceneController.instance.playerInventory.AddItem(item);
        if(isAdd)
            Destroy(gameObject);
        return isAdd;
    }
}
