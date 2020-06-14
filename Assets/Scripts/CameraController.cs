using UnityEngine;

public class CameraController : MonoBehaviour {

    public float panSpeed = 30.0f, panBorderThickness = 50.0f, scrollSpeed = 5.0f;

    private bool doMovement = true;
    private float minY = 20.0f, maxY = 80.0f;

    // Update is called once per frame
    void Update() {

        if(Input.GetKeyDown(KeyCode.Escape))
            doMovement = !doMovement;

        if(!doMovement)
            return;

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.mousePosition.y >= Screen.height - panBorderThickness) {
            transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
        }
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.mousePosition.y <= panBorderThickness) {
            transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World);
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.mousePosition.x <= panBorderThickness) {
            transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.mousePosition.x >= Screen.width - panBorderThickness) {
            transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 pos = transform.position;
        pos.y -= scroll * 1000 * scrollSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;

    }
}
