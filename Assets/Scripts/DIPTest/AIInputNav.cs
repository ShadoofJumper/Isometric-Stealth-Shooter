using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInputNav : ICharacterInput
{
    // parametrs for ai working
    private Vector3 poinToMove;
    private int currentPointToMove;
    private Vector3[] _wayPoints;
    private Transform _path;
    private CharacterSettings _settings;
    private Transform _objectToMove;
    private MonoBehaviour _myMonoBehavior;
    private FieldOfView _fieldOfView;
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
    //properties
    public bool IsOnPause       { get { return pause; }         set { pause = value; } }
    public bool IsOnRotatePause { get { return rotatePause; }   set { rotatePause = value; } }
    public InteractInfo InteractObejctInfo  => interactInfo;
    public Vector3 PointToMove              => poinToMove;

    public AIInputNav(CharacterSettings settings, Transform objectToMove, MonoBehaviour myMonoBehavior)
    {
        _path           = settings.Path.transform;
        _objectToMove   = objectToMove;
        _myMonoBehavior = myMonoBehavior;
        _settings       = settings;
        _fieldOfView    = objectToMove.GetComponent<FieldOfView>();

        //start initialization
        GetWayPointsFromPath(_path);
        poinToMove          = _wayPoints[currentPointToMove];
        doorMask            = LayerMask.GetMask("Door");
        distanceInteract    = 2.0f;
        angleInteract       = 30.0f;
    }


    private void GetWayPointsFromPath(Transform path)
    {
        // set first point to move
        currentPointToMove = 0;

        _wayPoints = new Vector3[path.childCount];

        for (int i = 0; i < path.childCount; i++)
        {
            _wayPoints[i] = path.GetChild(i).position;
            _wayPoints[i] = new Vector3(_wayPoints[i].x, _objectToMove.position.y, _wayPoints[i].z);
        }
    }

    public void UpdateInput()
    {
        CheckInteractWithObject();
        

        // if get to destination and not on rotate pause already
        if (IsCharacterGetNextPoint() && !rotatePause)
        {
            //return 0 if equal lenght in other case just index
            currentPointToMove = (currentPointToMove + 1) % _wayPoints.Length;
            poinToMove = _wayPoints[currentPointToMove];

            // whene distance completed, then pause true, start coroutin and after progress
            rotatePause = true;
            _myMonoBehavior.StartCoroutine(PauseForRotation(_settings.PauseDelay));
        }
        
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
                Vector3 leftDir = DirFromAngle(angleInteract*-1, false).normalized;
                Vector3 rightDir = DirFromAngle(angleInteract, false).normalized;

                Debug.DrawLine(_objectToMove.transform.position, _objectToMove.transform.position + leftDir * 10, Color.red);
                Debug.DrawLine(_objectToMove.transform.position, _objectToMove.transform.position + rightDir * 10, Color.red);
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
                        //_wayPoints = _wayPoints + new Vector3[1]{ objectCollider.transform.position };
                            //= new Vector3[path.childCount];
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
        int fixBroken = 0;

        while (!doorController.IsOpened())
        {
            yield return null;
        }
        pause = false;
    }


    // TEST
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += _objectToMove.transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    //struct to save info about edges in our field of view
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

}
