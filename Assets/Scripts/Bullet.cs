using System.Security.Cryptography;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private Transform target;

    public GameObject impactEffect;

    public float speed = 50.0f;

    // Update is called once per frame
    void Update() {
        if(!target) {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if(direction.magnitude <= distanceThisFrame) {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    public void Seek(Transform _target) {
        target = _target;
    }

    void HitTarget() {
        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);        
        Destroy(target.gameObject);
        Destroy(effectInstance, 2.0f);
        Destroy(gameObject);
    }
}
