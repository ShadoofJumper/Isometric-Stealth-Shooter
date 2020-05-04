using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : ICharacterInput
{
    private Vector3         pointToLook;
    private Vector3         velocity;
    private Vector3         notRawVelocity;
    private MouseInput[]    mouseInput = new MouseInput[] {new MouseInput(), new MouseInput()};
    private bool            isPressReload = false;
    private float           playerEyesLevel = 2.5f;
    // for correct isometrical rotation
    private Vector3 forward;
    private Vector3 right;
    private float   velosityIsomFix;
    private Plane   gamePlane;
    private Transform _objectToMove;
    public Vector3 PointToLook      { get { return pointToLook; } }
    public Vector3 LookDirection    {
        get {
            Vector3 temp = Vector3.Normalize(pointToLook - _objectToMove.position);
            temp.y = 0.0f;
            return temp;
        }
    }
    public Vector3 Velocity         { get { return velocity; } }
    public Vector3 NotRawVelocity   { get { return notRawVelocity; } }
    public MouseInput[] MouseInput  { get { return mouseInput; } }
    public bool IsPressReload => isPressReload;

    private Camera mainCamera;

    // constuctor for input
    public PlayerInput(Transform objectToMove)
    {
        _objectToMove = objectToMove;
        mainCamera = Camera.main;
        forward         = Camera.main.transform.forward;
        forward.y       = 0;
        right           = Quaternion.Euler(0, 90, 0) * forward;
        // for velocity bug fix
        velosityIsomFix = 2.0f;

        // create plane for raycast ray on it and get player look point
        // set level of plane on player eye level
        gamePlane = new Plane(Vector3.up, new Vector3(0, playerEyesLevel, 0));
    }

    private void UpdateMouseInput(int mouseId)
    {
        mouseInput[mouseId] = new MouseInput(Input.GetMouseButtonDown(mouseId), Input.GetMouseButton(mouseId), Input.GetMouseButtonUp(mouseId));
    }

    public void UpdateInput()
    {
        if (GameManager.IsGameOnPause)
            return;

        // TODO not need to velocity can be one and another normolized

        Vector3 vericalMovementNoRaw    = forward * Input.GetAxis("Vertical");
        Vector3 horizontalMovementNoRaw = right * Input.GetAxis("Horizontal");

        notRawVelocity = vericalMovementNoRaw + horizontalMovementNoRaw;
        notRawVelocity *= velosityIsomFix;

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
        //pointToLook = ClampPointToLook(pointToLook, 8, 15);
        //pointToLook.y = playerEyesLevel;
    }

    private Vector3 ClampPointToLook(Vector3 point, float minRange, float maxRange)
    {
        Vector3 offsetFromCenter = pointToLook - _objectToMove.transform.position;
        offsetFromCenter.y = 0;

        double om = offsetFromCenter.sqrMagnitude;
        if (om > (double)maxRange * (double)maxRange)
        {
            return _objectToMove.transform.position + offsetFromCenter.normalized * maxRange;
        }
        else if (om < (double)minRange * (double)minRange)
        {
            return _objectToMove.transform.position + offsetFromCenter.normalized * minRange;
        }


        return point;
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
