using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Intro : MonoBehaviour {
    public RawImage target;
    public VideoPlayer player;
    public SceneFader sceneFader;

    void Start() {
#if UNITY_EDITOR
        PlayerPrefs.DeleteAll(); //no saving in editor mode
#endif
        Application.targetFrameRate = 150;
        StartCoroutine(PlayIntro());
    }

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            StopAllCoroutines();
            sceneFader.FadeTo("Main Menu");
            AudioManager.Main = true;
        }
    }

    IEnumerator PlayIntro() {
        player.Prepare();
        while(!player.isPrepared) {
            yield return new WaitForSeconds(1.0f);
        }
        target.color = Color.white;
        target.texture = player.texture;
        player.Play();
        AudioManager.StaticPlay("Intro", Vector3.zero);
        while(player.isPlaying) {
            yield return new WaitForSeconds(1.0f);
        }
        sceneFader.FadeTo("Main Menu");
        AudioManager.Main = true;

    }
}
