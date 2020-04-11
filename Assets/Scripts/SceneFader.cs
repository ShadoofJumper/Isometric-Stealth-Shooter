using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public Image img;
    public AnimationCurve fadeCurve;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeTo(string scene, float speed = 1.0f,  UnityAction afterFade = null)
    {
        StartCoroutine(FadeOut(scene, speed, afterFade));
    }

    IEnumerator FadeIn()
    {
        float t = 1f;
        while (t > 0)
        {
            t -= Time.deltaTime;
            float a = fadeCurve.Evaluate(t);
            img.color = new Color(0, 0, 0, a);
            yield return 0;
        }
    }

    IEnumerator FadeOut(string sceneToLoad, float speed, UnityAction afterFade)
    {
        float t = 0f;
        while (t < 1f)
        {

            t += Time.deltaTime * speed;

            float a = fadeCurve.Evaluate(t);
            img.color = new Color(0, 0, 0, a);
            yield return 0;
        }
        // action after fade to scene
        if (afterFade != null)
        {
            afterFade.Invoke();
        }
        SceneManager.LoadScene(sceneToLoad);
    }

}
