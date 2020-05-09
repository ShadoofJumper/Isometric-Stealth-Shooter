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

    public static Color hexToColor(string hex, byte alpha = 255)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = alpha;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }

}
