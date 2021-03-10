
using UnityEngine;

public class CameraShake : MonoBehaviour {

    // How long the object should shake for.
    public float shakeDuration;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount;
    public float decreaseFactor;

    Vector3 originalPos;

    private void Start() {
        originalPos = transform.localPosition;
    }

    void Update() {
        if(shakeDuration > 0) {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        } else {
            shakeDuration = 0f;
            transform.localPosition = originalPos;
        }
    }
}
