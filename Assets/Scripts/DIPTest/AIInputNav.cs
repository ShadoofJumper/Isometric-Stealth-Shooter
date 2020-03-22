using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInputNav : ICharacterInput
{
    private Vector3 poinToMove;
    private int currentPointToMove;
    private Vector3[] _wayPoints;
    private Transform _path;
    private CharacterSettings _settings;
    private Transform _objectToMove;
    private MonoBehaviour _myMonoBehavior;
    private bool rotatePause = false;

    public Vector3 PointToMove { get { return poinToMove; } }
    public bool IsOnPause { get { return rotatePause; } }

    public AIInputNav(CharacterSettings settings, Transform objectToMove, MonoBehaviour myMonoBehavior)
    {
        _path = settings.Path.transform;
        _objectToMove = objectToMove;
        _myMonoBehavior = myMonoBehavior;
        _settings = settings;

        GetWayPointsFromPath(_path);
        poinToMove = _wayPoints[currentPointToMove];
    }


    private void GetWayPointsFromPath(Transform path)
    {
        // set first point to move
        currentPointToMove = 0;

        _wayPoints = new Vector3[path.childCount];

        for (int i = 0; i < path.childCount; i++)
        {
            _wayPoints[i] = path.GetChild(i).position;
            _wayPoints[i] = new Vector3(_wayPoints[i].x, _objectToMove.position.y, _wayPoints[i].z);
        }
    }

    public void UpdateInput()
    {
        //Debug.Log("Dist: " + Vector3.Distance(_objectToMove.position, poinToMove));
        // if get to destination
        if (IsCharacterGetNextPoint() && !rotatePause)
        {
            // whene distance completed, then pause true, start coroutin and after progress
            rotatePause = true;
            _myMonoBehavior.StartCoroutine(PauseForRotation(_settings.PauseDelay));

            //return 0 if equal lenght in other case just index
            currentPointToMove = (currentPointToMove + 1) % _wayPoints.Length;
            poinToMove = _wayPoints[currentPointToMove];
        }
        //Debug.Log($"Move to id: {currentPointToMove}, point: {poinToMove}");
    }

    public bool IsCharacterGetNextPoint()
    {
        return Vector3.Distance(_objectToMove.position, poinToMove) < 0.5 ;
    }

    IEnumerator PauseForRotation(float pauseDelay)
    {
        yield return new WaitForSeconds(pauseDelay);
        rotatePause = false;
    }

}
