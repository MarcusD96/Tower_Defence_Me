using UnityEngine;

public class GetMainCamera : MonoBehaviour {
    public Canvas canvas;

    void Awake() {
        canvas.worldCamera = Camera.main;
    }
}
