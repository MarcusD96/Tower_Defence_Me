using UnityEngine;

public class CameraController : MonoBehaviour {
    public static bool isEnabled;

    public float panSpeed = 30.0f, panBorderThickness = 50.0f, scrollSpeed = 5.0f;

    private float minY = 20.0f, maxY = 80.0f;

    void Start() {
        isEnabled = true;
    }

    // Update is called once per frame
    void Update() {
        if(!isEnabled) {
            return;
        }

        if(GameManager.gameEnd) {
            this.enabled = false; //technically not required, but to be explicit
            return;
        }

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
        }
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World);
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 pos = transform.position;
        pos.y -= scroll * 1000 * scrollSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;

    }
}
