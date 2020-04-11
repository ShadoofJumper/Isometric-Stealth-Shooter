using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    #region Singlton

    public static SceneController instance;


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

    // enemys settings
    private List<CharacterSettings> enemySettings;

    // target parent
    [SerializeField] private GameObject targetFolder;
    // enemy origin
    [SerializeField] private Character enemyOrigin;
    // character params
    public Character player;
    [SerializeField] private CharacterSettings playerSettings;
    [SerializeField] private Transform startPositionPlayer;
    // enemys list
    public List<Character> Enemys = new List<Character>();
    // all characters dict, for state all getcomponent info
    public Dictionary<Transform, Character> charactersOnScene = new Dictionary<Transform, Character>();
    // all cameras dict, for state all getcomponent info
    public Dictionary<Transform, LevelCamera> camerasOnScene = new Dictionary<Transform, LevelCamera>();
    // doors parent
    [SerializeField] private GameObject doorsParent;


    // propertys
    public List<CharacterSettings> EnemySettings    { get { return enemySettings; }         set { enemySettings         = value; } }
    public Transform StartPositionPlayer            { get { return startPositionPlayer; }   set { startPositionPlayer   = value; } }
    public GameObject DoorsParent                    { get { return doorsParent; }           set { doorsParent           = value; } }


    public void SpawnAllEnemys()
    {
        // spawn all enemys
        for (int enemyId = 0; enemyId < enemySettings.Count; enemyId++)
        {
            Character enemy = SpawnEnemy(enemyId);
            Enemys.Add(enemy);
        }
    }

    private Character SpawnEnemy(int enemyId)
    {
        if (enemySettings == null)
        {
            Debug.Log("No enemy settings in scene controller!");
            return null;
        }
        // create game object
        Character enemy = Instantiate(enemyOrigin, targetFolder.transform);
        // set setting to game object
        enemy.InitializeCharacter(enemySettings[enemyId]);
        return enemy;
    }

    public void SpawnPlayer()
    {
        if(playerSettings == null)
        {
            Debug.Log("No player settings in scene controller!");
            return;
        }
        player.InitializeCharacter(playerSettings);
        player.characterMover.SetStartPosition(startPositionPlayer.position);
    }

    public void RemoveAllEnemys()
    {
        foreach (Character enemy in Enemys)
        {
            Destroy(enemy.gameObject);
        }
        Enemys.Clear();
    }


    // ----------------------------- Dev -----------------------------
    // paint gizmos if ai
    private void OnDrawGizmos()
    {
        // paint start point
        Gizmos.DrawSphere(startPositionPlayer.position, 0.6f);

    }

}
