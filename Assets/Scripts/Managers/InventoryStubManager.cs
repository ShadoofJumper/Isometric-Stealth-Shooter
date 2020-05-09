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
        OnAwake();
    }
    #endregion

    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private Transform  handToMove;
    [SerializeField] private List<WeaponSettings> wSettings;

    public List<WeaponSettings> items = new List<WeaponSettings>();
    private CharacterIKMover playerIKMover;
    private Character player;

    private void OnAwake()
    {
        player = SceneController.instance.player;
        GameObject playerObject = player.gameObject;
        playerIKMover = playerObject.GetComponentInChildren<CharacterIKMover>();
    }

    private WeaponSettings GetGunSetting(string name)
    {
        foreach (WeaponSettings ws in wSettings)
        {
            if (ws.Name == name)
                return ws;
        }
        return null;
    }

    public void Additem(string weaponName)
    {
        WeaponSettings wSettings = GetGunSetting(weaponName);
        if (items.Count == 0 && wSettings!= null)
        {
            items.Add(wSettings);
            Equip(wSettings);
        }
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
        //weaponComp.WeaponInitialized(player, weaponSettings, weaponModel.transform.GetChild(1));
        // add weapon to character
        //player.CharacterWeapon = weaponComp;

        return weaponComp;
    }

}
