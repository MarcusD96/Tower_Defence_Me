using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {
    private SceneFader sceneFader;
    public string menuSceneName = "Main Menu";

    void Start() {
        sceneFader = FindObjectOfType<SceneFader>();
    }

    public void Retry() {
        sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }
    
    public void Quit() {
        sceneFader.FadeTo(menuSceneName);
    }
}
