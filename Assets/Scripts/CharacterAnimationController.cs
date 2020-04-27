using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _characterAnimator;
    private List<Collider> dollColliders = new List<Collider>();


    private void Awake()
    {
        SetRagdollParts();
    }


    private void SetRagdollParts()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            //c.enabled = false;
            c.isTrigger = true;
            dollColliders.Add(c);
        }
    }

    public void EnableRagdoll()
    {
        //_characterAnimator.avatar = null;

        foreach (Collider c in dollColliders)
        {
            //c.enabled = true;
            c.attachedRigidbody.velocity = Vector3.zero;
            c.isTrigger = false;
        }
    }

    public void DisableRagdoll()
    {
        foreach (Collider c in dollColliders)
        {
            //c.enabled = false;
            c.isTrigger = true;
        }
    }


    public Collider CheckWhereHitRagdoll(Vector3 hitPoint)
    {
        foreach (Collider c in dollColliders)
        {

            if (c.bounds.Contains(hitPoint))
            {
                Debug.Log($"AAAAAAAAAAAAA: {c.name}");
                return c;
            }

        }
        return null;
    }

    public void UpdateMoveAnim(float currentSpeed, Vector3 movePos)
    {
        _characterAnimator.SetFloat("Speed", currentSpeed);
        _characterAnimator.SetFloat("PosX", movePos.x);
        _characterAnimator.SetFloat("PosY", movePos.z);
    }

    public void PauseCharAnim()
    {
        _characterAnimator.enabled = false;
    }

    public void ResumeCharAnim()
    {
        _characterAnimator.enabled = true;
    }


}
