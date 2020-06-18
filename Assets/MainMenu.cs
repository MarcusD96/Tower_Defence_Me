using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public string levelToLoad = "Level1";

    public void Play() {
        SceneManager.LoadScene(levelToLoad);
    }

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif

    }
}
