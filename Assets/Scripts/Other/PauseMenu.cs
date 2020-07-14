using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public GameObject ui;
    public SceneFader sceneFader;
    public string menuSceneName = "Main Menu";

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Toggle();
        }
    }

    public void Toggle() {
        ui.SetActive(!ui.activeSelf);
        if(ui.activeSelf) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }

    public void Restart() {
        Toggle();
        sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }

    public void Quit() {
        Toggle();
        sceneFader.FadeTo(menuSceneName);
    }
}
