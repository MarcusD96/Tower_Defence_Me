using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {
    public SceneFader sceneFader;
    public string menuSceneName = "Main Menu";

    public void Retry() {
        sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }
    
    public void Quit() {
        sceneFader.FadeTo(menuSceneName);
    }
}
