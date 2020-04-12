using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInputNav : ICharacterInput
{
    // parametrs for ai working
    private Vector3             poinToMove;
    private int                 currentPointToMove;
    private List<Point>         _wayPoints;
    private Transform           _path;
    private CharacterSettings   _settings;
    private Transform           _objectToMove;
    private MonoBehaviour       _myMonoBehavior;
    private FieldOfView         _fieldOfView;
    // general pause for all stuff
    private bool pause = false;
    // for info that we pause for rotate and can rotate
    private bool rotatePause = false;
    // distance to interact object
    private float distanceInteract;
    private float angleInteract;
    // mask for objects can interact
    private LayerMask doorMask;
    // info about object can interact
    private InteractInfo interactInfo;
    //for shoot mechanic
    private MouseInput[] mouseInput = new MouseInput[] { new MouseInput(), new MouseInput() };
    private bool isPressReload  = false;
    // for calculate currecnt speed of character
    private Vector3 previosPos;
    //properties
    public bool IsOnPause       { get { return pause; }         set { pause = value; } }
    public bool IsOnRotatePause { get { return rotatePause; }   set { rotatePause = value; } }
    public InteractInfo InteractObejctInfo  => interactInfo;
    public Vector3 PointToMove              => poinToMove;
    public MouseInput[] MouseInput { get { return mouseInput; } }
    public bool IsPressReload               => isPressReload;

    public AIInputNav(CharacterSettings settings, Transform objectToMove, MonoBehaviour myMonoBehavior)
    {
        _path           = settings.Path.transform;
        _objectToMove   = objectToMove;
        _myMonoBehavior = myMonoBehavior;
        _settings       = settings;
        _fieldOfView    = objectToMove.GetComponent<FieldOfView>();

        //start initialization
        GetWayPointsFromPath(_path);
        poinToMove          = _wayPoints[currentPointToMove].position;
        doorMask            = LayerMask.GetMask("Door");
        distanceInteract    = 2.0f;
        angleInteract       = 30.0f;
    }


    private void GetWayPointsFromPath(Transform path)
    {
        // set first point to move (not zero, on zero we start)
        currentPointToMove = 1;

        _wayPoints = new List<Point>();
        for (int i = 0; i < path.childCount; i++)
        {
            Vector3 pathPoint = new Vector3(
                path.GetChild(i).position.x, 
                _objectToMove.position.y, 
                path.GetChild(i).position.z
            );
            float pauseDelay = path.GetChild(i).GetComponent<Waypoint>().pause;
            Point point = new Point(pathPoint, pauseDelay);
            _wayPoints.Add(point);
        }
        //TestPrint();
    }

    public void UpdateInput()
    {

        CheckInteractWithObject();
        // if get to destination and not on rotate pause already
        if (IsCharacterGetNextPoint() && !rotatePause)
        {
            float pauseDelay = _wayPoints[currentPointToMove].pauseDelay;

            //return 0 if equal lenght in other case just index
            currentPointToMove = (currentPointToMove + 1) % _wayPoints.Count;
            poinToMove = _wayPoints[currentPointToMove].position;
            // whene distance completed, then pause true, start coroutin and after progress
            rotatePause = true;
            _myMonoBehavior.StartCoroutine(PauseForRotation(pauseDelay));
        }
    }

    private void AddPointToWayPath(int index, Vector3 pointPosition, float pauseDelay)
    {
        // if this point not in path alredy
        if (Vector3.Distance(_wayPoints[index].position, pointPosition) > 0.1)
        {
            Vector3 pathPoint = new Vector3(
                pointPosition.x,
                _objectToMove.position.y,
                pointPosition.z
            );
            Point waypoint = new Point(pathPoint, pauseDelay);
            _wayPoints.Insert(index, waypoint);
        }
        // update next point move
        poinToMove = _wayPoints[currentPointToMove].position;
    }



    public bool IsCharacterGetNextPoint()
    {
        return Vector3.Distance(_objectToMove.position, poinToMove) < 0.5 ;
    }

    IEnumerator PauseForRotation(float pauseDelay)
    {
        yield return new WaitForSeconds(pauseDelay);
        //pause = false;
        rotatePause = false;
    }

    private void CheckInteractWithObject()
    {
        List<Collider> interacteObjects = _fieldOfView.ObjectsInRange;
        interactInfo =  new InteractInfo();
        foreach (Collider objectCollider in interacteObjects)
        {
            //bit compaire operation
            if (1 << objectCollider.gameObject.layer == doorMask)
            {
                //if distance to door less then distance interacte
                float dist = Vector3.Distance(objectCollider.transform.position, _objectToMove.transform.position);
                
                //// dir to door
                Vector3 dirToDoor = (objectCollider.transform.position - _objectToMove.transform.position).normalized;
                
                ////angle to door
                float angleToDoor = Vector3.Angle(_objectToMove.forward, dirToDoor);
                

                //test
                //Vector3 leftDir = DirFromAngle(angleInteract*-1, false).normalized;
                //Vector3 rightDir = DirFromAngle(angleInteract, false).normalized;

                //Debug.DrawLine(_objectToMove.transform.position, _objectToMove.transform.position + leftDir * 10, Color.red);
                //Debug.DrawLine(_objectToMove.transform.position, _objectToMove.transform.position + rightDir * 10, Color.red);
                //Debug.Log($"Door angleToDoor{angleToDoor}, angleInteract{angleInteract}, dist {dist}" );

                // check distance and angle look
                if (dist <= distanceInteract && angleToDoor < angleInteract)
                {
                    //save interact type in var
                    interactInfo = new InteractInfo(INTERACTS_TYPE_CONST.OpenDoor, objectCollider.gameObject);
                    // pause
                    if (!pause)
                    {
                        pause = true;
                        _myMonoBehavior.StartCoroutine(PauseMoveNearDoor());
                        // add to path a point to correct door entrance
                        AddPointToWayPath(currentPointToMove, objectCollider.transform.position, 0);
                    }
                }
            }
            else
            {
                //
            }
        }
    }

    IEnumerator PauseMoveNearDoor()
    {
        //TO DO fix ugle codee
        DoorController doorController = interactInfo.interactObject.GetComponent<DoorController>();
        while (!doorController.IsOpened())
        {
            yield return null;
        }
        pause = false;
    }


    // TEST

    private void TestPrint()
    {
        Debug.Log($"-------------------- TestPrint ({_wayPoints.Count}) -------------------");
        foreach (Point point in _wayPoints)
        {
            Debug.Log("Point: " + point.position);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += _objectToMove.transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    //struct to save info about interact
    public struct InteractInfo
    {
        public INTERACTS_TYPE_CONST interactType;
        public GameObject interactObject;

        public InteractInfo(INTERACTS_TYPE_CONST _interactType, GameObject _interactObject)
        {
            interactType = _interactType;
            interactObject = _interactObject;
        }

    }

    //struct to save info about waypoint
    public struct Point
    {
        public Vector3 position;
        public float pauseDelay;

        public Point(Vector3 _position, float _pauseDelay)
        {
            position = _position;
            pauseDelay = _pauseDelay;
        }

    }

}
