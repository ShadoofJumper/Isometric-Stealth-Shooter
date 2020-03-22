using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [SerializeField] NavMeshSurface sceneNavMeshSurface;

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

    public void BakeSurface()
    {
        sceneNavMeshSurface.BuildNavMesh();
    }

}
