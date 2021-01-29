using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public static Camera cam;

    // Start is called before the first frame update
    void Awake() {
        cam = Camera.main;
    }
    public static void UpdateCam(Camera newCam) {
        cam = newCam;
    }

    public static Camera GetCurrentCam() {
        if(cam == null) {
            return Camera.main;
        }
        return cam;
    }
}
