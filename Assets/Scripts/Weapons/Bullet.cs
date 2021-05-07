
using UnityEngine;

public class Bullet : Projectile {

    Vector3 direction;

    void Awake() {
        bullet = this;
        direction = transform.forward;
    }

    new void FixedUpdate() {
        base.FixedUpdate();
        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    public override void HitTarget(bool endOfLife) {
        if(!endOfLife) {
            Damage(target);
            GameObject effect = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effect, 1.0f);
            Destroy(gameObject);
            return;
        }
        Destroy(gameObject);
        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 1.0f);
    }
}