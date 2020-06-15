using UnityEngine;

public class Bullet : MonoBehaviour {

    public GameObject impactEffect;
    public float speed = 50.0f, explosionRadius = 0.0f;
    public int penetration, damage = 50;

    private Transform target;

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
        transform.LookAt(target);
    }

    public void Seek(Transform _target) {
        target = _target;
    }

    void HitTarget() {
        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 5.0f);

        if(explosionRadius > 0) {
            Explode();
        } else {
            Damage(target);
        }

        Destroy(gameObject);
    }

    void Damage(Transform enemy) {
        Enemy e = enemy.GetComponent<Enemy>();
        if(e) {
            e.TakeDamage(damage);
        }
    }

    void Explode() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        int p = 0;
        foreach(var c in colliders) {
            if(c.gameObject.CompareTag("Enemy")) {
                if(p < penetration) {
                    Damage(c.transform);
                } else
                    break;
                p++;
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
