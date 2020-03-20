using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponSettings settings;
    [SerializeField] private GameObject bulletOrigin;
    [SerializeField] private Transform shootSpot;
    private Vector3 shootSpotPos;
    private int bulletCounter = 0;

    // queue for bullet pool
    private Queue<Bullet> bulletPool = new Queue<Bullet>();
    //event for bullet destroy
    private 

    void Start()
    {
        //shootSpotPos = shootSpot.position;
        Debug.Log("forward: " + transform.forward);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LeftButtonShoot();
        }
        if (Input.GetMouseButtonDown(1))
        {
            RightButtonShoot();
        }
    }


    public virtual void LeftButtonShoot()
    {
        Debug.Log("LeftButtonShoot");
        //create bullet object
        GameObject bulletObject = Instantiate(bulletOrigin, shootSpot.position, Quaternion.identity);
        // program add script to bullet, in future script can be change depend on bullet type
        Bullet bullet = bulletObject.AddComponent<Bullet>();
        bullet.SpeedMove = settings.Speed;
        bullet.Velocity = transform.forward;
        bullet.OnDestroyBullet = OnBulletDestroy;
        bullet.name = "Bullet_"+ bulletCounter;
        // add bullet to pool
        bulletPool.Enqueue(bullet);
        bulletCounter++;
    }

    public virtual void RightButtonShoot()
    {
        Debug.Log("RightButtonShoot");
    }

    //call when bullet destroy
    private void OnBulletDestroy(Bullet destroyBullet)
    {
        // remove from queue
        bulletPool.Enqueue(destroyBullet);
        Debug.Log("Remove: "+ destroyBullet.name);
    }

}
