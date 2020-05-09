using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    private bool isInteracted;
    private bool isSelected;
    private bool isCanInteract;
    
    public virtual bool Interact()
    {
        return false;
    }

    public virtual void Selecting()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isInteracted = false;
            isSelected = true;
        }
    }

    private void Update()
    {
        if (isCanInteract && !isInteracted)
        {
            //TO DO check in player face object
            bool iFace = true;
            if (iFace)
            {
                //to do show gui
                Selecting();
                if (isSelected)
                {
                    bool isInteractSucces = Interact();
                    isInteracted    = isInteractSucces;
                    isSelected      = isInteractSucces;
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponent<Character>();
        if (character && character.isPlayer)
        {
            isCanInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character character = other.GetComponent<Character>();
        if (character && character.isPlayer)
        {
            isCanInteract = false;
        }
    }

}
