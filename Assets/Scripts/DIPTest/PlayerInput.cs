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
    // for correct isometrical rotation
    private Vector3 forward;
    private Vector3 right;
    private float   velosityIsomFix;
    private Plane   gamePlane;

    public Vector3 PointToLook      { get { return pointToLook; } }
    public Vector3 Velocity         { get { return velocity; } }
    public Vector3 NotRawVelocity   { get { return notRawVelocity; } }
    public MouseInput[] MouseInput  { get { return mouseInput; } }
    public bool IsPressReload => isPressReload;

    private Camera mainCamera;

    // constuctor for input
    public PlayerInput()
    {
        mainCamera = Camera.main;

        forward = Camera.main.transform.forward;
        forward.y = 0;
        right = Quaternion.Euler(0, 90, 0) * forward;
        // for velocity bug fix
        velosityIsomFix = 2.0f;

        // create plane for raycast ray on it and get player look point
        // set level of plane on player eye level
        Plane gamePlane = new Plane(Vector3.up, new Vector3(0, 2.5f, 0));
    }

    private void UpdateMouseInput(int mouseId)
    {
        mouseInput[mouseId] = new MouseInput(Input.GetMouseButtonDown(mouseId), Input.GetMouseButton(mouseId), Input.GetMouseButtonUp(mouseId));
    }

    public void UpdateInput()
    {
        if (GameManager.IsGameOnPause)
            return;

        notRawVelocity  = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        Vector3 verticalMovement    = forward   * Input.GetAxisRaw("Vertical");
        Vector3 horizontalMovement  = right     * Input.GetAxisRaw("Horizontal");

        velocity = verticalMovement + horizontalMovement;
        velocity *= velosityIsomFix;

        UpdatePointToLook();

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

    private void UpdatePointToLook()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // distance from start in ray where ray hit plane
        float enter = 0.0f;
        if (gamePlane.Raycast(ray, out enter))
        {
            pointToLook = ray.GetPoint(enter);
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
        down    = _down;
        press   = _press;
        up      = _up;
    }

}
