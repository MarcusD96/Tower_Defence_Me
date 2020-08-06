
using UnityEngine;

public class Bullet : Projectile {

    void Awake() {
        bullet = this;
        lifeEnd = Time.time + 1;
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
