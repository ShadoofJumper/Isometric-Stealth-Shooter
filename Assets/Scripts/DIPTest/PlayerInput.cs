using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : ICharacterInput
{
    private float speed;
    private float velocity;

    public float Speed { get { return speed; } }
    public float Velocity { get { return velocity; } }

    // constuctor for input
    public PlayerInput()
    {

    }

    private void Awake()
    {
        Debug.Log("Create Player Input1");
    }

    private void Start()
    {
        Debug.Log("Create Player Input2");
    }

    public void UpdateInput()
    {
        Debug.Log("Try update Player inpur");
    }

}
