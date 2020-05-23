using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterCombat
{
    private float           _health;
    private int             dieImpulsePower = 200;
    private float           dieFallDelay    = 1.0f;
    private Weapon          _weapon;
    private WeaponShooter   _weaponShooter;
    private Character       _character;
    private NavMeshAgent    _agent;
    private GameObject      _characterObject;
    private FieldOfView     _characterField;
    private Rigidbody       _characterRigidbody;
    private CharacterAnimationController _charAnim;

    private MonoBehaviour   _myMonoBehaviour;
    private ICharacterInput _input;
    private DamageHit       lastHit;

    private bool isShootExtra   = false;
    private bool isShootMain    = false;
    private float weaponGetSpeed = 0.3f;

    public CharacterCombat(ICharacterInput input, float health, GameObject character, MonoBehaviour myMonoBehaviour)
    {
        _health             = health;
        _input              = input;
        _characterObject    = character;
        _myMonoBehaviour    = myMonoBehaviour;
        _character          = character.GetComponent<Character>();
        _characterField     = character.GetComponent<FieldOfView>();
        _characterRigidbody = character.GetComponent<Rigidbody>();
        _agent              = character.GetComponent<NavMeshAgent>();
        _charAnim           = character.GetComponentInChildren<CharacterAnimationController>();

        if (_character.isPlayer)
        {
            UIController.instance.UpdateHealthUI(_health);
        }

    }

    public void SetCombatWeapon(Weapon weapon)
    {
        _weapon         = weapon;
        _weaponShooter  = weapon.WeaponShooter;
        UIController.instance.UpdateAmmoUI(_weaponShooter.CurrentAmmoInStore, _weaponShooter.CurrentAmmoAmmount);
    }

    public void TakeDamage(float damage, ContactPoint hitPoint = new ContactPoint(), Vector3 hitDirection = new Vector3())
    {
        _health -= damage;
        _health = _health < 0 ? 0 : _health;

        lastHit = new DamageHit(hitPoint.point, hitDirection);

        if (_character.isPlayer)
        {
            UIController.instance.UpdateHealthUI(_health);
        }
    }

    private void ShootMain(MouseInput leftButton)
    {
        if (leftButton.down) {
            _charAnim.EnableAim(weaponGetSpeed, delegate { isShootMain = true; });
        }
        if (leftButton.up) {
            _charAnim.DisableAim(weaponGetSpeed);
            isShootMain = false;
        }

        if (leftButton.press && (isShootMain || isShootExtra))
        {
            _weaponShooter.Shoot();
            if (_character.isPlayer)
                UIController.instance.UpdateAmmoUI(_weaponShooter.CurrentAmmoInStore, _weaponShooter.CurrentAmmoAmmount);
            if (_weaponShooter.IsStoreEmpty && _weaponShooter.CurrentAmmoAmmount > 0)
                UIController.instance.ShowAmmoHint();
        }
    }



    private void ShootExtra(MouseInput rightButton)
    {

        if (rightButton.down)
        {
            _charAnim.EnableAim(weaponGetSpeed, delegate { _weaponShooter.TurnOnLazer(); isShootExtra = true; });
        }

        if (rightButton.up)
        {
            _charAnim.DisableAim(weaponGetSpeed);
            _weaponShooter.TurnOffLazer();
            isShootExtra = false;
        }

        if (rightButton.press && isShootExtra)
        {
            _weaponShooter.ShootExtra();
        }
    }


    // TO Do correct bad code call input shoot
    public void DoWaeponAction(MouseInput[] mouseButtonsState)
    {
        if (_weaponShooter == null)
            return;

        ShootMain(mouseButtonsState[0]);
        ShootExtra(mouseButtonsState[1]);
    }


    public void OnPlayerFound(string failtext, string failDescription)
    {
        if (MissionManager.instance.IsSilenceRequireMain)
        {
            // show some animation about faill

            // show warning about fail
            GameManager.instance.FailLevel(failtext, failDescription);
        }
    }

    private bool SearchPlayerInTargets(List<Transform> targetsInField)
    {
        // search for player
        foreach (Transform target in targetsInField)
        {
            if (SceneController.instance.charactersOnScene[target].isPlayer)
            {
                return true;
            }
        }
        return false;
    }


    public void Tik()
    {
        if (!_character.isPlayer)
        {
            if (SearchPlayerInTargets(_characterField.TargetsInField))
            {
                OnPlayerFound("FAIL MISSION!", "You were caught!");
            }
        }

        if (_health <= 0)
        {
            Die();
        }

        if (_weaponShooter != null)
        {
            // call action left
            DoWaeponAction(_input.MouseInput);

            if (_input.IsPressReload)
            {
                _weaponShooter.Reload();
                UIController.instance.HideAmmoHint();

                if (_character.isPlayer)
                {
                    UIController.instance.UpdateAmmoUI(_weaponShooter.CurrentAmmoInStore, _weaponShooter.CurrentAmmoAmmount);
                }
            }
        }

    }

    public void Die()
    {
        // set character die
        _character.isAlive = false;
        // disable visual field
        GameObject fieldMesh = _characterObject.transform.GetChild(0).gameObject;
        fieldMesh.SetActive(false);
        _characterField.enabled = false;
        // disable nav agent
        _character.DestroyNavMeshAgent();
        // start fall
        _myMonoBehaviour.StartCoroutine(ShowFallAnim(dieFallDelay));
    }


    IEnumerator ShowFallAnim(float delayFall)
    {
        // enable rigidbody gravity
        _characterRigidbody.useGravity      = true;
        _characterRigidbody.freezeRotation  = false;
        _character.GetComponent<Collider>().enabled = false;
        _charAnim.EnableRagdoll();
        yield return 0;
        // add impulse for fun drop
        _charAnim.HitRagdoll(lastHit, dieImpulsePower);
        _charAnim.PauseCharAnim();
    }

}

public struct DamageHit
{
    public Vector3 point;
    public Vector3 dir;
    public DamageHit(Vector3 _point, Vector3 _dir)
    {
        point   = _point;
        dir     = _dir;
    }
}
