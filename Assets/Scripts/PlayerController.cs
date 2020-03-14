using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{

    [SerializeField] private Camera mainCamera;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point =  GetPointToGo();
            MoveAgentToPoint(point);
        }
    }

    private Vector3 GetPointToGo()
    {
        // create raycast from camera
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    public void MoveAgentToPoint(Vector3 point)
    {
        agent.SetDestination(point);
    }
}
