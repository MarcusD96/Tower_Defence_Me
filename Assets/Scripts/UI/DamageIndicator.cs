using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour {

    public float damage, fallTime = 0.5f;
    public TextMeshProUGUI damageText;

    void Start() {
        IndicateDamage(damage);
        Destroy(gameObject, 0.5f);
    }

    void Update() {
        transform.LookAt(Camera.main.transform);
        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, pos.y + 3, Time.deltaTime * 2);
        transform.position = pos;
    }

    public void IndicateDamage(float damage_) {
        damageText.text = damage_.ToString();
    }
}
