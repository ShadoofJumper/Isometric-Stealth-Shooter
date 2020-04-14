using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Same as FieidVisualization but with additional functionality
public class FieldModVisualization : FieidVisualization
{
    // method for tagets of filed of view
    public List<Transform> FindVisibleTargets(Transform center, float viewRadius, LayerMask targetsMask, LayerMask obsticalsMask, bool fullCircle, float viewAngle = 0)
    {
        List<Transform> targetsInField = new List<Transform>();
        // find all targets in our range using standart method
        Collider[] alltargets = Physics.OverlapSphere(center.position, viewRadius, targetsMask);

        //Debug.Log("FindVisibleTargets: ");
        for (int i = 0; i < alltargets.Length; i++)
        {
            Transform target = alltargets[i].transform;
            Vector3 dirTotarget = (target.position - center.position).normalized;

            float angleToTarget;

            //find angle to taget
            angleToTarget = Vector3.Angle(center.forward, dirTotarget);
            //Debug.Log("target1: " + alltargets[i].transform.name);
            // if check full range or target in field of view
            if (fullCircle || angleToTarget < viewAngle / 2)
            {
                //Debug.Log("target2: " + alltargets[i].transform.name);
                if (!CheckObsticalBetween(center.position, target.position, dirTotarget, obsticalsMask))
                {
                    targetsInField.Add(target);
                }
            }

        }
        return targetsInField;
    }

    public bool CheckObsticalBetween(Vector3 a, Vector3 b, Vector3 dirTotarget, LayerMask obsticalsMask)
    {
        float dst = Vector3.Distance(a, b);
        // make raycast to taget, if between not obsticals, then we can see
        if (!Physics.Raycast(a, dirTotarget, dst, obsticalsMask))
        {
            return false;
        }
        return true;
    }

    public void PaintLineToTarget(Transform parent, List<Transform> targetsInField)
    {
        foreach (Transform targetTransform in targetsInField)
        {
            Debug.DrawLine(parent.position, targetTransform.position, Color.red);
        }
    }

    public void PaintLineToInteractObjects(Transform parent, List<Collider> objectsInRange)
    {
        foreach (Collider targetTransform in objectsInRange)
        {
            Debug.DrawLine(transform.position, targetTransform.transform.position, Color.yellow);
        }
    }
}
