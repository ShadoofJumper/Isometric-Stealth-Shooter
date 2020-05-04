using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponSettings settings;
    [SerializeField] private GameObject     bulletOrigin;

    [Header("Parameters for laser")]
    [SerializeField] private float          laserWidth;
    [SerializeField] private float          laserDistance;
    // use for mask
    [SerializeField] private LineRenderer   laserLineRender;
    [SerializeField] private LayerMask      laserObsticalsMask;

    private bool        isStoreEmpty = true;
    private int         ammoStoreMax;
    private int         ammoAmountInStore;
    private int         ammoAmount;
    private Transform   shootSpot;
    private Vector3     shootDirection;
    private float       timeToFire;

    public bool IsStoreEmpty        => isStoreEmpty;
    public int CurrentAmmoInStore   => ammoAmountInStore;
    public int CurrentAmmoAmmount   => ammoAmount;
    public WeaponSettings WeaponSettings    { get { return settings; } set { settings = value; } }
    public Transform ShootSpot              { get { return shootSpot; } set { shootSpot = value; } }

    // queue for bullet pool
    private Queue<GameObject> bulletPool = new Queue<GameObject>();
    private int bulletPoolCounter;
    private GameObject bulletParent;
    private Dictionary<GameObject, Bullet> bulletDict = new Dictionary<GameObject, Bullet>();

    //weapom character
    Character   weaponChar;
    ICharacterInput inputChar;

    private void Awake()
    {
        ammoStoreMax        = settings.AmmoStoreMax;
        ammoAmountInStore   = settings.AmmoAmountInStore;
        ammoAmount          = settings.AmmoAmount;
        //set default
        shootSpot           = transform;
        shootDirection      = transform.forward;

        if (ammoAmountInStore > 0)
            isStoreEmpty = false;
    }

    public void WeaponInitialized(Character weaponChar, WeaponSettings weaponSettings, Transform shootSpot)
    {
        this.shootSpot      = shootSpot;
        this.settings       = weaponSettings;
        this.weaponChar     = weaponChar;
        this.inputChar      = weaponChar.CharacterInput;
    }

    private Vector3 CalculateShootDirection(Character weaponChar)
    {
        if (!weaponChar.isPlayer)
            return inputChar.LookDirection;

        return Vector3.Normalize(inputChar.PointToLook - shootSpot.position);
    }


    //TEST
    private void Update()
    {
        if(weaponChar!=null)
            shootDirection = CalculateShootDirection(weaponChar);
    }


    void Start()
    {
        // create empty object for future bullets
        bulletParent        = new GameObject();
        bulletParent.name   = "Bullets weapon:" + gameObject.name;

        if (laserLineRender != null)
        {
            laserLineRender.useWorldSpace = true;
            laserLineRender.startWidth    = laserWidth;
            laserLineRender.endWidth      = laserWidth;
        }
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



    public void ShootExtra()
    {
        Vector3 startPoint = shootSpot.position;
        Vector3 finisPoint = shootSpot.position + shootDirection * laserDistance;

        laserLineRender.SetPosition(0, startPoint);
        laserLineRender.SetPosition(1, finisPoint);

        // shoot raycast
        Vector3 dir = (finisPoint - startPoint).normalized;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, laserDistance, laserObsticalsMask))
        {
            laserLineRender.SetPosition(1, hit.point);
        }
    }

    private void Shoot()
    {        
        // create or take from pull
        Bullet bullet       = bulletPool.Count != 0 ? GetBullet() : CreateBullet();
        bullet.SpeedMove    = settings.Speed;
        bullet.Velocity     = shootDirection;
        bullet.StartBulletMove();
    }

    private Bullet CreateBullet()
    {
        GameObject bulletObject = Instantiate(bulletOrigin, shootSpot.position, Quaternion.identity);
        // set parent for groupe
        bulletObject.transform.SetParent(bulletParent.transform);
        // program add script to bullet, in future script can be change depend on bullet type
        Bullet bullet           = bulletObject.AddComponent<Bullet>();
        bullet.Damage           = settings.Damage;
        bullet.OnDestroyBullet  = OnBulletDestroy;
        bullet.name             = "Bullet_" + bulletPoolCounter;
        bulletDict.Add(bulletObject, bullet);
        bulletPoolCounter++;
        return bullet;
    }

    private Bullet GetBullet()
    {
        // take from start of pool
        GameObject bulletObject = bulletPool.Dequeue();
        // get bullet comp
        Bullet bullet = bulletDict[bulletObject];
        bulletObject.SetActive(true);
        bulletObject.transform.position = shootSpot.position;

        return bullet;
    }

    public virtual void RightButtonDown()
    {
        // turn on laser
        laserLineRender.enabled = true;
    }

    public virtual void RightButtonUp()
    {
        // turn on laser
        laserLineRender.enabled = false;
    }

    public virtual void RightButtonHold()
    {
        // turn on laser
        ShootExtra();
    }

    public virtual void LeftButtonHold()
    {
        // check if time to shoot
        if (Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / settings.FireRate;
            if (ammoAmountInStore > 0)
            {
                ammoAmountInStore -= 1;
                Shoot();
            }
            else
            {
                isStoreEmpty = true;
            }
        }
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
