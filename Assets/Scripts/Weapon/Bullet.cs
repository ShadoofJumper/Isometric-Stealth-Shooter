using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet: MonoBehaviour
{
    private float _speedMove;
    private Vector3 _velocity;
    private Rigidbody _bulletRig;
    private float delayDestroy = 3.0f;

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

    public float DelayDestroy
    {
        get { return delayDestroy; }
        set { delayDestroy = value; }
    }

    #endregion

    // action that call delegate method when we estroy bullet
    public Action<Bullet> OnDestroyBullet;

    private void Start()
    {
        _bulletRig = GetComponent<Rigidbody>();
        StartCoroutine(DestroyBulletAfterTime(delayDestroy));
    }

    private void FixedUpdate()
    {
        MoveBullet();
    }

    
    public void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("OnCollisionEnter: "+name);
        BulletCollide(collision);
    }

    IEnumerator DestroyBulletAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        BulletDestroy();
    }

    // move bullet in one fixed frame
    public void MoveBullet()
    {
        _bulletRig.MovePosition(_bulletRig.position + _velocity * _speedMove * Time.fixedDeltaTime);
    }



    public virtual void BulletCollide(Collision collision)
    {
        Debug.Log("Collision : " + collision.contacts[0].point);
        BulletDestroy();

    }

    public virtual void BulletDestroy()
    {
        OnDestroyBullet(this);
        //Destrot bullet
        Destroy(gameObject);
    }



}
