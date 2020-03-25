using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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



}
