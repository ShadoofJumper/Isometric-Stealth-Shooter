using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBones : MonoBehaviour
{
    private Animator anim;


    /// <summary>
    ///  ---------------- for loot at target
    /// </summary>
    [Range(0, 1)]
    public float bodyWeight;
    [Range(0, 1)]
    public float headWeight;
    [Range(0, 1)]
    public float eyesWeight;
    [Range(0, 1)]
    public float lookClamp;
    [Range(0, 1)]
    public float powerIK = 1.0f;

    public bool useLook = false;
    public Transform target;
    ///  -----------------------------------
    ///  
    // shoulder for parent of aip, so aim move with shoulder
    private Transform shoulder;
    //for place where will be left hand
    public Transform leftHandSpot;
    // for place where will be right hand
    public Vector3 rotationRightHand;
    public Vector3 positionRightHand;
    // game objects
    private GameObject aimPivot;
    private GameObject leftHandPivot;
    private GameObject rightHandPivot;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        shoulder = anim.GetBoneTransform(HumanBodyBones.RightShoulder);
        // create empty objects
        aimPivot = new GameObject("aimPivot");
        aimPivot.transform.parent = shoulder;
        aimPivot.transform.localPosition = Vector3.zero;
        aimPivot.transform.localRotation = new Quaternion();

        leftHandPivot = new GameObject("leftHandPivot");
        leftHandPivot.transform.parent = aimPivot.transform;
        leftHandPivot.transform.localPosition = Vector3.zero;

        rightHandPivot = new GameObject("rightHandPivot");
        rightHandPivot.transform.parent = aimPivot.transform;
        rightHandPivot.transform.localPosition = Vector3.zero;

        Quaternion rightRot = Quaternion.Euler(rotationRightHand.x, rotationRightHand.y, rotationRightHand.z);
        rightHandPivot.transform.localRotation = rightRot;
        rightHandPivot.transform.localPosition = positionRightHand;
    }

    // Update is called once per frame
    void Update()
    {
        RotateToTarget();
        // set leftHandPivot ro target pivot position
        leftHandPivot.transform.position = leftHandSpot.position;
        leftHandPivot.transform.rotation = leftHandSpot.rotation;

        if (Input.GetMouseButtonDown(1))
        {
            useLook = true;
            anim.SetLayerWeight(2, 1);
        }

        if (Input.GetMouseButtonUp(1))
        {
            useLook = false;
            anim.SetLayerWeight(2, 0);
        }
    }

    private void RotateToTarget()
    {

    }

    void OnAnimatorIK(int layerIndex)
    {
        if (!useLook)
            return;
        anim.SetLookAtWeight(powerIK, bodyWeight, headWeight, eyesWeight, lookClamp);
        anim.SetLookAtPosition(target.position);

        // move hand left
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPivot.transform.position);

        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPivot.transform.rotation);
        // move hand right
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandPivot.transform.position);

        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandPivot.transform.rotation);
    }

}
