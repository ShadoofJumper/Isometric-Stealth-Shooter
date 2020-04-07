using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : FieidVisualization
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    // ray cout resolution
    public float meshResolution;
    // resolution of how many time check edge of obstical, for smooth look
    public int obsticalCheckResolution;
    // for check different walls whene search for edge
    public float edgeDistanceThresh;
    // for file of view mesh can leat bit overlap objects
    public float fieldObjectsOverlap = 0.15f;

    // mask for obsticals and targets
    public LayerMask obsticalsMask;
    public LayerMask targetsMask;
    public LayerMask interactMask;

    // mesh of field of view visualization
    public MeshFilter fieldMeshFilter;
    private Mesh fieldMesh;

    // list of target we can see
    [HideInInspector]
    private List<Transform> targetsInField = new List<Transform>();
    private List<Collider> objectsInRange = new List<Collider>();

    public List<Collider> ObjectsInRange => objectsInRange;
    public List<Transform> TargetsInField => targetsInField;

    private void Start()
    {
        // create empty mesh
        fieldMesh               = new Mesh();
        fieldMesh.name          = "View field";
        fieldMeshFilter.mesh    = fieldMesh;
        StartCoroutine("FindTargetWithDelay", .2f);
    }

    private void LateUpdate()
    {
        // send all info, mb need send struct of all need params
        DrawField(fieldMesh, fieldObjectsOverlap, meshResolution, viewAngle, viewRadius, obsticalCheckResolution, edgeDistanceThresh, obsticalsMask);
    }

    //corutin for delay search of targets
    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
            PaintLineToTarget();
            PaintLineToInteractObjects();
        }
    }

    private void PaintLineToTarget()
    {
        foreach (Transform targetTransform in targetsInField)
        {
            Debug.DrawLine(transform.position, targetTransform.position, Color.red);
        }
    }

    private void PaintLineToInteractObjects()
    {
        foreach (Collider targetTransform in objectsInRange)
        {
            Debug.DrawLine(transform.position, targetTransform.transform.position, Color.yellow);
        }
    }


    // method for tagets of filed of view
    public void FindVisibleTargets()
    {
        targetsInField.Clear();
        objectsInRange.Clear();
        // find all targets in our range using standart method
        Collider[] alltargets   = Physics.OverlapSphere(transform.position, viewRadius, targetsMask);
        // find all object we can interacte
        objectsInRange.AddRange(Physics.OverlapSphere(transform.position, viewRadius, interactMask));

        for (int i = 0; i < alltargets.Length; i++)
        {
            Transform target    = alltargets[i].transform;
            // get direction to target
            Vector3 dirTotarget   = (target.position - transform.position).normalized;
            //find angle to ratget
            float angleToTarget = Vector3.Angle(transform.forward, dirTotarget);

            // if target in field of view
            if (angleToTarget < viewAngle / 2)
            {            
                float dst = Vector3.Distance(target.position, transform.position);
                // make raycast to taget, if between not obsticals, then we can see
                if (!Physics.Raycast(transform.position, dirTotarget, dst, obsticalsMask))
                {
                    targetsInField.Add(target);
                }
            }
        }

    }


}
