using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Settings", fileName = "Level setting")]
public class LevelSettings : ScriptableObject
{
    [SerializeField] private List<CharacterSettings> enemySettings;
    [SerializeField] private List<string> startMissionsTasks;
    [SerializeField] private List<bool> startMissionsIsSilance;

    public List<CharacterSettings> EnemySettings    { get { return enemySettings; } }
    public List<string> StartMissionsTasks          { get { return startMissionsTasks; } }
    public List<bool>   StartMissionsIsSilance      { get { return startMissionsIsSilance; } }

    //struct to save info about edges in our field of view
    public struct MissionStruct
    {
        public string missionTask;
        public bool isSilence;

        public MissionStruct(string _missionTask, bool _isSilence)
        {
            missionTask = _missionTask;
            isSilence = _isSilence;
        }

    }
}
