using UnityEngine;

public class CameraController : MonoBehaviour {
    public static bool isEnabled;

    public float panSpeed = 50.0f, scrollSpeed = 5.0f;

    private float minX = -20.0f, maxX = 20.0f;
    private float minY = 40.0f, maxY = 100.0f;
    private float minZ = -45.0f, maxZ = 10.0f;

    void Start() {
        isEnabled = true;
    }

    // Update is called once per frame
    void Update() {
        if(GameManager.gameEnd) {
            this.enabled = false; //'this' technically not required, but to be explicit
            return;
        }

        if(!isEnabled) {
            return;
        }

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            transform.Translate(Vector3.forward * panSpeed * Time.fixedDeltaTime * Time.timeScale, Space.World);
        }
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            transform.Translate(Vector3.back * panSpeed * Time.fixedDeltaTime * Time.timeScale, Space.World);
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            transform.Translate(Vector3.left * panSpeed * Time.fixedDeltaTime * Time.timeScale, Space.World);
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            transform.Translate(Vector3.right * panSpeed * Time.fixedDeltaTime * Time.timeScale, Space.World);
        }


        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 pos = transform.position;
        pos.y -= scroll * 1000 * scrollSpeed * Time.fixedDeltaTime * Time.timeScale;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }
}
