using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterInput
{
    float Speed { get; }
    float Velocity { get; }

    void UpdateInput();
}
