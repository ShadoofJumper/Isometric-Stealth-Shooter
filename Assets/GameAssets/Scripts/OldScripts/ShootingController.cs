using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    [SerializeField] private GameObject bulletOrigin;
    [SerializeField] private GameObject shootSpot;

    void Start()
    {
        
    }

    void Update()
    {
        // on right mouse click
        if (Input.GetMouseButtonDown(0))
        {
            SpawnBoolet();
        }
    }

    //create raycast from player to player direction
    private void SpawnBoolet()
    {
        //
        if (shootSpot != null)
        {
            bulletOrigin = Instantiate(bulletOrigin, shootSpot.transform.position, Quaternion.identity);
            Vector3 shootDir = transform.forward;
            bulletOrigin.transform.rotation = Quaternion.LookRotation(shootDir);
        }
        else
        {
            Debug.Log("Error no bullet add!!!");
        }
    }
}
