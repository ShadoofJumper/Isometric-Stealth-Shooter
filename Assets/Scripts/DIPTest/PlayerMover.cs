using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : ICharacterMover
{
    private PlayerInput _input;
    private Transform   _objectToMove;
    private Rigidbody   _objectToMoveRig;
    private Animator    _characterAnimator;
    private CharacterSettings   _settings;
    private SpeedParams         _speed;
    private int speedId = 1;
    private float currentSpeed;

    // properties
    public float    CurrentSpeed   { get { return currentSpeed; }  set { currentSpeed  = value; } }
    public int      SpeedId        { get { return speedId; }       set { speedId       = value; } }


    public PlayerMover(ICharacterInput input, Transform objectToMove, CharacterSettings settings)
    {
        _input              = input as PlayerInput;
        _objectToMove       = objectToMove;
        _settings           = settings;
        _objectToMoveRig    = objectToMove.GetComponent<Rigidbody>();
        _characterAnimator  = objectToMove.GetComponentInChildren<Animator>();
        _speed              = new SpeedParams(settings.Speed);
        currentSpeed        = _speed.walk;
    }

    // can be method for spawn place
    public void SetStartPosition(Vector3 startPos)
    {
        // position
        _objectToMove.transform.position = startPos;
    }

    // method for move object one frame
    // need call in fixed update
    public void Move()
    {
        // TO DO change on key down, can be steals or run
        if (Input.GetKey(KeyCode.LeftControl)) {
            currentSpeed = _speed.steals;
            speedId = 0;
        }
        else if (Input.GetKey(KeyCode.LeftShift)) {
            currentSpeed = _speed.run;
            speedId = 2;
        }
        else {
            currentSpeed = _speed.walk;
            speedId = 1;
        }

        // normilize velocity
        Vector3 moveSpeed = _input.Velocity.normalized * currentSpeed;
        // move object rigidbody on velocity from input
        // multyple on speed from setting

        _objectToMoveRig.MovePosition(_objectToMoveRig.position + moveSpeed * Time.fixedDeltaTime);
    }

    public void UpdateMoveAnim()
    {
        Vector3 pos = Vector3.ClampMagnitude(_input.NotRawVelocity, 1) * currentSpeed / _speed.run;
        // local direction
        Vector3 localPos = _objectToMove.InverseTransformDirection(pos);

        Debug.Log($"notRawVelocity: {_input.NotRawVelocity}, change: {Vector3.ClampMagnitude(_input.NotRawVelocity, 1)} local {localPos}");//, localPos {}

        // devide currnt speed to max speed
        float objectSpeed = pos.magnitude;
        Debug.Log($"mag: {localPos.magnitude}");
        // update animation
        _characterAnimator.SetFloat("Speed", objectSpeed);
        _characterAnimator.SetFloat("PosX", localPos.x);
        _characterAnimator.SetFloat("PosY", localPos.z);

    }

    // method for look in correect direction
    // call in updaye
    public void UpdateMover()
    {
        //get point to look and calculate object rotation
        Vector3 lookDir = _input.PointToLook - _objectToMove.transform.position;
        lookDir.y = 0;
        Debug.DrawLine(_objectToMove.transform.position, _input.PointToLook, Color.red);
        Quaternion newRotateion = Quaternion.LookRotation(lookDir.normalized);
        _objectToMoveRig.MoveRotation(newRotateion);
    }

}
