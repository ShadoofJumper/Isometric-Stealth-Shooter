using UnityEngine;

public interface ICharacterMover
{
    void Move();
    void SetStartPosition(Vector3 startPos);
    void UpdateMover();
}
