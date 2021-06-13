
using System.Collections;
using UnityEngine;

public class LaserWave : MonoBehaviour {
    public float growSpeed;
    public ParticleSystem ps;
    public ParticleSystem.ShapeModule aura;

    private float duration = 0, slowFactor, maxSize;
    private Transform child;
    private LaserTurret owner;
    private SphereCollider sCollider;

    public void Initialize(float slowFactor_, float maxSize_, float duration_, LaserTurret owner_) {
        duration = duration_;
        maxSize = maxSize_;
        slowFactor = slowFactor_;
        owner = owner_;
        child = transform.GetChild(0);
        sCollider = GetComponent<SphereCollider>();
        aura = GetComponentInChildren<ParticleSystem>().shape;
        StartCoroutine(Animate());
    }

    private void LateUpdate() {
        transform.Rotate(Vector3.up, Time.deltaTime * 10);
    }

    Enemy enemy;
    void OnTriggerStay(Collider other) {
        enemy = other.gameObject.GetComponent<Enemy>();
        if(enemy) {
            enemy.SlowAura(slowFactor);
        }
    }

    private void OnTriggerExit(Collider other) {
        enemy = other.gameObject.GetComponent<Enemy>();
        if(enemy) {
            enemy.RestoreSlow();
        }
    }

    
    IEnumerator Animate() {
        float endTime = Time.time + duration;
        AudioManager.StaticPlayEffect(AudioManager.instance.sounds, "Slow Aura", transform.position);

        while(child.localScale.x < maxSize) {
            //grow
            child.localScale += Vector3.one * Time.deltaTime * growSpeed;
            var scale = child.localScale;
            child.localScale = scale;
            sCollider.radius = aura.radius = scale.x / 2;
            yield return null;
        }
        while(Time.time < endTime) {
            //ocsillate bigger and smaller
            yield return null;
        }
        while(child.localScale.x > 0) {
            //shrink
            child.localScale -= Vector3.one * Time.deltaTime * growSpeed * 10;
            var scale = child.localScale;
            child.localScale = scale;
            sCollider.radius = aura.radius = scale.x / 2;
            yield return null;
        }
        Destroy(gameObject);
    }
}
