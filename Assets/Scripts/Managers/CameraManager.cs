
using UnityEngine;

public class CameraManager {

    public static Camera cam = Camera.main;

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
