using UnityEngine;

public class OrbitCamera : MonoBehaviour {

    public float orbitSpeed = 3;

    // Update is called once per frame
    void Update() {
        gameObject.transform.Rotate(Vector3.up, Time.deltaTime * orbitSpeed);
    }
}
