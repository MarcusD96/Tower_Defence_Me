using System.Collections;
using UnityEngine;

public class SlowWave : MonoBehaviour {
    public float speed, slowFactor, slowDuration;
    public Vector3 maxSize;
    [HideInInspector]
    public bool done = false;

    void Start() {
        StartCoroutine(WaveBlast());
    }

    void OnTriggerEnter(Collider other) {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if(enemy) {
            enemy.Slow(slowFactor, slowDuration); 
        }
    }

    IEnumerator WaveBlast() {
        while(transform.localScale.x < maxSize.x - 5) {
            transform.localScale = Vector3.Slerp(transform.lossyScale, maxSize, Time.deltaTime * speed);
            yield return null;
        }
        done = true;
    }
}
