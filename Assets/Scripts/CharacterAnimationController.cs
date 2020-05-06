using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator           characterAnimator;
    [SerializeField] private CharacterIKMover   characterIKMover;
    private List<Collider> dollColliders = new List<Collider>();
    private Collider    characterSpine;
    private int         aimEnableCount;
    private float       currentAimWeight;
    private Coroutine increasAim;
    private Coroutine decreasAim;

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
    public void EnableAim(float enableTime = 0.0f, UnityAction delayFunk = null)
    {
        aimEnableCount += 1;
        if (aimEnableCount == 1)
        {
            if (enableTime == 0.0f)
            {
                characterAnimator.SetLayerWeight(2, 1.0f);
                characterIKMover.PowerLookIK = 1.0f;
                currentAimWeight = 1.0f;
                return;
            }
            if (increasAim != null) StopCoroutine(increasAim);
            if (decreasAim != null) StopCoroutine(decreasAim);
            increasAim = StartCoroutine(ChangeAimWeight(currentAimWeight, 1.0f, enableTime, delayFunk));
        }
        else if(aimEnableCount > 1)
        {
            delayFunk.Invoke();
        }
    }

    public void DisableAim(float disableTime = 0.0f, UnityAction delayFunk = null)
    {
        aimEnableCount -= 1;
        if(aimEnableCount <= 0)
        {
            if (disableTime == 0.0f)
            {
                characterAnimator.SetLayerWeight(2, 0.0f);
                characterIKMover.PowerLookIK = 0.0f;
                currentAimWeight = 0.0f;
                return;
            }
            if (increasAim != null) StopCoroutine(increasAim);
            if (decreasAim != null) StopCoroutine(decreasAim);
            decreasAim = StartCoroutine(ChangeAimWeight(currentAimWeight, 0.0f, disableTime, delayFunk));
            aimEnableCount = 0;
        }
    }

    IEnumerator ChangeAimWeight(float from, float to, float time, UnityAction delayFunk)
    {
        float t = 0f;
        while (t < 1.0f)
        {
            currentAimWeight = Mathf.Lerp(from, to, t);
            t += Time.deltaTime * 1/time;
            characterAnimator.SetLayerWeight(2, currentAimWeight);
            characterIKMover.PowerLookIK = currentAimWeight;
            yield return null;
        }
        if(delayFunk!=null)
            delayFunk.Invoke();
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
