using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour {

    public float damage, fallTime;
    public Color color;
    public TextMeshProUGUI damageText;

    void Start() {
        IndicateDamage(damage, color);
        Destroy(gameObject, fallTime);
    }

    void Update() {
        transform.LookAt(CameraManager.GetCurrentCam().transform, Vector3.up);

        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, pos.y + 7, Time.deltaTime);
        transform.position = pos;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Lerp(scale.x, 0, Time.deltaTime);
        scale.y = scale.z = scale.x;
        transform.localScale = scale;

        float a = damageText.color.a;
        a = Mathf.Lerp(a, 0, Time.deltaTime);
        Color aa = new Color(damageText.color.r, damageText.color.g, damageText.color.b, a);
        damageText.color = aa;
    }
    
    public void IndicateDamage(float damage_, Color color) {
        damageText.text = Mathf.CeilToInt(damage_).ToString();
        damageText.color = color;
    }
}
