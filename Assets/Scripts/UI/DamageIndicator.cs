using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour {

    public float damage, fallTime;
    public TextMeshProUGUI damageText;

    void Start() {
        IndicateDamage(damage);
        Destroy(gameObject, fallTime);
    }

    void Update() {
        transform.LookAt(CameraManager.GetCurrentCam().transform, Vector3.up);
        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, pos.y + 5, time.deltaTime);
        transform.position = pos;
    }
    
    public void IndicateDamage(float damage_) {
        damageText.text = damage_.ToString();
    }
}
