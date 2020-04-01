using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public Transform path;
    public float speed;
    public float waitTime;
    public float rotateSpeed = 90;

    private Vector3[] wayPoints;

    private void Start()
    {
        wayPoints = new Vector3[path.childCount];

        for (int i = 0; i < path.childCount; i++)
        {
            wayPoints[i] = path.GetChild(i).position;
            wayPoints[i] = new Vector3(wayPoints[i].x, transform.position.y, wayPoints[i].z);
        }
        StartCoroutine("MoveBotToPoint", waitTime);

    }


    private void OnDrawGizmos()
    {
        Vector3 startPosition = path.GetChild(0).position;
        Vector3 previosPosition = startPosition;

        // create path point vizualisation
        foreach (Transform pathPoint in path)
        {
            Gizmos.DrawSphere(pathPoint.position, 0.2f);
            //paint lines between points
            Gizmos.DrawLine(previosPosition, pathPoint.position);
            previosPosition = pathPoint.position;
        }
        Gizmos.DrawLine(previosPosition, startPosition);
    }

    IEnumerator MoveBotToPoint(float pausaTime)
    {
        // set start position
        transform.position = wayPoints[0];



        float moveStep = speed * Time.deltaTime;
        // set first point to move
        int nextPositionId = 1;
        Vector3 nextPosition = wayPoints[nextPositionId];
        transform.LookAt(nextPosition);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveStep);
            if (transform.position == nextPosition)
            {
                //return 0 if equal lenght in other case just index
                nextPositionId = (nextPositionId + 1) % wayPoints.Length;
                nextPosition = wayPoints[nextPositionId];
                yield return StartCoroutine("RotateBotToTarget", wayPoints[nextPositionId]);
                yield return new WaitForSeconds(pausaTime);
            }
            yield return null;
        }
    }

    IEnumerator RotateBotToTarget(Vector3 targetPoint)
    {
        // calculate direction to target
        Vector3 targetDir = (targetPoint - transform.position).normalized;
        // calculate angle from direction
        float targetAngle = 90 - Mathf.Atan2(targetDir.z, targetDir.x) * Mathf.Rad2Deg;
        //Debug.Log(targetAngle);
        float deltaBetweenAngles = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        while (Mathf.Abs(deltaBetweenAngles) > 0.05f)//
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, rotateSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            deltaBetweenAngles = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
            yield return null;
        }
    }



}
