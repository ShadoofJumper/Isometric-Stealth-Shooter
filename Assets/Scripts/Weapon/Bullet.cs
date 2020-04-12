using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet: MonoBehaviour
{
    private float       _speedMove;
    private float       _damage = 0;
    private Vector3     _velocity;
    private Rigidbody   _bulletRig;
    private float       delayDestroy = 3.0f;

    #region [Properties]

    public float SpeedMove {
        get { return _speedMove; }
        set {
            if (value > 1) { _speedMove = value; } else { _speedMove = 1; }
        }
    }
    public Vector3 Velocity
    {
        get { return _velocity; }
        set { _velocity = value; }
    }

    public float Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    public float DelayDestroy
    {
        get { return delayDestroy; }
        set { delayDestroy = value; }
    }

    #endregion

    // action that call delegate method when we destroy bullet
    public Action<GameObject> OnDestroyBullet;

    private void Start()
    {
        _bulletRig = GetComponent<Rigidbody>();
        StartBulletMove();
    }

    private void FixedUpdate()
    {
        MoveBullet();
    }

    public void StartBulletMove()
    {
        StartCoroutine(DestroyBulletAfterTime(delayDestroy));
    }

    
    public void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("OnCollisionEnter: "+name);
        BulletCollide(collision);
    }

    IEnumerator DestroyBulletAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        // after time, if bullet actiove then destroy
        if (gameObject.activeSelf)
        {
            BulletDestroy();
        }
    }

    // move bullet in one fixed frame
    public void MoveBullet()
    {
        _bulletRig.MovePosition(_bulletRig.position + _velocity * _speedMove * Time.fixedDeltaTime);
    }

    public virtual void BulletCollide(Collision collision)
    {
        BulletDestroy();

        // get collide object
        GameObject objectCollide = collision.collider.gameObject;
        // get component if exist
        Character character = objectCollide.GetComponent<Character>();
        // if collide character then call take damage
        if (character != null)
        {
            character.characterCombat.TakeDamage(_damage);
        }
    }

    public virtual void BulletDestroy()
    {
        OnDestroyBullet(gameObject);
    }



}
