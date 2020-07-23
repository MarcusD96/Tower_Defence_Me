using UnityEngine;
using TMPro;

public class DamageIndicator : MonoBehaviour {

    public float damage, fallTime = 0.5f, distance = 3;
    public TextMeshPro damageText;

    void Start() {
        IndicateDamage(damage);
    }

    void Update() {
        transform.LookAt(Camera.main.transform);
    }

    public void IndicateDamage(float damage_) {
        damageText.text = damage_.ToString();
    }
}
