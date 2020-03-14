using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : ICharacterInput
{
    private float speed;
    private float velocity;

    public float Speed { get { return speed; } }
    public float Velocity { get { return velocity; } }

    // constuctor for input
    public AIInput()
    {
        Debug.Log("Create Bot Input0");
    }

    private void Awake()
    {
        Debug.Log("Create Bot Input1");
    }

    private void Start()
    {
        Debug.Log("Create Bot Input2");
    }

    public void UpdateInput()
    {
        //Debug.Log("Try update Bot inpur");
    }
}
