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
        if(ui.activeSelf) { //paused
            Time.timeScale = 0;
            foreach(var o in FindObjectsOfType<Outline>()) {
                AudioManager.StopAllSounds();
                o.enabled = false;
            }
        } else {            //un paused
            Time.timeScale = 1;
            foreach(var o in FindObjectsOfType<Outline>()) {
                o.enabled = true;
            }
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
