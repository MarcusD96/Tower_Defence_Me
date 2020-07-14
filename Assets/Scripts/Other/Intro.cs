using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Intro : MonoBehaviour {
    public RawImage target;
    public VideoPlayer player;
    public SceneFader sceneFader;

    void Start() {
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro() {
        player.Prepare();
        while(!player.isPrepared) {
            yield return new WaitForSeconds(1.0f);
        }
        target.color = Color.white;
        target.texture = player.texture;
        player.Play();
        while(player.isPlaying) {
            yield return new WaitForSeconds(1.0f);
        }
        sceneFader.FadeTo("Main Menu");
    }
}
