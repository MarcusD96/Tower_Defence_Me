using UnityEngine;

public class Missile : Projectile {

    private int penetration;
    private float explosionRadius = 5.0f;

    void Awake() {
        missile = this;
        lifeEnd = Time.time + 3;
    }

    public void SetExplosion(int penetration_, float radius_) {
        penetration = penetration_;
        explosionRadius = radius_;
    }

    new void Update() {
        base.Update();
    }

    protected override void HitTarget(bool endOfLife) {
        if(!endOfLife) {
            Explode();
            GameObject indicatorInstance = Instantiate(indicator, transform.position, Quaternion.identity);
            indicatorInstance.GetComponent<DamageIndicator>().damage = damage;
            Destroy(indicatorInstance, 0.5f);
        }
        Destroy(gameObject);

        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 5.0f);
    }

    void Explode() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        int p = 0;
        foreach(var c in colliders) {
            if(c.gameObject.CompareTag("Enemy")) {
                if(p < penetration) {
                    Damage(c.transform);
                    GameObject indicatorInstance = Instantiate(indicator, c.transform.position, Quaternion.identity);
                    indicatorInstance.GetComponent<DamageIndicator>().damage = damage;
                    Destroy(indicatorInstance, 0.5f);
                } else
                    break;
                p++;
            }
        }
    }
}
