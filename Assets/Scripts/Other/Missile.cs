using UnityEngine;

public class Missile : MonoBehaviour {

    public GameObject impactEffect;
    public float speed = 50.0f, explosionRadius = 0.0f;
    public int penetration, damage = 50;
    public bool miss;

    private Transform target;
    private Vector3 preDirection;
    private float lifeEnd, distanceThisFrame;

    void Start() {
        lifeEnd = Time.time + 10;
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
            Explode();
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
