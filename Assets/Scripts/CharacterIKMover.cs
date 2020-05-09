using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIKMover : MonoBehaviour
{
    private Animator anim;

    ///  ---------------- For loot at target
    [Range(0, 1)]
    [SerializeField] private float bodyWeight;
    [Range(0, 1)]
    [SerializeField] private float headWeight;
    [Range(0, 1)]
    [SerializeField] private float eyesWeight;
    [Range(0, 1)]
    [SerializeField] private float lookClamp;
    [Range(0, 1)]
    [SerializeField] private float powerLookIK = 0.0f;
    //  use look on point
    public bool useLook = false;
    private Character _character;

    public Vector3 lookOffset;


    public bool UseLook         { get { return useLook; }       set { useLook = value; } }
    public float PowerLookIK    { get { return powerLookIK; }   set { powerLookIK = value; } }


    // Start is called before the first frame update
    void Start()
    {
        anim        = GetComponent<Animator>();
        _character  = GetComponentInParent<Character>();
    }


    void OnAnimatorIK(int layerIndex)
    {
        if (!useLook)
            return;

        Vector3 forwardLook     = _character.gameObject.transform.forward;
        forwardLook             = Quaternion.Euler(lookOffset) * forwardLook;
        Vector3 lookPosition    = _character.gameObject.transform.position + forwardLook * 5;
        lookPosition.y = 2.5f;

        //test
        //Debug.DrawLine(_character.gameObject.transform.position, lookPosition, Color.blue);

        anim.SetLookAtWeight(powerLookIK, bodyWeight, headWeight, eyesWeight, lookClamp);
        anim.SetLookAtPosition(lookPosition);
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