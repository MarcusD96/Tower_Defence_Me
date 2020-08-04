
using UnityEngine;

public class Bullet : Projectile {

    void Start() {
        lifeEnd = Time.time + 2;
    }

    new void Update() {
        base.Update();
    }

    protected override void HitTarget(bool endOfLife) {
        if(!endOfLife) {
            Damage(target);
            GameObject indicatorInstance = Instantiate(indicator, transform.position, Quaternion.identity);
            indicatorInstance.GetComponent<DamageIndicator>().damage = damage;
            Destroy(indicatorInstance, 0.5f);
        }
        Destroy(gameObject);

        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 5.0f);
    }
}
