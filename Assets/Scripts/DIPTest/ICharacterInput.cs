using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterInput
{
    MouseInput[] MouseInput { get; }
    Vector3 LookDirection { get; }
    Vector3 PointToLook { get; }
    bool IsPressReload { get; }

    void UpdateInput();
}
