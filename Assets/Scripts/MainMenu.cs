using UnityEngine;

public class MainMenu : MonoBehaviour {

    public string levelToLoad = "Level1";
    public SceneFader sceneFader;

    public void Play() {
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
