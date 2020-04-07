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
        // move object rigidbody on velocity from input
        // multyple on speed from setting
        _objectToMoveRig.MovePosition(_objectToMoveRig.position + _input.Velocity * currentSpeed * Time.fixedDeltaTime);

    }

    public void UpdateMoveAnim()
    {
        // devide currnt speed to max speed
        float objectSpeed = (_input.NotRawVelocity * currentSpeed / _speed.run).magnitude;
        // update animation
        _characterAnimator.SetFloat("Speed", objectSpeed);
    }

    // method for look in correect direction
    // call in updaye
    public void UpdateMover()
    {
        //object look to point we get from input, and correct it by object height
        _objectToMove.LookAt(_input.PointToLook + Vector3.up * _objectToMove.position.y);
    }


}
