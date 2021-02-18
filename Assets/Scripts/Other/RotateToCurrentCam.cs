
using UnityEngine;

public class RotateToCurrentCam : MonoBehaviour {

    void LateUpdate() {
        transform.rotation = CameraManager.GetCurrentCam().transform.rotation;
    }
}
