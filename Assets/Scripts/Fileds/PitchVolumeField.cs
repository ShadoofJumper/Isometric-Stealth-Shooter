using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchVolumeField : FieldModVisualization
{
    [Header("Mesh for display volume")]
    // mesh of mask of character volume pitch
    [SerializeField] private MeshFilter fieldColorFilter;

    private Mesh colorMesh;
    private Character _character;
    // paramms for mesh
    private float volumeRange = 0;
    // volume params [power, step, speed]
    [Header("Parameters of moving speed")]
    public float[,] volumePramms = new float[3, 3]
        {
            //volumePrammsSteals
            { 0.8f, 0.2f, 0.2f},
            //volumePrammsWalk
            { 1.0f, 0.5f, 0.5f},
            //volumePrammsRun
            { 2.0f, 1.5f, 1.5f},
        };

    // flag for check is colume changed
    private int lastVolumeId;
    // start range from this
    private float volumePower = 1;
    // step to chandge
    private float volumeStep = 0.5f;
    // step to chandge
    private float volumeChangeSpeed = 0.5f;
    // variable to save coroutin object
    private IEnumerator changeScaleIdle;

    // Start is called before the first frame update
    void Start()
    {
        _character = gameObject.GetComponent<Character>();
        // create empty mesh for mask
        fieldMesh = new Mesh();
        fieldMesh.name           = "Volume field";
        fieldMeshFilter.mesh    = fieldMesh;
        // create empty mesh for mask color
        fieldColorFilter.mesh   = fieldMesh;

        // save coroutin method
        changeScaleIdle = ChangeScaleIdle();

        // set start for run
        UpdateVolumeParams(1);

        // for security
        if (fieldObjectsOverlap > 0)
        {
            fieldObjectsOverlap = 0;
            Debug.Log("For circle must be 0, or can lug!");
        }
    }

    private void UpdateVolumeParams(int speedId)
    {
        if (lastVolumeId != speedId)
        {
            volumePower         = volumePramms[speedId, 0];
            volumeStep          = volumePramms[speedId, 1];
            volumeChangeSpeed   = volumePramms[speedId, 2];
            // restart coroutin
            StopCoroutine(changeScaleIdle);
            StartCoroutine(changeScaleIdle);
            lastVolumeId = speedId;
        }
    }

    private void OnDisable()
    {
        if (changeScaleIdle != null)
        {
            // stop coroutine
            StopCoroutine(changeScaleIdle);
        }
    }


    private void LateUpdate()
    {
        if (_character.characterMover == null)
            return;
        // dependence of volume on speed
        // speed devide on medium speed and discard one
        float volume    = volumeRange + volumePower;
        UpdateVolumeParams(_character.characterMover.SpeedId);

        // get all targets that hear volume
        List<Transform> targetsFound = FindVisibleTargets(transform, volume, targetsMask, obsticalsMask, true);
        // send call to all players they hear
        CallOnPlayerFound(targetsFound);

        // change viewRadius
        viewRadius = volume;

        //        DrawField(maskMesh, transform, 0.0f, meshResolution, 360.0f, volume, obsticalCheckResolution, edgeDistanceThresh, obsticalsMask);

        // draw volume field
        DrawField(transform);
    }

    private void CallOnPlayerFound(List<Transform> targetsFound)
    {
        foreach (Transform target in targetsFound)
        {
            // get target character combat
            Character character  = SceneController.instance.charactersOnScene[target];
            if (character.isAlive)
            {
                CharacterCombat characterCombat = character.characterCombat;
                characterCombat.OnPlayerFound("FAIL MISSION!", "Too much noise!");
            }
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

}
