using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : ICharacterInput
{

    private Transform _path;
    private float _speed;
    private float _pauseDelay;
    private float _rotateSpeed;
    private Vector3[] _wayPoints;
    private Transform _objectToMove;
    private MonoBehaviour _myMonoBehaviour;

    private MouseInput[] mouseInput = new MouseInput[] { new MouseInput(), new MouseInput() };
    private Vector3 pointToLook;
    private Vector3 velocity;
    private Vector3 notRawVelocity;
    private bool isPressReload = false;

    public Vector3 PointToLook      { get { return pointToLook; } }
    public Vector3 Velocity         { get { return velocity; } }
    public Vector3 NotRawVelocity   { get { return notRawVelocity; } }

    public MouseInput[] MouseInput  => mouseInput;
    public bool IsPressReload       => isPressReload;
    public Vector3 LookDirection    => throw new System.NotImplementedException();

    // constuctor for input
    public AIInput(CharacterSettings settings, Transform objectToMove, MonoBehaviour myMonoBehaviour)
    {
        // get start parametrs for calculate move outpoot
        _path = settings.Path.transform;
        // get walk speed as defauls
        _speed = settings.Speed[1];
        _pauseDelay = settings.PauseDelay;
        _rotateSpeed = settings.RotateSpeed;
        _objectToMove = objectToMove;
        // for abylity start coroutin from this object
        _myMonoBehaviour = myMonoBehaviour;

        GetWayPointsFromPath(_path);

        // start calculate output
        _myMonoBehaviour.StartCoroutine(MoveBotToPoint(_pauseDelay));
    }

    // constuctor for input for flexible initization
    public AIInput(Transform path, float speed, float pauseDelay, float rotateSpeed, Transform objectToMove, MonoBehaviour myMonoBehaviour)
    {
        // get start parametrs for calculate move outpoot
        _path = path;
        _speed = speed;
        _pauseDelay = pauseDelay;
        _rotateSpeed = rotateSpeed;
        _objectToMove = objectToMove;
        // for abylity start coroutin from this object
        _myMonoBehaviour = myMonoBehaviour;

        GetWayPointsFromPath(path);

        // start calculate output
        _myMonoBehaviour.StartCoroutine(MoveBotToPoint(_pauseDelay));
    }

    private void GetWayPointsFromPath(Transform path)
    {
        _wayPoints = new Vector3[path.childCount];

        for (int i = 0; i < path.childCount; i++)
        {
            _wayPoints[i] = path.GetChild(i).position;
            _wayPoints[i] = new Vector3(_wayPoints[i].x, _objectToMove.position.y, _wayPoints[i].z);
        }
    }


    IEnumerator MoveBotToPoint(float pauseDelay)
    {
        // set start position
        //transform.position = wayPoints[0];

        // calculate speed for one step
        float moveStep = _speed * Time.deltaTime;

        // set first point to move
        int nextPositionId = 1;
        Vector3 nextPosition = _wayPoints[nextPositionId];
        // set start point to look
        pointToLook = new Vector3(nextPosition.x, 0, nextPosition.z);
        //transform.LookAt(nextPosition);

        while (true)
        {
            Vector3 dir = (nextPosition - _objectToMove.position).normalized;
            velocity = new Vector3(dir.x, 0, dir.z);

            if (Vector3.Distance(_objectToMove.position, nextPosition) < 0.1)
            {
                //return 0 if equal lenght in other case just index
                nextPositionId = (nextPositionId + 1) % _wayPoints.Length;
                nextPosition = _wayPoints[nextPositionId];

                velocity = Vector3.zero;
                yield return _myMonoBehaviour.StartCoroutine(RotateBotToTarget(_wayPoints[nextPositionId]));
                yield return new WaitForSeconds(pauseDelay);
            }
            yield return null;
        }
    }

    IEnumerator RotateBotToTarget(Vector3 targetPoint)
    {
        // calculate direction to target
        Vector3 targetDir = (targetPoint - _objectToMove.position).normalized;
        // calculate angle from direction
        float targetAngle = 90 - Mathf.Atan2(targetDir.z, targetDir.x) * Mathf.Rad2Deg;
        
        // while for smooth changing angle of character between two parametrs
        float deltaBetweenAngles = Mathf.DeltaAngle(_objectToMove.eulerAngles.y, targetAngle);
        while (Mathf.Abs(deltaBetweenAngles) > 0.05f)//
        {
            // we get angle we nead to achieve, then direction and in end we have point we will look
            float angle = Mathf.MoveTowardsAngle(_objectToMove.eulerAngles.y, targetAngle, _rotateSpeed * Time.deltaTime);
            Vector3 dir = Helpers.DirFromAngle(angle, false, _objectToMove.transform).normalized;
            Vector3 point = _objectToMove.transform.position + dir;
            pointToLook = new Vector3(point.x, 0, point.z);

            deltaBetweenAngles = Mathf.DeltaAngle(_objectToMove.eulerAngles.y, targetAngle);
            yield return null;
        }
        pointToLook = new Vector3(targetPoint.x, 0, targetPoint.z); ;
    }



    public void UpdateInput()
    {
    }
}
