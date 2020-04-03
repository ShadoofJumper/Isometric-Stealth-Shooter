using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    #region Singlton

    public static UIController instance;

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

    [Tooltip("Текстовые объекты для UI сверху экрана")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI hintText;

    [Tooltip("Текстовые объекты для UI связанык с миссией")]
    [SerializeField] private RectTransform MissionPanel;
    [SerializeField] private RectTransform MissionBlockOrigin;

    [Tooltip("Панель пауз скрина")]
    [SerializeField] private GameObject pausePanel;
    [Tooltip("Кнопка паузы")]
    [SerializeField] private GameObject buttonSettings;
    [Tooltip("Панель предупреждения")]
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private TextMeshProUGUI WarningDescription;
    [SerializeField] private Button warningButton;

    private CharacterCombat characterCombat;
    private HintType currentHintType;
    private Dictionary<string, RectTransform> missionBlocksDict = new Dictionary<string, RectTransform>();

    public enum HintType
    {
        nothing,
        ammo,
        mission
    }


    // Start is called before the first frame update
    void Start()
    {
        characterCombat = SceneController.instance.player.characterCombat;
    }

    public void UpdateHealthUI(float health)
    {
        healthText.text = health.ToString();
    }

    public void UpdateAmmoUI(float ammoInStoke, float ammoAmount)
    {
        ammoText.text = $"{ammoInStoke}/{ammoAmount}";
    }

    //------------------------------ Hint ------------------------------//
    public void ShowAmmoHint()
    {
        SetHintText("Press R for reload");
        currentHintType = HintType.ammo;
    }
    public void HideAmmoHint()
    {
        // if this hint show now, then hide
        if (currentHintType == HintType.ammo)
        {
            currentHintType = HintType.nothing;
            ResetHintText();
        }
    }
    private void SetHintText(string text)
    {
        hintText.text = text;
    }
    private void ResetHintText()
    {
        hintText.text = "";
    }
    //------------------------------ Mission ------------------------------//

    public void AddMissionToPanel(Mission mission)
    {
        //get text
        string text = mission.Text;

        // add new block to mission panel
        RectTransform newMissionBlock = Instantiate(MissionBlockOrigin);
        // add block to dict
        missionBlocksDict.Add(mission.Key, newMissionBlock);
        // seet text
        newMissionBlock.GetComponentInChildren<TextMeshProUGUI>().text = text;
        newMissionBlock.SetParent(MissionPanel);
    }

    public void RemoveMissionFromPanel(string missionKey)
    {
        // found block to remove
        RectTransform missionBlock = missionBlocksDict[missionKey];
        Destroy(missionBlock.gameObject);
        missionBlocksDict.Remove(missionKey);
    }

    //------------------------------ Pause menu ------------------------------//

    public void PauseGame()
    {
        GameManager.instance.PauseGameLogic();
        // disable setting button and enable pause menu
        pausePanel.SetActive(true);
        buttonSettings.SetActive(false);
    }

    public void ResumeGame()
    {
        GameManager.instance.ResumeGameLogic();
        pausePanel.SetActive(false);
        buttonSettings.SetActive(true);
    }

    public void ReturnMenu()
    {
        GameManager.instance.ResumeGameLogic();
        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    //------------------------------ Warning panel ------------------------------//
    //    // action for warning button, so can delegate function to it
    //private UnityAction onButtonWarningClick;
    public void ShowWarning(string message, bool showButton = false, string buttonText = "", UnityAction onWarningButtonClick = null, string messageDescib = "")
    {
        warningPanel.SetActive(true);
        warningText.text = message;
        WarningDescription.text = messageDescib;
        if (showButton)
        {
            warningButton.gameObject.SetActive(true);
            TextMeshProUGUI buttonTextMesh = warningButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonTextMesh.text = buttonText;
            if (onWarningButtonClick != null)
            {
                warningButton.onClick.AddListener(onWarningButtonClick);
            }
        }
    }

    public void HideWarning()
    {
        warningPanel.SetActive(false);
        warningText.text = "";
        warningButton.onClick.RemoveAllListeners();
    }



}
