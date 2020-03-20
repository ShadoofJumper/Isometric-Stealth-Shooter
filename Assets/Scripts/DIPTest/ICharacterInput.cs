using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterInput
{
    Vector3 PointToLook { get; }
    Vector3 Velocity { get; }

    void UpdateInput();
}
