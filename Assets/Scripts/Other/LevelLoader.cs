using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {
    public bool stay;
    public TextMeshProUGUI tip;

    private void Awake() {
        StartCoroutine(LoadNextLevel(PlayerStats.levelToLoad));
        InitializeTips();
        ChooseRandomTip();
        Time.timeScale = 1;
    }

    void InitializeTips() {
        string dataPath;
#if UNITY_EDITOR
        dataPath = "Assets/StreamingAssets/";
#else
        dataPath = Application.streamingAssetsPath;
        if(!Directory.Exists(dataPath)) {
            Directory.CreateDirectory(dataPath);
        }
#endif
        tips = File.ReadAllLines(System.IO.Path.Combine(dataPath, "tips.txt"));
    }

    string[] tips;
    void ChooseRandomTip() {
        int r = Random.Range(0, tips.Length);
        tip.text = "Tip: " + tips[r];
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
