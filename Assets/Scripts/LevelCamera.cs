using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCamera : FieldModVisualization
{
    [Header("Pivots for camera")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform fieldPivot;

    [Header("Camere settings")]
    // for rotation
    [Range(0, 360)]
    [SerializeField] private float diapasonAngle;
    // for field see meshs
    [Range(0, 360)]
    [SerializeField] private float seeAngle;
    [SerializeField] private float diapasonRadius;
    [SerializeField] private float speed;

    [Header("Use quarternion for rotate")]
    [SerializeField] private bool isQuarternion = true;
    // for change rotation in opossir direction
    private float k = 1f;
    // Euler
    private Vector3 currentAngle = Vector3.zero;
    // Qurternion
    private float startRotation;
    private float currentRotation;
    // start camera global rotate
    private float globalRotate;


    [HideInInspector]
    private List<Transform> targetsInField = new List<Transform>();
    // ---------------------------------------------------
    private bool isSearchCharacter = true;
    public bool IsSearchCharacter { get { return isSearchCharacter; } set { isSearchCharacter = value; } }


    private void Start()
    {
        // create empty mesh for mask
        fieldMesh = new Mesh();
        fieldMesh.name = "Camera field";
        fieldMeshFilter.mesh = fieldMesh;
        StartCoroutine(FindTargetWithDelay(0.2f));

        globalRotate = transform.eulerAngles.y;

        SceneController.instance.camerasOnScene.Add(transform, this);
    }

    private void Update()
    {
        if (isQuarternion) {
            RotateCameraQuaternion();
        } else {
            RotateCameraEuler();
        }

        // check if character detect
        if (isSearchCharacter && targetsInField.Count > 0)
        {
            OnPlayerFound("FAIL MISSION!", "Camera see you!");
        }
    }

    private void LateUpdate()
    {
        DrawField(fieldPivot);
    }


    private void RotateCameraQuaternion()
    {
        float angleLimit = diapasonAngle / 2 * k;
        currentRotation += Mathf.Lerp(startRotation, angleLimit, speed * Time.deltaTime);
        // set rotation
        cameraPivot.rotation = Quaternion.AngleAxis(currentRotation + globalRotate, Vector3.up);
        if (    currentRotation >= diapasonAngle / 2 
            ||  currentRotation <= diapasonAngle / -2
            )
        {
            k = k * -1;
        }
    }

    private void RotateCameraEuler()
    {
        Vector3 angleLimit  = new Vector3(0, diapasonAngle / 2 * k, 0);
        currentAngle        += Vector3.Lerp(Vector3.zero, angleLimit, speed * Time.deltaTime);
        // clamp in diapasone
        currentAngle        = new Vector3(currentAngle.x, Mathf.Clamp(currentAngle.y, diapasonAngle / -2, diapasonAngle / 2), currentAngle.z);
        // set angle
        cameraPivot.localEulerAngles = currentAngle;

        if (Vector3.Distance(currentAngle, angleLimit) < 1f)
        {
            k = k * -1; // reverse limit
        }
    }



    // ---------------------- For check targets in field of view ----------------------
    //corutin for delay search of targets
    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            targetsInField = FindVisibleTargets(fieldPivot.transform, diapasonRadius, targetsMask, obsticalsMask, false, diapasonAngle);
            PaintLineToTarget(transform, targetsInField);
        }
    }
    public void OnPlayerFound(string failtext, string failDescription)
    {
        if (MissionManager.instance.IsSilenceRequireMain)
        {
            // show some animation about faill

            // show warning about fail
            GameManager.instance.FailLevel(failtext, failDescription);
        }
    }
    // --------------------------------------------------------------------------------


}
