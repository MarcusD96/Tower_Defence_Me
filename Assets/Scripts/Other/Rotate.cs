
using UnityEngine;

public class Rotate : MonoBehaviour {

    private float speed = 50;
    
    private void Update() {
        transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
    }
}
