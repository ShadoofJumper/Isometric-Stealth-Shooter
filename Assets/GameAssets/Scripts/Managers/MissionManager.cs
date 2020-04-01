using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{

    #region Singlton

    public static MissionManager instance;

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

    private bool isGameComplete;
    private Dictionary<string, Mission> missionDict = new Dictionary<string, Mission>();
    private int missionCount = 0;

    // general level property
    private bool isSilenceRequireMain = false;
    public  bool IsSilenceRequireMain { get {return isSilenceRequireMain;} }

    public void AddMission(string missonKey, Transform marker, bool isSilenceRequireMain = false)
    {
        // update general silence flag
        this.isSilenceRequireMain = isSilenceRequireMain;
        //create mission
        string text = MissionTasks.TASKS[missonKey];
        Mission newMission = new Mission(missionCount, missonKey, text, marker, isSilenceRequireMain);
        // add to dict of missions
        missionDict.Add(missonKey, newMission);
        missionCount += 1;
        // add to UI
        UIController.instance.AddMissionToPanel(newMission);
    }

    public void CompleteMission(string missonKey)
    {
        // remove from dict
        missionDict.Remove(missonKey);
        // remove from UI
        UIController.instance.RemoveMissionFromPanel(missonKey);
        // check complete main mission
        if (missonKey == "MAIN_MISSION")
        {
            CompleteMainMission();
        }
        // check if need change silence flag
        isSilenceRequireMain = CheckIsMissionsRequireSilence();
    }

    public void CompleteMainMission()
    {
        isGameComplete = true;
    }
    // return true if at least one of mission need silence
    private bool CheckIsMissionsRequireSilence()
    {
        // check all mission
        foreach (KeyValuePair<string, Mission> missionPair in missionDict)
        {
            if (missionPair.Value.IsRequireSilence)
            {
                return true;
            }
            
        }
        return false;
    }
}
