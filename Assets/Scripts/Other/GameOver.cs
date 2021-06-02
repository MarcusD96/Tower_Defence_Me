using UnityEngine;

public class GameOver : MonoBehaviour {
    private SceneFader sceneFader;
    string menuSceneName = "Main Menu";

    void Start() {
        sceneFader = FindObjectOfType<SceneFader>();
    }

    public void Retry() {
        PlayerStats.levelToLoad = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        sceneFader.FadeTo("Loading");
    }

    public void Quit() {
        sceneFader.FadeTo(menuSceneName);
    }
}
