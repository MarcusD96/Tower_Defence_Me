using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public GameObject ui;

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit() {
        Toggle();
        SceneManager.LoadScene("Main Menu");
    }
}
