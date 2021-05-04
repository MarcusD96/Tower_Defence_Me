using System.Collections;
using UnityEngine;

public class FPSCounter : MonoBehaviour {

    string label = "";
    float count;

    IEnumerator Start() {
        DontDestroyOnLoad(this);
        GUI.depth = 2;
        while(true) {
            if(Time.timeScale >= 1) {
                yield return new WaitForSeconds(0.1f);
                count = (1 / Time.unscaledDeltaTime);
                label = "FPS :" + (Mathf.Round(count));
            }
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    void OnGUI() {
        GUI.Label(new Rect(5, 40, 100, 25), label);
    }
}