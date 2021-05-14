using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public static bool paused;

    public GameObject ui, settings;
    private SceneFader sceneFader;
    private float timeScale_prev;
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
            timeScale_prev = Time.timeScale;
            AudioManager.StaticStopAllSounds();
            Time.timeScale = 0;
        } else {            //un paused
            Time.timeScale = timeScale_prev;
            settings.SetActive(false);
        }
    }

    public void OpenSettings() {
        settings.SetActive(true);
        settings.GetComponent<SettingsMenu>().InitialUpdate();
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
