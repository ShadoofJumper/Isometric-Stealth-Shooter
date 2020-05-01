using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : ICharacterMover
{
    private PlayerInput _input;
    private Transform   _objectToMove;
    private Rigidbody   _objectToMoveRig;
    private CharacterAnimationController _charAnim;
    private CharacterSettings   _settings;
    private SpeedParams         _speed;
    private int speedId = 1;
    private float currentSpeed;
    GameObject _test;
    // properties
    public float    CurrentSpeed   { get { return currentSpeed; }  set { currentSpeed  = value; } }
    public int      SpeedId        { get { return speedId; }       set { speedId       = value; } }


    public PlayerMover(ICharacterInput input, Transform objectToMove, CharacterSettings settings, GameObject test)
    {
        _input              = input as PlayerInput;
        _objectToMove       = objectToMove;
        _settings           = settings;
        _objectToMoveRig    = objectToMove.GetComponent<Rigidbody>();
        _charAnim           = objectToMove.GetComponentInChildren<CharacterAnimationController>();
        _speed              = new SpeedParams(settings.Speed);
        currentSpeed        = _speed.walk;

        _test = test;
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
        Vector3 pos         = Vector3.ClampMagnitude(_input.NotRawVelocity, 1) * (speedId + 1) / 3;
        Vector3 localPos    = _objectToMove.InverseTransformDirection(pos);
        float objectSpeed   = pos.magnitude;
        _charAnim.UpdateMoveAnim(objectSpeed, localPos);
    }

    // method for look in correect direction
    // call in updaye
    public void UpdateMover()
    {
        Debug.DrawLine(_objectToMove.transform.position, _input.PointToLook, Color.red);
        _test.transform.position = _input.PointToLook;

        Quaternion newRotateion = Quaternion.LookRotation(_input.LookDirection);
        _objectToMoveRig.MoveRotation(newRotateion);
    }

}
