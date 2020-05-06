using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTWEAPONGET : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            InventoryStubManager.instance.Additem("M107");
        }
    }
}
