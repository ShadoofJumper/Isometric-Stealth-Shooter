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
    Character weaponChar;

    private void Awake()
    {
        ammoStoreMax        = settings.AmmoStoreMax;
        ammoAmountInStore   = settings.AmmoAmountInStore;
        ammoAmount          = settings.AmmoAmount;
        //set default
        shootSpot           = transform;
        shootDirection      = transform.forward;

        if (ammoAmountInStore > 0)
        {
            isStoreEmpty = false;
        }
    }

    public void WeaponInitialized(Character weaponChar, WeaponSettings weaponSettings, Transform shootSpot)
    {
        this.shootSpot      = shootSpot;
        this.settings       = weaponSettings;
        this.weaponChar     = weaponChar;
        this.shootDirection = CalculateShootDirection(weaponChar);
    }

    private Vector3 CalculateShootDirection(Character weaponChar)
    {
        Vector3 dir;
        // if ai
        if (!weaponChar.isPlayer) {
            Debug.Log("aaa1");
            dir = weaponChar.CharacterInput.LookDirection;
        } else {
            Debug.Log("aaa2");
            PlayerInput playerInput = weaponChar.CharacterInput as PlayerInput;
            Vector3 pointToLook     = playerInput.PointToLook;
            dir = Vector3.Normalize(playerInput.PointToLook - shootSpot.position);
        }
        return dir;
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


    public virtual bool LeftButtonShoot()
    {
        if (ammoAmountInStore > 0)
        {
            ammoAmountInStore -= 1;
            Shoot();
        }
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

    //TEST
    private void Update()
    {
        //ShootLaser();
    }

    public void ShootLaser()
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
        Bullet bullet = bulletPool.Count != 0 ? GetBullet() : CreateBullet();

        bullet.SpeedMove    = settings.Speed;
        bullet.Velocity     = shootSpot.forward;
        bullet.StartBulletMove();
    }

    private Bullet CreateBullet()
    {
        GameObject bulletObject = Instantiate(bulletOrigin, shootSpot.position, Quaternion.identity);
        // set parent for groupe
        bulletObject.transform.SetParent(bulletParent.transform);
        // program add script to bullet, in future script can be change depend on bullet type
        Bullet bullet = bulletObject.AddComponent<Bullet>();
        // add to dict
        bulletDict.Add(bulletObject, bullet);
        bullet.Damage = settings.Damage;
        // add delegate to bullet action
        bullet.OnDestroyBullet = OnBulletDestroy;
        bullet.name             = "Bullet_" + bulletPoolCounter;
        // add bullet to end of pool
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
        ShootLaser();
    }


    public virtual void RightButtonHold()
    {
        // turn on laser
        ShootLaser();
    }

    public virtual void RightButtonUp()
    {
        // turn on laser
        laserLineRender.enabled = false;
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
