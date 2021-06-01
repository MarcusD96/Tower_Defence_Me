
using UnityEngine;

public class Bullet : Projectile {

    Vector3 direction;

    void Awake() {
        bullet = this;
        startSpeed = speed;
    }

    public void InitializeDirection() {
        direction = VaryDirection();
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
            ObjectPool.instance.Deactivate(gameObject);
            return;
        }
        ObjectPool.instance.Deactivate(gameObject);
        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 1.0f);
    }

    Vector3 VaryDirection() {
        float r = Random.Range(-0.03f, 0.03f);
        Vector3 direction = transform.rotation * new Vector3(Vector3.forward.x + r, Vector3.forward.y, Vector3.forward.z);
        direction.Normalize();
        return direction;
    }
}