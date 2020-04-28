using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIKMover : MonoBehaviour
{
    private Animator anim;

    ///  ---------------- For loot at target
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

    //  use look on point
    private bool useLook = false;
    ///  -----------------------------------
    // shoulder for parent of aip, so aim move with shoulder
    private Transform shoulder;
    //for place where will be left hand
    private SimpleTransform leftHandSpot;
    private SimpleTransform rightHandSpot;
    // game objects
    private GameObject aimPivot;
    private GameObject leftHandPivot;
    private GameObject rightHandPivot;

    public bool UseLook { get { return useLook; } set { useLook = value; } }


    // Start is called before the first frame update
    void Start()
    {
        anim        = GetComponent<Animator>();
        shoulder    = anim.GetBoneTransform(HumanBodyBones.RightShoulder);
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

        // start empty transforms
        leftHandSpot    = new SimpleTransform();
        rightHandSpot   = new SimpleTransform();

        Debug.Log($"StartWeaponHandSpots: pos1: {leftHandSpot.position}, pos2: {rightHandSpot.position}");
    }

    public void UpdateWeaponHandSpots(SimpleTransform leftHand, SimpleTransform rightHand)
    {
        Debug.Log($"UpdateWeaponHandSpots: pos1: {leftHand.position}, pos2: {rightHand.position}");
        leftHandSpot    = leftHand;
        rightHandSpot   = rightHand;
    }

    // Update is called once per frame
    void Update()
    {
        leftHandPivot.transform.position = leftHandSpot.position;
        leftHandPivot.transform.rotation = leftHandSpot.rotation;

        rightHandPivot.transform.position = rightHandSpot.position;
        rightHandPivot.transform.rotation = rightHandSpot.rotation;
    }



    void OnAnimatorIK(int layerIndex)
    {
        if (!useLook)
            return;
        //anim.SetLookAtWeight(powerIK, bodyWeight, headWeight, eyesWeight, lookClamp);
       // anim.SetLookAtPosition(target.position);

        // move left hand
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPivot.transform.position);

        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPivot.transform.rotation);
        // move right hand
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandPivot.transform.position);

        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandPivot.transform.rotation);
    }

}

public struct SimpleTransform
{
    public Vector3 position;
    public Quaternion rotation;

    public SimpleTransform(Vector3 _position, Quaternion _rotation)
    {
        position = _position;
        rotation = _rotation;
    }
}