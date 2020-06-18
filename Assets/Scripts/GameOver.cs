using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {
    public TextMeshProUGUI roundsText;
    public SceneFader sceneFader;
    public string menuSceneName = "Main Menu";

    void OnEnable() {
        roundsText.text = PlayerStats.rounds.ToString();
    }

    public void Retry() {
        sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }
    
    public void Quit() {
        sceneFader.FadeTo(menuSceneName);
    }
}
