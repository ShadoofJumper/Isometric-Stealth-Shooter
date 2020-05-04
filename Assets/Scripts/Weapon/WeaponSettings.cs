using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Settings", fileName = "Weapon parametrs")]
public class WeaponSettings : ScriptableObject
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float reload;
    [SerializeField] private float damage;
    // current ammo in store
    [SerializeField] private int ammoAmountInStore;
    [SerializeField] private int ammoAmount;
    [SerializeField] private int ammoStoreMax;
    // weapon prefab settings
    [SerializeField] private GameObject weaponModel;
    [Header("Paramms for weapon right hand placement")]
    [SerializeField] private Vector3 weaponRightHandPos;
    [SerializeField] private Vector3 weaponRightHandRot;

    public float Speed      { get { return bulletSpeed; }}
    public float FireRate { get { return fireRate; }}
    public float Reload     { get { return reload; } }
    public float Damage     { get { return damage; } }
    public int AmmoAmountInStore { get { return ammoAmountInStore; } }
    public int AmmoAmount   { get { return ammoAmount; } }
    public int AmmoStoreMax { get { return ammoStoreMax; } }
    // weapon prefab props
    public Vector3 RightHandSpotPos { get { return weaponRightHandPos; }}
    public Vector3 RightHandSpotRot { get { return weaponRightHandRot; }}
    public GameObject WeaponModel { get { return weaponModel; } }


}
