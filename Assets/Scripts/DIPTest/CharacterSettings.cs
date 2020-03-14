using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Character/Settings", fileName = "Character parametrs")]
public class CharacterSettings : ScriptableObject
{
    // parametrs form characters
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;

    [SerializeField] private bool isAi;
    //for ai settings // need hide in future dynamicly
    [SerializeField] private float pauseDelay;
    [SerializeField] private GameObject path;

    // get variables
    public float Speed { get { return speed; } }
    public float RotateSpeed { get { return rotateSpeed; } }
    public float PauseDelay { get { return pauseDelay; } }
    public bool IsAi { get { return isAi; } }
    public GameObject Path { get { return path; }}
}
