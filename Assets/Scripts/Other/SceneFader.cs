﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour {

    public Image image;
    public AnimationCurve fadeCurve;

    void Start() {
        StartCoroutine(FadeIn());
    }

    public void FadeTo(string scene) {
        StartCoroutine(FadeOut(scene));
    }

    IEnumerator FadeIn() {
        float t = 1.0f;

        while(t > 0.0f) {
            t -= Time.deltaTime;
            float a = fadeCurve.Evaluate(t);
            image.color = new Color(0, 0, 0, a);
            yield return 0; //skip frame
        }
    }

    IEnumerator FadeOut(string scene) {
        float t = 0.0f;

        while(t < 1.0f) {
            t += Time.deltaTime;
            float a = fadeCurve.Evaluate(t);
            image.color = new Color(0, 0, 0, a);
            yield return 0; //skip frame
        }

        SceneManager.LoadScene(scene);
    }
}