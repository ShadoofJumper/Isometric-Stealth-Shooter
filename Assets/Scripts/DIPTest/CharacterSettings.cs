using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Character/Settings", fileName = "Character parametrs")]
public class CharacterSettings : ScriptableObject
{
    // parametrs form characters
    // speed of character, steals, walk and run speed
    [SerializeField] private Vector3 speed;
    [SerializeField] private float health;

    [SerializeField] private bool isAi;
    //for ai settings // need hide in future dynamicly
    [SerializeField] private float pauseDelay;
    [SerializeField] private GameObject pathObject;
    [SerializeField] private float rotateSpeed;

    // get variables
    public Vector3 Speed { get { return speed; } }
    public float Health { get { return health; } }

    public bool IsAi { get { return isAi; } }
    public GameObject Path { get { return pathObject; }}
    public float RotateSpeed { get { return rotateSpeed; } }
    public float PauseDelay { get { return pauseDelay; } }
}
