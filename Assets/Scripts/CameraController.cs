using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSmoothens        = 0.125f;
    [SerializeField] private float moveLookSpeed        = 1.0f;
    [SerializeField] private float moveLookSpeedBuff    = 10.0f;
    [SerializeField] private float rangeLook            = 1.0f;

    // flag for switch global and local look of player
    private bool isGlobalLook = false;

    void Start()
    {
        // seet start camera position near player
        Vector3 playerPos = SceneController.instance.StartPositionPlayer.position;
        transform.position = new Vector3(playerPos.x, transform.position.y, playerPos.z);  
    }

    void FixedUpdate()
    {
        // set position
        transform.position = CalculateCameraPos();
        //check is gloval look
        isGlobalLook = Input.GetKey(KeyCode.Space);
    }

    private Vector3 CalculateCameraPos()
    {
        // speed for camera move to mouse
        float speedCameraMoveToMouse = !isGlobalLook ? moveLookSpeed : moveLookSpeed + moveLookSpeedBuff;// 
        // calculate mouse direction from target to mouse
        Vector3 mouseLookDirect     = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - target.position).normalized;
        Vector3 cameraMouseOffset   = new Vector3(mouseLookDirect.x, 0, mouseLookDirect.z);

        if (!isGlobalLook)
        {
            // set mouse move in range
            cameraMouseOffset.x = Mathf.Clamp(cameraMouseOffset.x, rangeLook * -1, rangeLook);
            cameraMouseOffset.z = Mathf.Clamp(cameraMouseOffset.z, rangeLook * -1, rangeLook);
        }
 

        // move camera to player position + mouse offset
        Vector3 destPosition = new Vector3(target.position.x, transform.position.y, target.position.z) + cameraMouseOffset * speedCameraMoveToMouse;
        Vector3 newCameraPos = Vector3.Lerp(transform.position, destPosition, moveSmoothens * Time.fixedDeltaTime);

        return newCameraPos;

    }
}
