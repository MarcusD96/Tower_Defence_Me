using UnityEngine;

public class Missile : Projectile {

    private int penetration;
    private float explosionRadius = 3.5f;

    void Awake() {
        missile = this;
    }

    public void SetExplosion(int penetration_, float radius_) {
        penetration = penetration_;
        explosionRadius = radius_;
    }

    new void Update() {
        base.Update();

        if(!miss) { //has target that is not the mock enemy, the bullet is locked in on an enemy, still need to check if target died on the way           
            if(target) {
                Vector3 direction = target.position - transform.position;
                transform.Translate(direction.normalized * distanceThisFrame, Space.World);
                transform.LookAt(target);
            } else {
                miss = true;
                TryFindNewTargetInfront();
            }
        } else {
            TryFindNewTargetInfront();
        }
    }

    protected override void HitTarget(bool endOfLife) {
        Explode();
        Destroy(gameObject);

        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 3.0f);
    }

    void Explode() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(var c in colliders) {
            if(c.gameObject.CompareTag("Enemy")) {
                if(penetration > 0) {
                    Damage(c.transform);
                } else
                    break;
                penetration--;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Enemy")) {
            target = other.transform;
            HitTarget(false);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
