using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Singlton

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Try create another instance of game manager!");
        }
        
    }
    #endregion

    [SerializeField] NavMeshSurface sceneNavMeshSurface;

    //
    private static bool isGameOnPause = false;
    private static bool isGameOnWarning = false;
    public static bool  IsGameOnPause => isGameOnPause;

    // player mission
    public Character player;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform mainMissionMarker;


    //here store all method need to call when doorway enter
    public event Action<int> onDoorwayTriggerEnter;
    public event Action<int> onDoorwayTriggerExit;

    // -------------- Call from other script for open doors than subscribe to action
    public void DoorwayTriggerEnter(int doorId)
    {
        if (onDoorwayTriggerEnter != null)
        {
            onDoorwayTriggerEnter(doorId);
        }
    }

    public void DoorwayTriggerExit(int doorId)
    {
        if (onDoorwayTriggerExit != null)
        {
            onDoorwayTriggerExit(doorId);
        }
    }
    // ---------------------------------------------------------------

    public void BakeSurface()
    {
        sceneNavMeshSurface.BuildNavMesh();
    }

    // ---------------------------- Mission ------------------------------

    private void Start()
    {
        SpawnPlayer();

        // add main mission on start
        MissionManager.instance.AddMission("MISSION_MAIN", mainMissionMarker);

        //test
        //StartCoroutine(FailOverTime());
    }

    IEnumerator FailOverTime()
    {
        yield return new WaitForSeconds(0);
        // test fail level
        FailLevel();
    }

    private void SpawnPlayer()
    {
        player.characterMover.SetStartPosition(startPosition.position);
    }


    public void CompleteLevel()
    {
        // show warning that complete level
        UIController.instance.ShowWarning("Complete level!");
        PauseGameLogic();
        isGameOnWarning = true;
    }

    public void FailLevel()
    {
        // show warning that complete level
        UIController.instance.ShowWarning("Fail level!", true, "RESTART", RestartLevel);
        PauseGameLogic();
        isGameOnWarning = true;
    }

    public void RestartLevel()
    {
        Debug.Log("Restart here!");
    }

    public void PauseGameLogic()
    {
        Time.timeScale = 0f;
        isGameOnPause = true;
    }

    public void ResumeGameLogic()
    {
        if (!isGameOnWarning)
        {
            Time.timeScale = 1.0f;
            isGameOnPause = true;
        }
    }





    // ----------------------------- Dev -----------------------------
    // paint gizmos if ai
    private void OnDrawGizmos()
    {
        // paint start point
        Gizmos.DrawSphere(startPosition.position, 0.6f);

    }


}
