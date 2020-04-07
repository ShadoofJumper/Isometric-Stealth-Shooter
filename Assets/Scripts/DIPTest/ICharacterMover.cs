using UnityEngine;

public interface ICharacterMover
{

    float   CurrentSpeed    { get; set; }
    int     SpeedId         { get; set; }

    void Move();
    void SetStartPosition(Vector3 startPos);
    void UpdateMover();
    void UpdateMoveAnim();
}
