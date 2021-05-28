
using UnityEngine;

[RequireComponent(typeof(Turret))]
public class TurretMovement : MonoBehaviour {

    Turret turret;
    public Transform turretCam;

    private void Awake() {
        turret = GetComponent<Turret>();
    }

    private void Update() {
        if(turret.manual) {
            ManualMovement();
        }
    }

    void ManualMovement() {
        Vector3 lookHere = Vector3.zero;        

        if(Settings.UseKeys) {
            if(Input.GetKey(KeyCode.A)) {
                lookHere = Vector3.down * Time.unscaledDeltaTime * Settings.Sensitivity;
                turret.pivot.Rotate(lookHere);
            }
            if(Input.GetKey(KeyCode.D)) {
                lookHere = Vector3.up * Time.unscaledDeltaTime * Settings.Sensitivity;
                turret.pivot.Rotate(lookHere);
            }
        }
        
        else {
            float mouseInput = Input.GetAxisRaw("Mouse X");
            lookHere = Vector3.up * mouseInput * Time.unscaledDeltaTime * Settings.Sensitivity;
            turret.pivot.Rotate(lookHere); 
        }

        SwayCamera(lookHere);
    }

    float angle = 0.5f;
    void SwayCamera(Vector3 dir) {
        float dt = Time.unscaledDeltaTime * 5;

        Vector3 v = turretCam.localPosition;

        if(dir.y > 0) {
            //move camera left
            v.x = Mathf.Lerp(v.x, -angle, dt);

        } else if(dir.y < 0) {
            //move camera right
            v.x = Mathf.Lerp(v.x, angle, dt);

        } else {
            //return to centre
            v.x = Mathf.Lerp(v.x, 0, dt);
        }

        v.x = Mathf.Clamp(v.x, -angle, angle);

        turretCam.localPosition = v;
    }
}
