using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Settings", fileName = "Weapon parametrs")]
public class WeaponSettings : ScriptableObject
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float shootDelay;
    [SerializeField] private float reload;

    public float Speed { get { return bulletSpeed; }}
    public float ShootDelay { get { return shootDelay; } }
    public float Reload { get { return reload; } }
}
