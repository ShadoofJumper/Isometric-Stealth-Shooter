using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : ICharacterInput
{
    private Vector3 pointToLook;
    private Vector3 velocity;
    private Vector3 notRawVelocity;
    private MouseInput[] mouseInput = new MouseInput[] {new MouseInput(), new MouseInput()};
    private bool isPressReload = false;

    public Vector3 PointToLook  { get { return pointToLook; } }
    public Vector3 Velocity     { get { return velocity; } }
    public Vector3 NotRawVelocity { get { return notRawVelocity; } }

    public MouseInput[] MouseInput { get { return mouseInput; } }

    public bool IsPressReload => isPressReload;

    private Camera mainCamera;

    // constuctor for input
    public PlayerInput()
    {
        mainCamera = Camera.main;
    }

    private void UpdateMouseInput(int mouseId)
    {
        mouseInput[mouseId] = new MouseInput(Input.GetMouseButtonDown(mouseId), Input.GetMouseButton(mouseId), Input.GetMouseButtonUp(mouseId));
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

        // set left button
        UpdateMouseInput(0);
        // set right button
        UpdateMouseInput(1);

        // TO DO change to press in time
        isPressReload = false;
        if (Input.GetKeyDown(KeyCode.R))
        {
            isPressReload = true;
        }
    }


}

//struct to save info about edges in our field of view
public struct MouseInput
{
    public bool down;
    public bool press;
    public bool up;

    public MouseInput(bool _down, bool _press, bool _up)
    {
        down = _down;
        press = _press;
        up = _up;
    }

}
