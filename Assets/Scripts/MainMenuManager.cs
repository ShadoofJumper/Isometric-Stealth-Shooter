using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TextMeshProUGUI smile;
    [SerializeField] private Slider slider;
    [SerializeField] private SceneFader sceneFader;
    private string sceneOnPlayName = "LevelSelect";
    // Start is called before the first frame update
    void Start()
    {
        smile.alpha = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    // ------------- main menu
    public void StartGame()
    {
        // TO DO load last scene, not first
        sceneFader.FadeTo(sceneOnPlayName);
        // resume game if it was on pause
        //GameManager.instance.RestartLevel();
    }

    public void TurnSettings()
    {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    // ------------- settings

    public void BackToMenu()
    {
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void SliderChange()
    {
        smile.alpha = slider.value;
    }


}
