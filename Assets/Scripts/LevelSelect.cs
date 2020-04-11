using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{

    [SerializeField] private Image previewSceneTemp;
    [SerializeField] private Button firstSceneButton;
    [SerializeField] private SceneFader sceneFader;
    [SerializeField] private List<Sprite> scenePreview  = new List<Sprite>();
    [SerializeField] private List<string> sceneNames    = new List<string>();
    private string sceneSelected;

    private void Start()
    {
        // set on start first button as selected
        firstSceneButton.Select();
        SelectScene(0);
    }

    public void LoadScene()
    {
        sceneFader.FadeTo(sceneSelected);
    }

    public void SelectScene(int sceneId)
    {
        // save selected scene
        sceneSelected = sceneNames[sceneId];
        // update temp img
        previewSceneTemp.sprite = scenePreview[sceneId];

        
    }

}
