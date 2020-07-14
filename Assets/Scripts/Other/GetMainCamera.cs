using UnityEngine;

public class GetMainCamera : MonoBehaviour {
    public Canvas canvas;

    void Start() {
        canvas.worldCamera = Camera.main;
    }
}
