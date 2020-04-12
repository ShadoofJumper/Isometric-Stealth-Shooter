using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterInput
{
    MouseInput[] MouseInput { get; }
    bool IsPressReload { get; }

    void UpdateInput();
}
