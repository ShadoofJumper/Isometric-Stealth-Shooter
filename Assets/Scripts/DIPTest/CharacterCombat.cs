using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterCombat
{
    private float           _health;
    private int             dieImpulsePower = 3;
    private float           dieFallDelay    = 1.0f;
    private Weapon          _weapon;
    private Character       _character;
    private NavMeshAgent    _agent;
    private GameObject      _characterObject;
    private FieldOfView     _characterField;
    private Rigidbody       _characterRigidbody;
    private Animator        _characterAnimator;

    private MonoBehaviour   _myMonoBehaviour;
    private ICharacterInput _input;

    public CharacterCombat(ICharacterInput input, float health, Weapon weapon, GameObject character, MonoBehaviour myMonoBehaviour)
    {
        _health             = health;
        _input              = input;
        _weapon             = weapon;
        _characterObject    = character;
        _myMonoBehaviour    = myMonoBehaviour;

        _character          = character.GetComponent<Character>();
        _characterField     = character.GetComponent<FieldOfView>();
        _characterRigidbody = character.GetComponent<Rigidbody>();
        _agent              = character.GetComponent<NavMeshAgent>();
        _characterAnimator  = character.GetComponentInChildren<Animator>();

        if (_character.isPlayer)
        {
            UIController.instance.UpdateHealthUI(_health);

            if (_weapon!= null)
            {
                // test
                UIController.instance.UpdateAmmoUI(_weapon.CurrentAmmoInStore, _weapon.CurrentAmmoAmmount);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        _health = _health < 0 ? 0 : _health;

        if (_character.isPlayer)
        {
            //Debug.Log($"Health: {_health}/{damage}");

            UIController.instance.UpdateHealthUI(_health);
        }
    }

    // TO Do correct bad code call input shoot
    public void DoWaeponAction(MouseInput[] mouseButtonsState)
    {
        if (_weapon != null)
        {
            // if click left button
            if (mouseButtonsState[0].down)
            {
                bool isShootSucces =_weapon.LeftButtonShoot();
                // play anim trigger
                if (isShootSucces)
                {
                    _characterAnimator.SetTrigger("isShootTrigger");
                }
                // update after shoot
                if (_character.isPlayer)
                {
                    UIController.instance.UpdateAmmoUI(_weapon.CurrentAmmoInStore, _weapon.CurrentAmmoAmmount);
                }
                // show hint if have ammo and not reload
                if (_weapon.IsStoreEmpty && _weapon.CurrentAmmoAmmount > 0)
                {
                    UIController.instance.ShowAmmoHint();
                }

            }
            else if (mouseButtonsState[1].down)
            {
                _weapon.RightButtonDown();
            }
            else if (mouseButtonsState[1].press)
            {
                _weapon.RightButtonHold();
            }
            else if (mouseButtonsState[1].up)
            {
                _weapon.RightButtonUp();
            }
        }


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

        if (_weapon != null)
        {
            // call action left
            DoWaeponAction(_input.MouseInput);

            if (_input.IsPressReload)
            {
                _weapon.Reload();
                UIController.instance.HideAmmoHint();

                if (_character.isPlayer)
                {
                    UIController.instance.UpdateAmmoUI(_weapon.CurrentAmmoInStore, _weapon.CurrentAmmoAmmount);
                }
            }
        }

    }

    public void Die()
    {
        // set character die
        _character.isAlive = false;
        //disable visual field
        GameObject fieldMesh = _characterObject.transform.GetChild(0).gameObject;
        fieldMesh.SetActive(false);
        _characterField.enabled = false;
        //disable nav agent
        _character.DestroyNavMeshAgent();
        // start fall
        _myMonoBehaviour.StartCoroutine(ShowFallAnim(dieFallDelay));
        // disable anim
        _characterAnimator.SetFloat("Speed", 0);
    }

    IEnumerator ShowFallAnim(float delayFall)
    {
        // enable rigidbody gravity
        _characterRigidbody.useGravity = true;
        _characterRigidbody.freezeRotation = false;
        // add impulse for fun drop
        Vector3 impulseVelocity = _character.transform.forward * dieImpulsePower;
        _characterRigidbody.AddForce(impulseVelocity, ForceMode.Impulse);
        yield return new WaitForSeconds(delayFall);
        // after time turn on kinematick, so character cant move after die
        _characterRigidbody.isKinematic = true;
    }
}
