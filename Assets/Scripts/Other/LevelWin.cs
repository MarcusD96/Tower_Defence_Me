using UnityEngine;

public class LevelWin : MonoBehaviour {
    private SceneFader sceneFader;
    private string menuSceneName = "Main Menu";
    public int levelToUnlock;

    void Awake() {
        sceneFader = FindObjectOfType<SceneFader>();
    }

    public void Menu() {
        sceneFader.FadeTo(menuSceneName);
    }

    public void Continue() {
        PlayerPrefs.SetInt("levelReached", levelToUnlock);
        Debug.Log(levelToUnlock);
        sceneFader.FadeTo("Level Selection");
    }
}
