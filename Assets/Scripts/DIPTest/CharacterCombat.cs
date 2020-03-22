using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat
{
    private float _health;
    private int dieImpulsePower = 3;
    private float dieFallDelay = 2.0f;
    private Character _character;
    private GameObject _characterObject;
    private FieldOfView _characterField;
    private Rigidbody _characterRigidbody;
    private MonoBehaviour _myMonoBehaviour;

    public CharacterCombat(float health, GameObject character, MonoBehaviour myMonoBehaviour)
    {
        _health             = health;
        _characterObject    = character;
        _myMonoBehaviour    = myMonoBehaviour;

        _character          = character.GetComponent<Character>();
        _characterField     = character.GetComponent<FieldOfView>();
        _characterRigidbody = character.GetComponent<Rigidbody>();
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        _health = _health < 0 ? 0 : _health;
        Debug.Log($"Health: {_health}/{damage}");
    }

    public void Tik()
    {
        if (_health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // set character die
        _character.isAlive = false;
        //disable visual
        GameObject fieldMesh = _characterObject.transform.GetChild(0).gameObject;
        fieldMesh.SetActive(false);
        _characterField.enabled = false;
        _myMonoBehaviour.StartCoroutine(ShowFallAnim(dieFallDelay));
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
