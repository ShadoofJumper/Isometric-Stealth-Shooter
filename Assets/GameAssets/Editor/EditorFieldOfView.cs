using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(FieldOfView))]
public class EditorFieldOfView : Editor
{
    void OnSceneGUI()
    {
        // target is ganeobject that we inspect
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;
        // paint area circle where we see
        Handles.DrawWireArc(fow.transform.position, Vector3.up, fow.transform.forward, 360, fow.viewRadius);

        // get angle for lines of field view 
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle/2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle/2, false);

        // paint tow lines
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        // for all targets paint a red line
        Handles.color = Color.red;
        foreach (Transform target in fow.TargetsInField)
        {
            Handles.DrawLine(fow.transform.position, target.position);
        }
    }
}
