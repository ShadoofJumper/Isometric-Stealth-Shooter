﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : FieldModVisualization
{

    // list of target we can see
    [HideInInspector]
    private List<Transform> targetsInField  = new List<Transform>();
    private List<Collider> objectsInRange   = new List<Collider>();

    public List<Collider> ObjectsInRange    => objectsInRange;
    public List<Transform> TargetsInField   => targetsInField;


    // hot spot of filed of view
    [Header("Hotspot of field")]
    public Transform hotSpot;

    private void Start()
    {
        // create empty mesh
        fieldMesh               = new Mesh();
        fieldMesh.name          = "View field";
        fieldMeshFilter.mesh    = fieldMesh;
        StartCoroutine("FindTargetWithDelay", .2f);
        if(!GetComponent<Character>().isPlayer)
            StartCoroutine("FindObjectsWithDelay", .2f);
    }

    private void LateUpdate()
    {
        // draw field, set parent of field
        DrawField(hotSpot != null ? hotSpot : transform);
    }

    private void OnDisable()
    {
        StopCoroutine("FindTargetWithDelay");
        StopCoroutine("FindObjectsWithDelay");
    }

    //corutin for delay search of targets
    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            targetsInField = FindVisibleTargets(transform, viewRadius, targetsMask, obsticalsMask, false, viewAngle);
            PaintLineToTarget(transform, targetsInField);
        }
    }

    IEnumerator FindObjectsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            objectsInRange = Helpers.FindTargets(transform.position, viewRadius, interactMask);
            PaintLineToInteractObjects(transform, objectsInRange);
        }
    }

}
