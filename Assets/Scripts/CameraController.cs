using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform cameraMoveParent;
    [SerializeField] private Transform cameraRotateParent;
    [SerializeField] private float moveSmoothens = 5.0f;
    //[SerializeField] private float moveLookSpeed = 5.0f;
    //[SerializeField] private float moveLookSpeedBuff = 10.0f;
    [SerializeField] private float rotateSmoothens  = 5.0f;
    [SerializeField] private float rangeLook        = 1.0f;
    private Vector3 startOffsetCamera;
    // flag for translate camera
    private bool isCameraControlled = false;
    // TEST
    [SerializeField] private AnimationCurve curveWithFade;
    [SerializeField] private AnimationCurve curveWithSpeedUp;

    // flag for switch global and local look of player
    private bool isGlobalLook = false;
    // for get player look pint
    private PlayerInput playerInput;

    private void Awake()
    {
        // seet start camera position near player
        Vector3 playerPos = SceneController.instance.StartPositionPlayer.position;

        Vector3 cameraOffset = new Vector3();
        cameraOffset.x = cameraMoveParent.position.y / (2 * Mathf.Sin(Mathf.Deg2Rad * 45)) - 0.7f;
        cameraOffset.z = cameraMoveParent.position.y / (2 * Mathf.Sin(Mathf.Deg2Rad * 45)) * -1 + 0.7f;
        startOffsetCamera = cameraOffset;
        //set start camera position
        cameraMoveParent.position = new Vector3(startOffsetCamera.x, cameraMoveParent.position.y, startOffsetCamera.z);


    }

    void Start()
    {
        playerInput = SceneController.instance.player.CharacterInput as PlayerInput;

    }

    void FixedUpdate()
    {
        // return if player on pause
        if (SceneController.instance.player.isOnPause || isCameraControlled)
            return;
        ////check is gloval look
        isGlobalLook = Input.GetKey(KeyCode.Space);

        // set camera position
        MoveCamera();
        // set camera rotaton around target
        RotateCamera();
    }

    private void RotateCamera()
    {
        // rotate pranet
        Quaternion newRotation = Quaternion.Euler(0, 0, (target.eulerAngles.y + 45) * -1);//
        Quaternion newCameraAngle = Quaternion.Lerp(cameraRotateParent.localRotation, newRotation, rotateSmoothens * Time.fixedDeltaTime);

        cameraRotateParent.localRotation = newCameraAngle;
        transform.localRotation = Quaternion.Inverse(newCameraAngle);
    }

    private void MoveCamera()
    {
        // look direction
        Vector3 targetLookDirection = (playerInput.PointToLook - target.position);
        Vector3 pointLookFix = playerInput.PointToLook;
        pointLookFix.y = 0.5f;
        //Debug.DrawLine(target.position, pointLookFix, Color.red);
        Vector3 cameraMouseOffset = Vector3.zero;

        //if global look
        if (isGlobalLook)
        {
            cameraMouseOffset = new Vector3(0, rangeLook, 0);
        }

        // set camera offset
        transform.localPosition = Vector3.Lerp(transform.localPosition, cameraMouseOffset, moveSmoothens * Time.fixedDeltaTime); ;


        //// move camera to player position + mouse offset for global look
        Vector3 destPosition = new Vector3(
            target.position.x,
            cameraMoveParent.position.y,
            target.position.z) + startOffsetCamera;

        //Debug.Log($"Offset to: {startOffsetCamera}");
        Vector3 newCameraPos = Vector3.Lerp(cameraMoveParent.position, destPosition, moveSmoothens * Time.fixedDeltaTime);
        cameraMoveParent.position = newCameraPos;
    }

    public void ShowPoint(Transform lookPoint, float time, bool withSlowDown = false, float startDelay = 0, float delayInPlace = 0, UnityAction delayFunk = null)
    {
        StartCoroutine(MoveToPointAndBack(lookPoint, time, withSlowDown, startDelay, delayInPlace, delayFunk));
    }

    IEnumerator MoveToPointAndBack(Transform lookPoint, float time, bool withSlowDown, float startDelay, float delayInPlace, UnityAction delayFunk)
    {
        // defoult start at target pos
        Vector3 startPoint = cameraMoveParent.position;
        Vector3 finalPoint = new Vector3(lookPoint.position.x, cameraMoveParent.position.y, lookPoint.position.z);
        isCameraControlled = true;
        yield return new WaitForSeconds(startDelay);
        // move to point
        yield return MoveToPoint(cameraMoveParent.position, finalPoint, time, withSlowDown);
        yield return new WaitForSeconds(delayInPlace);
        // do stuff
        if (delayFunk!=null) { delayFunk.Invoke(); };
        //move back
        yield return MoveToPoint(cameraMoveParent.position, startPoint, time, withSlowDown);
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
            cameraMoveParent.position = Vector3.Lerp(a, b, moveStep);
            yield return null;
        }
       
    }

}
