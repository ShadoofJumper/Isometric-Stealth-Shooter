using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchVolumeField : FieidVisualization
{

    // mesh of field of character volume pitch
    [SerializeField] private MeshFilter fieldMeshFilter;

    private Mesh fieldMesh;
    private Character _character;
    // paramms for mesh
    private float volumeRange = 0;

    // volume params
    // power, step, speed
    public float[,] volumePramms = new float[3,3]
        {
            //volumePrammsSteals
            { 0.8f, 0.2f, 0.2f},
            //volumePrammsWalk
            { 1.0f, 0.5f, 0.5f},
            //volumePrammsRun
            { 2.0f, 1.5f, 1.5f},
        };


    // start range from this
    private float volumePower = 1;
    // step to chandge
    private float volumeStep = 0.5f;
    // step to chandge
    private float volumeChangeSpeed = 0.5f;
    //
    private float volumeSpeedK;
    // variable to save coroutin object
    private IEnumerator changeScaleIdle;

    // for check different walls whene search for edge
    public float edgeDistanceThresh;
    // resolution of how many time check edge of obstical, for smooth look
    public int obsticalCheckResolution;


    [SerializeField] private float meshResolution;
    // mask for obsticals and targets
    public LayerMask obsticalsMask;
    public LayerMask targetMask;

    // Start is called before the first frame update
    void Start()
    {
        _character = gameObject.GetComponent<Character>();
        // create empty mesh
        fieldMesh               = new Mesh();
        fieldMesh.name          = "Volume field";
        fieldMeshFilter.mesh    = fieldMesh;

        // save coroutin method
        changeScaleIdle = ChangeScaleIdle();
        StartCoroutine(changeScaleIdle);

        // set start for run
        UpdateVolumeParams(1);
    }

    private void UpdateVolumeParams(int speedId)
    {
        volumePower         = volumePramms[speedId, 0];
        volumeStep          = volumePramms[speedId, 1];
        volumeChangeSpeed   = volumePramms[speedId, 2];
        // restart coroutin
        StopCoroutine(changeScaleIdle);
        StartCoroutine(changeScaleIdle);
    }

    private void LateUpdate()
    {
        // dependence of volume on speed
        // speed devide on medium speed and discard one
        volumeSpeedK    = (_character.characterMover.CurrentSpeed / _character.Settings.Speed[1]);//_character.characterMover.CurrentSpeed;
        float volume    = volumeRange + volumePower;

        UpdateVolumeParams(_character.characterMover.SpeedId);

        // get all targets that hear volume
        List<Transform> targetsFound = FindVisibleTargets(volume, targetMask, obsticalsMask);
        // send call to all players they hear
        CallOnPlayerFound(targetsFound);

        // draw volume field
        DrawField(fieldMesh, 0.0f, meshResolution, 360.0f, volume, obsticalCheckResolution, edgeDistanceThresh, obsticalsMask);
    }

    private void CallOnPlayerFound(List<Transform> targetsFound)
    {
        foreach (Transform target in targetsFound)
        {
            // get target character combat
            CharacterCombat characterCombat = SceneController.instance.charactersOnScene[target].characterCombat;
            characterCombat.OnPlayerFound();
        }
    }


    IEnumerator ChangeScaleIdle()
    {
        int k = -1;
        float startScale = volumeRange;
        while (true)
        {

            if      (volumeRange >= (startScale + volumeStep))  { k = -1;}
            else if (volumeRange <= startScale)                 { k =  1; }

            volumeRange = volumeRange + volumeChangeSpeed * k * Time.deltaTime;

            yield return null;
        }

    }


    // method for tagets of filed of view
    public List<Transform> FindVisibleTargets(float viewRadius, LayerMask _targetsMask, LayerMask _obsticalsMask)
    {
        List<Transform> targetsInField = new List<Transform>();

        // find all targets in our range using standart method
        Collider[] alltargets = Physics.OverlapSphere(transform.position, viewRadius, _targetsMask);

        for (int i = 0; i < alltargets.Length; i++)
        {
            Transform target = alltargets[i].transform;
            // get direction to target
            Vector3 dirTotarget = (target.position - transform.position).normalized;

            float dst = Vector3.Distance(target.position, transform.position);
            // make raycast to taget, if between not obsticals, then we can see
            if (!Physics.Raycast(transform.position, dirTotarget, dst, _obsticalsMask))
            {
                targetsInField.Add(target);
            }

        }
        return targetsInField;
    }

}
