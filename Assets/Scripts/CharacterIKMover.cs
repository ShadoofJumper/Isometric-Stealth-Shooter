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
    public bool useLook = false;
    ///  -----------------------------------
    // shoulder for parent of aip, so aim move with shoulder
    private Transform shoulder;
    // game objects
    private GameObject aimPivot;
    private GameObject leftHandPivot;
    private GameObject rightHandPivot;
    //test
    private Transform leftHandSpotObject;
    private GameObject rightHandSpotObject;
    //
    private Character _character;
    //private Vector3 charDirection;


    public bool UseLook { get { return useLook; } set { useLook = value; } }


    // Start is called before the first frame update
    void Start()
    {
        anim            = GetComponent<Animator>();
        shoulder        = anim.GetBoneTransform(HumanBodyBones.RightShoulder);
        _character      = GetComponentInParent<Character>();
        //charDirection   = _character.CharacterInput.LookDirection;

        CreateObjectsForIKMove();
    }

    private void CreateObjectsForIKMove()
    {
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
        rightHandSpotObject = new GameObject("rightHandSpot");
        rightHandSpotObject.transform.parent = shoulder;
    }

    public void UpdateWeaponHandSpots(Transform leftHandObject, SimpleTransform rightHand)
    {
        Debug.Log($"rightHand: pos: {rightHand.position}, rot: {rightHand.rotation}");
        rightHandSpotObject.transform.localPosition = rightHand.position;
        rightHandSpotObject.transform.localRotation = rightHand.rotation;
        leftHandSpotObject = leftHandObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (leftHandSpotObject == null)
            return;
        leftHandPivot.transform.position = leftHandSpotObject.position;
        leftHandPivot.transform.rotation = leftHandSpotObject.rotation;

        rightHandPivot.transform.position = rightHandSpotObject.transform.position;
        rightHandPivot.transform.rotation = rightHandSpotObject.transform.rotation;
    }



    void OnAnimatorIK(int layerIndex)
    {

        Vector3 lookPosition = _character.gameObject.transform.position + _character.gameObject.transform.forward * 5;
        lookPosition.y = 2.5f;
        //Debug.Log($"char pos: {_character.gameObject.transform.position}");
        //Debug.Log($"look pos: {lookPosition}");
        //Debug.Log($"look dir: {_character.CharacterInput.LookDirection}");

        //test
        Debug.DrawLine(_character.gameObject.transform.position, lookPosition, Color.blue);

        anim.SetLookAtWeight(powerIK, bodyWeight, headWeight, eyesWeight, lookClamp);
        anim.SetLookAtPosition(lookPosition);

        if (!useLook)
            return;
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