
using System.Collections;
using UnityEngine;

public class SlowWave : MonoBehaviour {
    public float growSpeed, slowFactor, slowDuration;
    public Vector3 maxSize;
    [HideInInspector]
    public bool done = false;

    [SerializeField]
    private float damagerPerSecond;


    void Start() {
        StartCoroutine(WaveBlast());
    }

    void OnTriggerEnter(Collider other) {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if(enemy) {
            enemy.superSlow = true;
            enemy.Slow(slowFactor, slowDuration);
            enemy.DamageOverTime(damagerPerSecond, slowDuration);
        }
    }

    IEnumerator WaveBlast() {
        AudioManager.StaticPlayEffect(AudioManager.instance.sounds, "EMP", transform.position);
        while(transform.localScale.x < maxSize.x - 5) {
            var scale = transform.localScale;
            scale = Vector3.Slerp(scale, maxSize, Time.deltaTime * growSpeed);
            scale.y = 0.01f;
            transform.localScale = scale;

            yield return null;
        }
        done = true;
    }
}
