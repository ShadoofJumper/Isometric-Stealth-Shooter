using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator characterAnimator;
    private List<Collider>  dollColliders = new List<Collider>();
    private Collider        characterSpine;
    [SerializeField] private CharacterIKMover characterIKMover;


    private int aimEnableCount;

    private void Awake()
    {
        SetRagdollParts();
    }

    public void UpdateMoveAnim(float currentSpeed, Vector3 movePos)
    {
        characterAnimator.SetFloat("Speed", currentSpeed);
        characterAnimator.SetFloat("PosX", movePos.x);
        characterAnimator.SetFloat("PosY", movePos.z);
    }

    public void PauseCharAnim()
    {
        characterAnimator.enabled = false;
    }

    public void ResumeCharAnim()
    {
        characterAnimator.enabled = true;
    }


    // ------- aim controll -----
    // aim work as counter, in two button hit aim then aim will close if two button disable aim
    public void EnableAim()
    {
        aimEnableCount += 1;
        if(aimEnableCount == 1)
        {
            characterAnimator.SetLayerWeight(2, 1.0f);
            characterIKMover.UseLook = true;
        }
    }

    public void DisableAim()
    {
        aimEnableCount -= 1;
        if(aimEnableCount <= 0)
        {
            characterAnimator.SetLayerWeight(2, 0.0f);
            aimEnableCount = 0;
            characterIKMover.UseLook = false;
        }
    }

    // -------------------------- [RAGDOLL LOGIC] --------------------------
    private void SetRagdollParts()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.isTrigger = true;
            dollColliders.Add(c);
            if (c.tag == "CharacterSpine")
            {
                characterSpine = c;
            }
        }
    }

    public void EnableRagdoll()
    {
        foreach (Collider c in dollColliders)
        {
            c.attachedRigidbody.velocity = Vector3.zero;
            c.isTrigger = false;
            c.gameObject.layer = default;
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


    public void HitRagdoll(DamageHit hitPoint, float power)
    {
        Vector3 dir = hitPoint.dir;
        Vector3 origin = hitPoint.point - dir;
        Vector3 impulseVelocity = dir * power;

        RaycastHit hit;
        //Debug.DrawLine(origin, origin + dir * 2, Color.green);
        if (Physics.Raycast(origin, dir, out hit, Mathf.Infinity)) {
            hit.rigidbody.AddForceAtPosition(impulseVelocity, hit.point, ForceMode.Impulse);
        } else {
            characterSpine.attachedRigidbody.AddForce(impulseVelocity, ForceMode.Impulse);
        }
    }
    // -------------------------- [RAGDOLL LOGIC END] --------------------------

}
