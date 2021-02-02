using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public static bool paused;

    public GameObject ui;
    private SceneFader sceneFader;
    private string menuSceneName = "Main Menu";


    void Awake() {
        sceneFader = FindObjectOfType<SceneFader>();
        paused = false;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Toggle();
        }
    }

    public void Toggle() {
        ui.SetActive(!ui.activeSelf);
        paused = ui.activeSelf;
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

    public void MainMenu() {
        Toggle();
        sceneFader.FadeTo(menuSceneName);
    }
}
