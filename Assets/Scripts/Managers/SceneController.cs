using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    #region Singlton

    public static SceneController instance;


    private void Awake()
    {
        Debug.Log("Awake SceneController");
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

    // enemy origin
    [SerializeField] private Character enemyOrigin;
    // character params
    public Character player;
    public CharacterInventory playerInventory;
    [SerializeField] private CharacterSettings playerSettings;
    [SerializeField] private Transform startPositionPlayer;
    public List<Character> Enemys = new List<Character>();
    public Dictionary<Transform, Character> charactersOnScene = new Dictionary<Transform, Character>();
    public Dictionary<Transform, LevelCamera> camerasOnScene = new Dictionary<Transform, LevelCamera>();
    [SerializeField] private GameObject doorsParent;
    [SerializeField] private GameObject targetFolder;

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
        if (!targetFolder.activeSelf)
        {
            Debug.Log("No target folder");
            return null;
        }

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

    // FOR TEST
    private bool isSlow = false;
    private bool isFast = false;
    private void Update()
    {
        // kill all bots
        if (Input.GetKey(KeyCode.K))
        {
            StartCoroutine("DieDelay");
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            Time.timeScale = !isFast ? 2.0f : 1.0f;
            isFast = !isFast;
        }
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            Time.timeScale = !isSlow ? 0.5f : 1.0f;
            isSlow = !isSlow;
        }

    }

    IEnumerator DieDelay()
    {
        yield return new WaitForSeconds(1.0f);

        foreach (Character enemy in Enemys)
        {
            enemy.characterCombat.Die();
        }
    }


    // ----------------------------- Dev -----------------------------
    // paint gizmos if ai
    private void OnDrawGizmos()
    {
        // paint start point
        Gizmos.DrawSphere(startPositionPlayer.position, 0.6f);

    }

}
