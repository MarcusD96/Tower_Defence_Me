
using UnityEngine;

public class RotateHPBar : MonoBehaviour {

    void LateUpdate() {
        transform.rotation = CameraManager.GetCurrentCam().transform.rotation;
    }
}
