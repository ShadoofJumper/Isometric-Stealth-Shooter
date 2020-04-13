using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : FieldModVisualization
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

    // hot spot of filed of view
    public Transform hotSpot;

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
        DrawField(fieldMesh, transform, fieldObjectsOverlap, meshResolution, viewAngle, viewRadius, obsticalCheckResolution, edgeDistanceThresh, obsticalsMask);
    }

    //corutin for delay search of targets
    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            targetsInField = FindVisibleTargets(transform, viewRadius, targetsMask, obsticalsMask, false, viewAngle);
            objectsInRange = FindTargets(transform.position, viewRadius, interactMask);
            PaintLineToTarget(transform, targetsInField);
            PaintLineToInteractObjects(transform, objectsInRange);
        }
    }


}
