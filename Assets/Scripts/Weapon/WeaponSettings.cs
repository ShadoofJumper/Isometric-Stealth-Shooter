using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Settings", fileName = "Weapon parametrs")]
public class WeaponSettings : ScriptableObject
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float shootDelay;
    [SerializeField] private float reload;
    [SerializeField] private float damage;

    public float Speed { get { return bulletSpeed; }}
    public float ShootDelay { get { return shootDelay; } }
    public float Reload { get { return reload; } }
    public float Damage { get { return damage; } }
}
