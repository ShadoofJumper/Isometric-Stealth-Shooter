using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class ItemPickup : Interactable
{
    private Item item;
    //create item object on scene
    //item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity, SceneController.instance.InteractableParent);

    private void Start()
    {
        item = GetComponent<Item>();
    }

    public override bool Interact()
    {
        return Pickup();
    }

    private bool Pickup()
    {
        bool isAdd = SceneController.instance.playerInventory.AddItem(item);
        if(isAdd)
            Destroy(gameObject);
        return isAdd;
    }
}
