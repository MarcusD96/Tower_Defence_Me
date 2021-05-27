using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {
    SceneFader fader;

    private void Awake() {
        fader = FindObjectOfType<SceneFader>();
        StartCoroutine(LoadNextLevel(PlayerStats.levelToLoad));
    }

    IEnumerator LoadNextLevel(string scene) {
        yield return new WaitForSeconds(3.0f);
        AsyncOperation op = SceneManager.LoadSceneAsync(scene);
        while(!op.isDone) {
            yield return null;
        }
    }
}
