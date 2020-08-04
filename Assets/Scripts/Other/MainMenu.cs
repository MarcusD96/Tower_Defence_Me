using UnityEngine;

public class MainMenu : MonoBehaviour {
    public string levelToLoad = "Level Selection";
    public string survivalLevelToLoad = "Survival";
    public SceneFader sceneFader;

    public void Play() {
        GameMode.survival = false;
        sceneFader.FadeTo(levelToLoad);
    }

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}

public static class GameMode {
    public static bool survival = false;
}
