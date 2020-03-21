using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat
{
    [SerializeField] private float _health;
    [SerializeField] private int dieImpulsePower = 3;
    Character _character;
    GameObject _characterObject;
    FieldOfView _characterField;
    Rigidbody _characterRigidbody;

    public CharacterCombat(float health, GameObject character)
    {
        _health = health;

        _characterObject = character;
        _character = character.GetComponent<Character>();
        _characterField = character.GetComponent<FieldOfView>();
        _characterRigidbody = character.GetComponent<Rigidbody>();
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        Debug.Log($"Health: {_health}/{damage}");
    }

    public void Tik()
    {
        if (_health <= 0)
        {
            _health = 0;
            Die();
        }
    }

    public void Die()
    {
        // set character die
        _character.isAlive = false;
        //disable visual
        //_characterField.viewRadius = 0;
        //_characterField.enabled = false;
        GameObject fieldMesh = _characterObject.transform.GetChild(0).gameObject;
        fieldMesh.SetActive(false);
        // enable rigidbody gravity
        _characterRigidbody.useGravity = true;
        _characterRigidbody.freezeRotation = false;
        // add impulse for fun drop
        Vector3 impulseVelocity = _character.transform.forward * dieImpulsePower;
        _characterRigidbody.AddForce(impulseVelocity, ForceMode.Impulse);
    }
}
