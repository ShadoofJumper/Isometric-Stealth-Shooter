using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Weapon/Settings", fileName = "Weapon parametrs")]
public class WeaponSettings : EquipmentSettings
{
    //weapon type 1 - main, 2 - add, 3 - extra
    [SerializeField] private WeaponType weaponPlace;
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


    #region Properties
    public float Speed      { get { return bulletSpeed; }}
    public float FireRate { get { return fireRate; }}
    public float Reload     { get { return reload; } }
    public float Damage     { get { return damage; } }
    public int AmmoAmountInStore { get { return ammoAmountInStore; } }
    public int AmmoAmount   { get { return ammoAmount; } }
    public int AmmoStoreMax { get { return ammoStoreMax; } }
    // weapon prefab props
    public GameObject WeaponModel { get { return weaponModel; } }
    public int WeaponPlaceNumber { get { return (int)weaponPlace; } }
    #endregion

}

public enum WeaponType {Main, Second, Extra}
