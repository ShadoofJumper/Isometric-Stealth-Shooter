using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSmoothens = 0.125f;
    [SerializeField] private float moveLookSpeed = 1.0f;
    [SerializeField] private float moveLookSpeedBuff = 10.0f;
    [SerializeField] private float rangeLook = 1.0f;
    // flag for translate camera
    private bool isCameraControlled = false;
    // TEST
    [SerializeField] private AnimationCurve curveWithFade;
    [SerializeField] private AnimationCurve curveWithSpeedUp;

    // flag for switch global and local look of player
    private bool isGlobalLook = false;

    private void Awake()
    {
        // seet start camera position near player
        Vector3 playerPos = SceneController.instance.StartPositionPlayer.position;
        transform.position = new Vector3(playerPos.x, transform.position.y, playerPos.z);
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        // return if player on pause
        if (SceneController.instance.player.isOnPause || isCameraControlled)
            return;

        // set position
        transform.position = CalculateCameraPos();
        ////check is gloval look
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

    public void ShowPoint(Transform lookPoint, float time, bool withSlowDown = false, float startDelay = 0, float delay = 0, UnityAction delayFunk = null)
    {
        StartCoroutine(MoveToPointAndBack(lookPoint, time, withSlowDown, startDelay, delay, delayFunk));
    }

    IEnumerator MoveToPointAndBack(Transform lookPoint, float time, bool withSlowDown, float startDelay, float delay, UnityAction delayFunk)
    {
        // defoult start at target pos
        Vector3 startPoint = transform.position;
        Vector3 finalPoint = new Vector3(lookPoint.position.x, transform.position.y, lookPoint.position.z);
        Debug.Log("startPoint: " + transform.position);
        isCameraControlled = true;
        yield return new WaitForSeconds(startDelay);
        // move to point
        yield return MoveToPoint(transform.position, finalPoint, time, withSlowDown);
        yield return new WaitForSeconds(delay);
        // do stuff
        if (delayFunk!=null) { delayFunk.Invoke(); };
        //move back
        yield return MoveToPoint(transform.position, startPoint, time, withSlowDown);
        isCameraControlled = false;
    }

    IEnumerator MoveToPoint(Vector3 a, Vector3 b, float time, bool withSlowDown = false)
    {

        float step      = (1.0f / time);
        float i         = 0;
        float iCurve    = 0;

        while (i <= 1.0f)
        {
            i += Time.deltaTime * step;
            float moveStep = withSlowDown ? iCurve = curveWithFade.Evaluate(i) : i;
            transform.position = Vector3.Lerp(a, b, moveStep);
            yield return null;
        }
       
    }

}
