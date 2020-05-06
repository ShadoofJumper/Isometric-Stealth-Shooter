using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Helpers
{
    public static List<Collider> FindTargets(Vector3 position, float viewRadius, LayerMask targetMask)
    {
        List<Collider> objectsInRange = new List<Collider>();
        // find all object we can interacte
        objectsInRange.AddRange(Physics.OverlapSphere(position, viewRadius, targetMask));
        return objectsInRange;
    }

    // method for get direction from angle
    public static Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal, Transform parent)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += parent.transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    
    public static Vector3 ClampMagnitudeCust(Vector3 v, float min, float max)
    {
        double dm = v.sqrMagnitude;
        if (dm > (double)max * (double)max) return v.normalized * max;
        else if (dm < (double)min * (double)min) return v.normalized * min;
        return v;
    }

}
