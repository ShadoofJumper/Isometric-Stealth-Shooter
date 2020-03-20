using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : ICharacterInput
{
    private Vector3 pointToLook;
    private Vector3 velocity;

    public Vector3 PointToLook { get { return pointToLook; } }
    public Vector3 Velocity { get { return velocity; } }

    private Camera mainCamera;

    // constuctor for input
    public PlayerInput()
    {
        mainCamera = Camera.main;
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
        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.transform.position.y));
        pointToLook = mousePos;
    }

}
