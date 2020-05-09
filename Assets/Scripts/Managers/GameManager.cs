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

    //FOR TEST
    [SerializeField] private Weapon weaponMain;
    [SerializeField] private Weapon weaponSecond;
    [SerializeField] private Weapon weaponExtra;

    [SerializeField] private Texture2D cursorAim;
    [SerializeField] private NavMeshSurface sceneNavMeshSurface;
    [SerializeField] private CameraController cameraController;
    // fader for smooth transition between scenes
    [SerializeField] private SceneFader sceneFader;

    // settings of current level
    public LevelSettings levelSetting;

    // global game params
    private static bool isGameOnPause = false;
    private static bool isGameOnWarning = false;

    public static bool  IsGameOnWarning { get { return isGameOnWarning; } set { isGameOnWarning = value; } }
    public static bool  IsGameOnPause   { get { return isGameOnPause; } }

    // player mission
    [SerializeField] private Transform mainMissionMarker;

    //here store all method need to call when doorway enter
    public event Action<int, float> onDoorwayTriggerEnter;
    public event Action<int, float> onDoorwayTriggerExit;

    // -------------- Call from other script for open doors than subscribe to action
    public void DoorwayTriggerEnter(int doorId, float time)
    {
        if (onDoorwayTriggerEnter != null)
        {
            onDoorwayTriggerEnter(doorId, time);
        }
    }

    public void DoorwayTriggerExit(int doorId, float time)
    {
        if (onDoorwayTriggerExit != null)
        {
            onDoorwayTriggerExit(doorId, time);
        }
    }

    private void CloseAllDoors(float time = 0.0f)
    {
        // for all tween has been stored
        Debug.Log(SceneController.instance.DoorsParent.name);
        DoorController[] doorControllers = SceneController.instance.DoorsParent.GetComponentsInChildren<DoorController>();

        foreach (DoorController doorController in doorControllers)
        {
            LTDescr tweenDoorMove = doorController.TweenDoorMove;
            if (tweenDoorMove != null)
            {
                LeanTween.cancel(tweenDoorMove.id);
            }
        }


        if (onDoorwayTriggerExit != null)
        {
            int doorsCount = onDoorwayTriggerExit.GetInvocationList().Length;

            // TO DO id can be not == count of doors
            for (int doorId = 1; doorId < doorsCount; doorId++)
            {
                onDoorwayTriggerExit(doorId, time);
            }
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
        StartLevel(1);
        //Cursor.SetCursor(cursorAim, Vector2.zero, CursorMode.Auto);
    }

    public void StartLevel(int levelId)
    {
        SceneController.instance.EnemySettings = levelSetting.EnemySettings;
        SceneController.instance.SpawnPlayer();
        SceneController.instance.SpawnAllEnemys();

        // add mission on start
        List<string> missionTasks   = levelSetting.StartMissionsTasks;
        List<bool> missionIsSilances = levelSetting.StartMissionsIsSilance;
        for (int i = 0; i < missionTasks.Count; i++)
        {
            string  missionName         = missionTasks[i];
            bool    missionIsSilance    = missionIsSilances[i];
            MissionManager.instance.AddMission(missionName, mainMissionMarker, missionIsSilance);

        }

        // block player gameplay
        Character player = SceneController.instance.player;
        //player.PauseCharacter();
        // For main mission use camera
        //cameraController.ShowPoint(mainMissionMarker, 2.0f, true, 1.0f, 3.0f, delegate { player.ResumeCharacter(); });

        //test
        //SceneController.instance.playerInventory.AddWeapon(weaponMain);
        //SceneController.instance.playerInventory.AddWeapon(weaponSecond);
        //SceneController.instance.playerInventory.AddWeapon(weaponExtra);
        SceneController.instance.playerInventory.AddItem(weaponMain);
        Debug.Log($"Weapon test: {weaponMain.WeaponSettings.Name}");
    }

    public void RestartLevel()
    {
        // disable warning
        isGameOnWarning = false;
        //resume game time and reset pause
        ResumeGameLogic();
        // reload scene
        sceneFader.FadeTo("Game", 2.0f);
    }

    public void StartNextLevel()
    {
        Debug.Log("Load next scene!");

    }

    IEnumerator FailOverTime()
    {
        yield return new WaitForSeconds(2.2f);
        // test fail level
        FailLevel("Fail!!!");
    }


    public void CompleteLevel()
    {
        // show warning that complete level
        UIController.instance.ShowWarning("Complete game!", true, "MENU", UIController.instance.ReturnMenu);
        PauseGameLogic();
        isGameOnWarning = true;
    }

    public void FailLevel(string failText, string failDesciption = "")
    {
        // show warning that complete level
        UIController.instance.ShowWarning(failText, true, "RESTART", RestartLevel, failDesciption);
        // disable characters calculation
        foreach (KeyValuePair<Transform, Character> characterPair in SceneController.instance.charactersOnScene)
        {
            if (characterPair.Value.isPlayer)
            {
                PitchVolumeField volumeComp = characterPair.Value.gameObject.GetComponent<PitchVolumeField>();
                if (volumeComp != null)
                {
                    characterPair.Value.gameObject.GetComponent<PitchVolumeField>().enabled = false;
                }
                
            }
            characterPair.Value.enabled = false;
        }
        // disable player volume

        // disable cameras search
        foreach (KeyValuePair<Transform, LevelCamera> characterPair in SceneController.instance.camerasOnScene)
        {
            characterPair.Value.IsSearchCharacter = false;
        }

        PauseGameLogic();
        isGameOnWarning = true;
    }



    public void PauseGameLogic()
    {
        Time.timeScale = 0f;
        isGameOnPause = true;
    }

    public void PauseAllCharactersMove()
    {
        foreach (KeyValuePair<Transform, Character> characterPair in SceneController.instance.charactersOnScene)
        {
            //get character mover
            Character character = characterPair.Value;
            character.PauseCharacter();
        }
    }

    public void ResumeGameLogic()
    {
        // here we do not need resume game from in game menu, if game on warning
        if (!isGameOnWarning)
        {
            Time.timeScale = 1.0f;
            isGameOnPause = false;
        }
    }





}
