using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMover : ICharacterMover
{
    private CharacterSettings _settings;
    private GameObject _objectToMove;
    private AIInputNav _input;
    private NavMeshAgent agent;
    private NavMeshPath _path;
    private Animator _characterAnimator;
    // steals, walk and run speed
    private SpeedParams _speed;
    private float currentSpeed;
    private int speedId = 1;

    public float CurrentSpeed { get { return currentSpeed; } set { currentSpeed = value; } }
    public int SpeedId { get { return speedId; } set { speedId = value; } }

    public AIMover(ICharacterInput input, GameObject objectToMove, CharacterSettings settings)
    {
        _settings               = settings;
        _objectToMove           = objectToMove;
        _input                  = input as AIInputNav;
        _characterAnimator      = objectToMove.GetComponentInChildren<Animator>();
        _speed                  = new SpeedParams(settings.Speed);
        Vector3 startPos        = _settings.Path.transform.GetChild(0).position;
        SetStartPosition(startPos);
        CreateNavMeshAgent();
    }

    private void CreateNavMeshAgent()
    {
        agent       = _objectToMove.AddComponent<NavMeshAgent>();
        // walk speed default
        agent.speed         = _speed.walk;
        agent.baseOffset    = 1.5f;
        agent.height        = 2.5f;
        _path               = new NavMeshPath();
        agent.CalculatePath(_input.PointToMove, _path);

        // update nav mesh surface
        GameManager.instance.BakeSurface();
    }

    // call in fixed delta time
    public void Move()
    {
        if (!_input.IsOnPause && !_input.IsOnRotatePause)
        {
            ResumeMove();
            agent.SetDestination(_input.PointToMove);
            _path = agent.path;
            //TestPaintPath(_path);
        }
        //if on rotate pause
        else
        {
            if (_input.IsOnRotatePause && !_input.IsOnPause)
            {
                //if final point is achieved theen recalculate new path
                RecalculateNewPathIfNeed();
                //if character on pause doo some stuff
                RotateToNextDirection(_settings.RotateSpeed, _path.corners[1]);
            }
            else
            {
                // pause agent
                PauseMove();
            }
        }
    }


    public void UpdateMoveAnim()
    {
        currentSpeed = agent.velocity.magnitude / _speed.run;
        // update animation
        //_characterAnimator.SetFloat("Speed", currentSpeed);
    }

    // if achiev final path and we not have path (agent not use setdestination to this moment)
    // recalucale by myself
    private void RecalculateNewPathIfNeed()
    {
        // compair just x and z
        Vector3 finalPoint = _path.corners[_path.corners.Length - 1];
        if (    finalPoint.x    == _objectToMove.transform.position.x 
            &&  finalPoint.z    == _objectToMove.transform.position.z)
        {
            _path = new NavMeshPath();
            agent.CalculatePath(_input.PointToMove, _path);
        }
    }

    private void RotateToNextDirection(float delay, Vector3 nextPoint)
    {
        // calculate direction to target
        Vector3 targetDir = (nextPoint - _objectToMove.transform.position).normalized;
        // for test
        Debug.DrawLine(_objectToMove.transform.position, nextPoint, Color.green);
        // calculate rotation
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        // set target rotation in time
        if (delay > 0)
        {
            _objectToMove.transform.rotation = Quaternion.Slerp(_objectToMove.transform.rotation, targetRotation, delay * Time.fixedDeltaTime);
        }
        else
        {
            _objectToMove.transform.rotation = targetRotation;
        }
    }

    private void TestPaintPath(NavMeshPath path)
    {
        Debug.DrawLine(_objectToMove.transform.position, path.corners[0], Color.green);

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Vector3 firstPoint  = path.corners[i];
            Vector3 nextPoint   = path.corners[i+1];
            Debug.DrawLine(firstPoint, nextPoint, Color.green);
        }

    }


    private Vector3 GetAngleToTarget(Vector3 targetPoint)
    {
        float angle = 0;
        // calculate direction to target
        Vector3 targetDir = (targetPoint - _objectToMove.transform.position).normalized;
        // calculate angle from direction
        float targetAngle = 90 - Mathf.Atan2(targetDir.z, targetDir.x) * Mathf.Rad2Deg;

        // while for smooth changing angle of character between two parametrs
        float deltaBetweenAngles = Mathf.DeltaAngle(_objectToMove.transform.eulerAngles.y, targetAngle);
        while (Mathf.Abs(deltaBetweenAngles) > 0.05f)//
        {
            // we get angle we nead to achieve, then direction and in end we have point we will look
            angle = Mathf.MoveTowardsAngle(_objectToMove.transform.eulerAngles.y, targetAngle, _settings.RotateSpeed * Time.deltaTime);
            
            deltaBetweenAngles = Mathf.DeltaAngle(_objectToMove.transform.eulerAngles.y, targetAngle);
        }
        return new Vector3(0, angle, 0);
    }

    public void SetStartPosition(Vector3 startPos)
    {
        // position
        _objectToMove.transform.position = new Vector3(startPos.x, _objectToMove.transform.position.y, startPos.z);

        // rotation
        RotateToNextDirection(0, _input.PointToMove);
    }

    public void PauseMove()
    {
        _input.IsOnPause = true;
        agent.isStopped = true;
    }

    public void ResumeMove()
    {
        _input.IsOnPause = false;
        if (agent.isStopped == true)
            agent.isStopped = false;
    }

    public void UpdateMover()
    {

        // for test
        if (Input.GetKeyDown(KeyCode.J))
        {
            // set pause while down
            PauseMove();
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
            // resume
            ResumeMove();

        }
    }

}
