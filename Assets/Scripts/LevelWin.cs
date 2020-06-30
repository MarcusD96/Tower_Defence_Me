using UnityEngine;

public class LevelWin : MonoBehaviour {
    public SceneFader sceneFader;
    public string menuSceneName = "Main Menu";
    public int levelToUnlock = 2;

    public void Menu() {
        sceneFader.FadeTo(menuSceneName);
    }

    public void Continue() {
        PlayerPrefs.SetInt("levelReached", levelToUnlock);
        sceneFader.FadeTo("Level" + levelToUnlock);
    }
}
