using System.Collections;
using UnityEngine;
using TMPro;

public class DamageIndicator : MonoBehaviour {

    public float fallTime = 0.5f, distance = 3;
    public TextMeshPro damageText;

    void Update() {
        transform.LookAt(Camera.main.transform);
    }

    public void IndicateDamage(float damage) {
        damageText.text = damage.ToString();
        var endTime = Time.time + fallTime;
        while(Time.time < endTime) {
            var position = transform.position;
            position.y = Mathf.Lerp(position.y, position.y - distance, fallTime);
        }
        Destroy(gameObject);
    }
}
