
using UnityEngine;

public class MenuProjectile : MonoBehaviour {

    public float speed;

    private void Update() {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }
}
