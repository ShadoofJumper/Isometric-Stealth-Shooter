using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterInput
{
    bool IsShoot { get; }
    bool IsPressReload { get; }

    void UpdateInput();
}
