using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour {

    public Image image;
    public AnimationCurve fadeCurve;

    void Start() {
        StartCoroutine(FadeIn());
    }

    public void FadeTo(string scene) {
        StartCoroutine(FadeOut(scene));
    }

    public void FadeToLevel() {
        StartCoroutine(FadeOut(PlayerStats.nextLevel));
    }

    IEnumerator FadeIn() {
        float t = 1.0f;

        while(t > 0.0f) {
            t -= Time.unscaledDeltaTime;
            float a = fadeCurve.Evaluate(t);
            image.color = new Color(0, 0, 0, a);
            yield return 0; //skip frame
        }
    }

    IEnumerator FadeOut(string scene) {
        float t = 0.0f;

        while(t < 1.0f) {
            t += Time.unscaledDeltaTime;
            float a = fadeCurve.Evaluate(t);
            image.color = new Color(0, 0, 0, a);
            yield return 0; //skip frame
        }

        SceneManager.LoadScene(scene);
    }
}
