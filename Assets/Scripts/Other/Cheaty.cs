using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Cheaty : MonoBehaviour {
    public static bool play;

    public RawImage image;
    public VideoPlayer vp;

    void Start() {
        play = false;
    }

    void Update() {
        if(play) {
            StartCoroutine(PlayVideo());
            play = false;
        }
    }
    IEnumerator PlayVideo() {
        vp.Prepare();
        while(!vp.isPrepared) {
            yield return new WaitForSeconds(1);
        }
        image.texture = vp.texture;
        image.gameObject.SetActive(true);
        vp.Play();

        while(vp.isPlaying) {
            yield return new WaitForSeconds(1);
        }
        Destroy(gameObject);
        Destroy(image.gameObject);
    }
}
