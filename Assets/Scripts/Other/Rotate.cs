
using UnityEngine;

public class Rotate : MonoBehaviour {

    public float speed = 50;
    
    private void Update() {
        transform.Rotate(Vector3.up * speed * Time.deltaTime * Time.timeScale, Space.World);
    }
}
