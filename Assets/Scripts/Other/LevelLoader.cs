using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {
    public bool stay;

    private void Awake() {
        StartCoroutine(LoadNextLevel(PlayerStats.levelToLoad));
    }

    IEnumerator LoadNextLevel(string scene) {
        if(stay)
            yield break;

        AsyncOperation op = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        while(op.isDone) {
            yield return null;
        }
        Camera.main.gameObject.GetComponent<AudioListener>().enabled = false;
        yield return new WaitForSeconds(0.01f);
        var pool = FindObjectOfType<ObjectPool>(true);
        while(!pool.CheckLoading())
            yield return null;
        SceneManager.UnloadSceneAsync("Loading");
    }
}
