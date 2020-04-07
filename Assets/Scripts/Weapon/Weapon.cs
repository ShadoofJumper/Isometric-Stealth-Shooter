using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponSettings settings;
    [SerializeField] private GameObject bulletOrigin;
    [SerializeField] private Transform shootSpot;
    private Vector3 shootSpotPos;


    public bool isStoreEmpty = true;
    private int ammoStoreMax;
    private int ammoAmountInStore;
    private int ammoAmount;

    public int CurrentAmmoInStore => ammoAmountInStore;
    public int CurrentAmmoAmmount => ammoAmount;


    // queue for bullet pool
    private Queue<GameObject> bulletPool = new Queue<GameObject>();
    private int bulletPoolCounter;

    // game object for groupe bullets
    private GameObject bulletParent;
    // dictionary for components of bullets
    private Dictionary<GameObject, Bullet> bulletDict = new Dictionary<GameObject, Bullet>();

    private void Awake()
    {
        ammoStoreMax = settings.AmmoStoreMax;
        ammoAmountInStore = settings.AmmoAmountInStore;
        ammoAmount = settings.AmmoAmount;

        if (ammoAmountInStore > 0)
        {
            isStoreEmpty = false;
        }

    }

    void Start()
    {
        // create emptyyobject for future bullets
        bulletParent = new GameObject();
        bulletParent.name = "Bullet wepone:" + gameObject.name;
    }


    public virtual bool LeftButtonShoot()
    {
        if (ammoAmountInStore > 0)
        {
            // minus ammo
            ammoAmountInStore -= 1;

            Shoot();

        }
        // if after shoot no emmo
        if (ammoAmountInStore == 0)
        {
            isStoreEmpty = true;
            return false;
        }
        return true;
    }

    public void Reload()
    {
        // if store not max
        if (ammoAmountInStore < ammoStoreMax)
        {
            int needAmmo = ammoStoreMax - ammoAmountInStore;
            //if have need ammo then just add, else add all what have
            if (ammoAmount >= needAmmo)
            {
                ammoAmount          -= needAmmo;
                ammoAmountInStore   += needAmmo;
            }
            else
            {
                if (ammoAmount == 0)
                {
                    //Debug.Log("No emmo");
                }
                else
                {
                    ammoAmountInStore += ammoAmount;
                    ammoAmount = 0;

                }
            }
            if (ammoAmountInStore > 0)
            {
                isStoreEmpty = false;
            }
        }
    }

    private void Shoot()
    {
        GameObject bulletObject;
        // if not have bullet in pull then take from pool
        if (bulletPool.Count != 0)
        {
            // take from start of pool
            bulletObject = bulletPool.Dequeue();
            //get bullet
            Bullet bullet = bulletDict[bulletObject];
            //set start position and speed
            bulletObject.SetActive(true);
            bulletObject.transform.position = shootSpot.position;
            bullet.SpeedMove = settings.Speed;
            bullet.Velocity = transform.forward;
            bullet.StartBulletMove();
        }
        else
        {
            bulletObject = Instantiate(bulletOrigin, shootSpot.position, Quaternion.identity);
            // set parent for groupe
            bulletObject.transform.SetParent(bulletParent.transform);
            // program add script to bullet, in future script can be change depend on bullet type
            Bullet bullet = bulletObject.AddComponent<Bullet>();
            // add to dict
            bulletDict.Add(bulletObject, bullet);

            bullet.SpeedMove = settings.Speed;
            bullet.Velocity = transform.forward;
            bullet.Damage = settings.Damage;
            // add delegate to bullet action
            bullet.OnDestroyBullet = OnBulletDestroy;
            bullet.name = "Bullet_" + bulletPoolCounter;
            bullet.StartBulletMove();
            // add bullet to end of pool
            bulletPoolCounter++;
        }
    }

    public virtual void RightButtonShoot()
    {
        Debug.Log("RightButtonShoot");
    }

    //call when bullet destroy
    private void OnBulletDestroy(GameObject destroyBulletObject)
    {
        // remove from queue
        // get bullet
        Bullet destroyBullet = bulletDict[destroyBulletObject];
        // disable bullet
        destroyBulletObject.SetActive(false);
        destroyBulletObject.transform.position = Vector3.zero;
        destroyBullet.SpeedMove = 0;
        // add to pool when complete
        bulletPool.Enqueue(destroyBulletObject);
    }


}
