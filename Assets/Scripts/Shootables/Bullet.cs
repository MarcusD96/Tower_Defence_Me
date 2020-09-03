
using UnityEngine;

public class Bullet : Projectile {

    Vector3 direction;

    void Awake() {
        bullet = this;
        direction = transform.forward;
    }

    new void Update() {
        base.Update();
        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    protected override void HitTarget(bool endOfLife) {
        if(!endOfLife) {
            Damage(target);
        }
        Destroy(gameObject);

        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 5.0f);
    }
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Enemy")) {
            target = other.transform;
            HitTarget(false);
        }
    }
}
