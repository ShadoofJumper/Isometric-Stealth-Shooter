using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover
{
    private ICharacterInput _input;
    private Transform _objectToMove;
    private Rigidbody _objectToMoveRig;
    private CharacterSettings _settings;

    public CharacterMover(ICharacterInput input, Transform objectToMove, CharacterSettings settings)
    {
        _input = input;
        _objectToMove = objectToMove;
        _settings = settings;
        _objectToMoveRig = objectToMove.GetComponent<Rigidbody>();
    }

    public void SetStartPosition()
    {
        if (_settings.IsAi)
        {
            Transform firstPoint = _settings.Path.transform.GetChild(0);
            _objectToMove.transform.position = firstPoint.position;
        }
    }

    // method for move object one frame
    // need call in fixed update
    public void Move()
    {
        // move object rigidbody on velocity from input
        // multyple on speed from setting
        _objectToMoveRig.MovePosition(_objectToMoveRig.position + _input.Velocity * _settings.Speed * Time.fixedDeltaTime);
    }

    // method for look in correect direction
    // call in updaye
    public void UpdateLook()
    {
        //object look to point we get from input, and correct it by object height
        _objectToMove.LookAt(_input.PointToLook + Vector3.up * _objectToMove.position.y);
    }
}
