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

    public AIMover(ICharacterInput input, GameObject objectToMove, CharacterSettings settings)
    {
        _settings = settings;
        _objectToMove = objectToMove;
        _input = input as AIInputNav;

        CreateNavMeshAgent();
    }

    private void CreateNavMeshAgent()
    {
        agent = _objectToMove.AddComponent<NavMeshAgent>();
        agent.speed = _settings.Speed;
        // update nav mesh surface
        GameManager.instance.BakeSurface();
        Debug.Log("here!");
    }

    public void Move()
    {
        if (!_input.IsOnPause)
        {
            agent.SetDestination(_input.PointToMove);
        }
        else
        {
            //if character on pause doo some stuff
            Debug.Log("Do stuff: "+ agent.path.corners[0]);
            //_objectToMove.transform.eulerAngles = GetAngleToTarget(agent.path.corners[0]);
            // calculate direction to target
            Vector3 targetDir = (agent.path.corners[0] - _objectToMove.transform.position).normalized;
            // calculate angle from direction
            float targetAngle = 90 - Mathf.Atan2(targetDir.z, targetDir.x) * Mathf.Rad2Deg;
            // calculate letp
            Debug.Log("targetAngle: " + targetAngle);
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

    public void SetStartPosition()
    {

       Transform firstPoint = _settings.Path.transform.GetChild(0);
       _objectToMove.transform.position = firstPoint.position;

    }

    public void UpdateMover()
    {
    }
}
