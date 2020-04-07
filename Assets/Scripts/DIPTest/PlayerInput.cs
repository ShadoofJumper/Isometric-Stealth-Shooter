using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : ICharacterInput
{
    private Vector3 pointToLook;
    private Vector3 velocity;
    private Vector3 notRawVelocity;
    private bool isShoot = false;
    private bool isPressReload = false;

    public Vector3 PointToLook  { get { return pointToLook; } }
    public Vector3 Velocity     { get { return velocity; } }
    public Vector3 NotRawVelocity { get { return notRawVelocity; } }

    public bool IsShoot { get { return isShoot; } }

    public bool IsPressReload => isPressReload;

    private Camera mainCamera;

    // constuctor for input
    public PlayerInput()
    {
        mainCamera = Camera.main;
    }

    public void UpdateInput()
    {
        if (GameManager.IsGameOnPause)
            return;

        velocity        = new Vector3(Input.GetAxisRaw("Horizontal"),   0, Input.GetAxisRaw("Vertical")).normalized;
        notRawVelocity  = new Vector3(Input.GetAxis("Horizontal"),      0, Input.GetAxis("Vertical"));

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.transform.position.y));
        pointToLook = mousePos;

        //check if over gui then do not calculate input
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        isShoot = false;
        if (Input.GetMouseButtonDown(0))
        {
            isShoot = true;
        }

        // TO DO change to press in time
        isPressReload = false;
        if (Input.GetKeyDown(KeyCode.R))
        {
            isPressReload = true;
        }
    }

}
