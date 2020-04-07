using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDoorArea : MonoBehaviour
{
    public int enterId;
    public float closeTime;
    public float openTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Character>())
        {
            GameManager.instance.DoorwayTriggerEnter(enterId, openTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Character>())
        {
            GameManager.instance.DoorwayTriggerExit(enterId, closeTime);
        }
    }
}
