using UnityEngine;

public class GetMainCamera : MonoBehaviour {
    public Canvas canvas;

    void Awake() {
        canvas.worldCamera = Camera.main;
        canvas.sortingLayerName = "Camera";
        InvokeRepeating("CheckCamera", 0, 0.25f);
    }

    void CheckCamera() {
        if(canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;
    }
}
