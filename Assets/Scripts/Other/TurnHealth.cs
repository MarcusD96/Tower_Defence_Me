
using UnityEngine;

public class TurnHealth : MonoBehaviour {
    public static Camera cam;

    void Start() {
        cam = Camera.main;
    }

    void Update() {
        transform.rotation = cam.transform.rotation;
    }

    public static void UpdateCam(Camera newCam) {
        if(cam) {
            Debug.Log("Old: " + cam.name);
        }
        cam = newCam;
        Debug.Log("New: " + newCam.name + " || " + "Set: " + cam.name);
    }
}
