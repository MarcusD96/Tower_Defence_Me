using UnityEngine;

public class LevelWin : MonoBehaviour {
    private SceneFader sceneFader;
    private readonly string menuSceneName = "Main Menu";
    private int levelToUnlock;

    void Awake() {
        sceneFader = FindObjectOfType<SceneFader>();
        levelToUnlock = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 2;
    }

    public void Menu() {
        sceneFader.FadeTo(menuSceneName);
    }

    public void Continue() {
        PlayerPrefs.SetInt("levelReached", levelToUnlock);
        PlayerPrefs.Save();
        sceneFader.FadeTo("Level Selection");
    }
}
