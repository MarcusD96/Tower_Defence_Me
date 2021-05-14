
using UnityEngine;

public class Menu_Shotgun : MonoBehaviour {
    public float speed;
    public GameObject[] pellets;

    private Vector3[] directions;

    private void Start() {
        Destroy(gameObject, 2.0f);
        directions = new Vector3[pellets.Length];
        for(int i = 0; i < directions.Length; i++) {
            directions[i] = RandomDirection();
        }
    }

    private void Update() {
        for(int i = 0; i < pellets.Length; i++) {
            pellets[i].transform.Translate(directions[i] * speed  * Time.deltaTime, Space.World);
        }
    }

    Vector3 RandomDirection() {
        float r = Random.Range(-0.5f, 0.5f);
        Vector3 direction = new Vector3(Vector3.up.x + r, Vector3.up.y, Vector3.up.z);
        direction.Normalize();
        var g = GameObject.Find("Head");
        var localForward = g.transform.rotation * direction;
        return localForward;
    }

}
