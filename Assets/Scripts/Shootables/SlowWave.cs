
using System.Collections;
using UnityEngine;

public class SlowWave : MonoBehaviour {
    public float speed, slowFactor, slowDuration;
    [HideInInspector]
    public float damageOverTime;
    public Vector3 maxSize;
    [HideInInspector]
    public bool done = false;


    void Start() {
        StartCoroutine(WaveBlast());
    }

    void OnTriggerEnter(Collider other) {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if(enemy) {
            enemy.superSlow = true;
            enemy.Slow(slowFactor, slowDuration);
            enemy.DamageOverTime(damageOverTime, slowDuration);
            ;
        }
    }

    IEnumerator WaveBlast() {
        while(transform.localScale.x < maxSize.x - 5) {
            var scale = transform.localScale;
            scale = Vector3.Slerp(scale, maxSize, Time.fixedDeltaTime * Time.timeScale * speed);
            scale.y = 0.01f;
            transform.localScale = scale;

            yield return null;
        }
        done = true;
    }
}
