using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDoorArea : MonoBehaviour
{
    public int enterId;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Character>())
        {
            GameManager.instance.DoorwayTriggerEnter(enterId);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Character>())
        {
            GameManager.instance.DoorwayTriggerExit(enterId);
        }
    }
}
