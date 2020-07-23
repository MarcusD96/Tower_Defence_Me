using UnityEngine;

public class Bullet : MonoBehaviour {

    public GameObject impactEffect, indicator;
    public float speed = 150.0f;
    public bool miss;

    private Transform target;
    private float lifeEnd, distanceThisFrame;
    private int damage, penetration, explosionRadius;

    public void SetDamage(int damage_) {
        damage = damage_;
    }

    public void SetExplosion(int penetration_, int radius_) {
        penetration = penetration_;
        explosionRadius = radius_;
    }

    void Start() {
        lifeEnd = Time.time + 5;
    }

    // Update is called once per frame
    void Update() {
        if(Time.time > lifeEnd) {
            HitTarget(true);
            return;
        }

        distanceThisFrame = speed * Time.deltaTime;

        if(!miss) { //has target that is not the mock enemy, the bullet is locked in on an enemy, still need to check if target died on the way           
            if(target) {
                Vector3 direction = target.position - transform.position;
                if(direction.magnitude <= distanceThisFrame) {
                    HitTarget(false);
                    return;
                }
                transform.Translate(direction.normalized * distanceThisFrame, Space.World);
                transform.LookAt(target);
            } else {
                miss = true;
                TryFindNewTargetInfront();
            }
            return;
        } else {
            TryFindNewTargetInfront();
        }
    }

    public void MakeTarget(Transform _target) {
        target = _target;
    }

    void HitTarget(bool endOfLife) {
        if(!endOfLife) {
            if(explosionRadius > 0) {
                Explode();
            } else {
                Damage(target);
            }
            GameObject indicatorInstance = Instantiate(indicator, transform.position, transform.rotation);
            indicatorInstance.GetComponent<DamageIndicator>().damage = damage;
            Destroy(indicatorInstance, 0.5f);
        }
        Destroy(gameObject);

        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 5.0f);
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
            if(hit.collider.gameObject.CompareTag("Enemy")) {
                target = hit.collider.transform;
                miss = false;
                return;
            }
        }
        transform.Translate(transform.forward * distanceThisFrame, Space.World);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
        Debug.DrawRay(transform.position, transform.forward, Color.red);
    }
}
