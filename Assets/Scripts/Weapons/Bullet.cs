
using UnityEngine;

public class Bullet : Projectile {

    Vector3 direction;
    int penetration;

    void Awake() {
        bullet = this;
        startSpeed = speed;
    }

    public void Initialize(int penetration_) {
        direction = VaryDirection();
        penetration = penetration_;
    }

    new void FixedUpdate() {
        base.FixedUpdate();
        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }
    
    public override void HitTarget(bool endOfLife) {
        if(endOfLife) {
            ObjectPool.instance.ActivateEffect(EffectType.BulletImpact, transform.position, transform.rotation, 1.0f);
            ObjectPool.instance.Deactivate(gameObject);
            return;
        }

        if(penetration <= 0) {
            Damage(target);
            ObjectPool.instance.ActivateEffect(EffectType.BulletImpact, transform.position, transform.rotation, 1.0f);
            ObjectPool.instance.Deactivate(gameObject);
            return;
        }

        ObjectPool.instance.ActivateEffect(EffectType.BulletImpact, transform.position, transform.rotation, 1.0f);
        Damage(target);
        penetration--;
    }

    Vector3 VaryDirection() {
        float r = Random.Range(-0.05f, 0.05f);
        Vector3 direction = transform.rotation * new Vector3(Vector3.forward.x + r, Vector3.forward.y, Vector3.forward.z);
        direction.Normalize();
        return direction;
    }
}