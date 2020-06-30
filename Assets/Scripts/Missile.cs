using UnityEngine;

public class Missile : MonoBehaviour {
    public GameObject impactEffect;
    public float speed = 50.0f, explosionRadius = 0.0f, distanceThisFrame;
    public int penetration, damage = 50;
    public bool miss;

    private Transform target;
    private Vector3 initDirection = Vector3.zero;
    private float lifeEnd;

    void Start() {
        lifeEnd = Time.time + 1;
        initDirection = target.position - transform.position;
    }

    // Update is called once per frame
    void Update() {
        distanceThisFrame = speed * Time.deltaTime;

        if(initDirection.magnitude <= 0) {
        }

        if(!target) {
            Destroy(gameObject);
            return;
        }

        if(!miss) {
            Vector3 direction = target.position - transform.position;

            if(direction.magnitude <= distanceThisFrame) {
                HitTarget();
                return;
            }

            transform.Translate(direction.normalized * distanceThisFrame, Space.World);
            transform.LookAt(target);
        } else { //manual mode and if target dies before reaching them
            TryFindNewTargetInfront();
        }

        if(Time.time > lifeEnd) {
            Destroy(gameObject);
        }
    }

    public void Seek(Transform _target) {
        target = _target;
    }

    void HitTarget() {
        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 5.0f);
        Explode();
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

    void TryFindNewTargetInfront() {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, float.MaxValue)) {
            if(hit.collider) {
                target = hit.collider.transform;
                miss = false;
                return;
            }
        }
        transform.Translate(initDirection.normalized * distanceThisFrame, Space.World);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
