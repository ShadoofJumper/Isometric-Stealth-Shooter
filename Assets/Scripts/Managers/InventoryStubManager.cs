using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryStubManager : MonoBehaviour
{

    #region Singlton
    public static InventoryStubManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Try create another instance of game manager!");
        }

    }
    #endregion

    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private Transform  handToMove;

    public List<WeaponSettings> items = new List<WeaponSettings>();
    private CharacterIKMover playerIKMover;
    private Character player;
    private void Start()
    {
        player = SceneController.instance.player;
        GameObject playerObject = player.gameObject;
        playerIKMover = playerObject.GetComponentInChildren<CharacterIKMover>();
    }

    public void Additem(WeaponSettings weaponSettings)
    {
        items.Add(weaponSettings);
        Equip(weaponSettings);
    }

    public void Equip(WeaponSettings weaponSettings)
    {
        CreateWeapon(weaponSettings);
    }

    public Weapon CreateWeapon(WeaponSettings weaponSettings)
    {
        // instantiate weapon
        GameObject weaponMain   = Instantiate(weaponPrefab, handToMove);
        GameObject weaponModel  = Instantiate(weaponSettings.WeaponModel, weaponMain.transform);
        Weapon weaponComp       = weaponMain.GetComponent<Weapon>();
        weaponMain.name         = "Weapon";
        weaponComp.WeaponSettings = weaponSettings;
        weaponComp.ShootSpot    = weaponModel.transform.GetChild(1);

        SimpleTransform rightHandSpot   = new SimpleTransform(weaponSettings.RightHandSpotPos, Quaternion.Euler(weaponSettings.RightHandSpotRot));
        Transform leftHandSpotTr        = weaponModel.transform.GetChild(0);
        SimpleTransform leftHandSpot    = new SimpleTransform(leftHandSpotTr.position, leftHandSpotTr.rotation);

        // add weapon to character
        player.CharacterWeapon = weaponComp;

        // update player hands position on weapon
        playerIKMover.UpdateWeaponHandSpots(leftHandSpotTr, rightHandSpot);


        return weaponComp;
    }

}
