
using UnityEngine;

public class Rod : Projectile {

    private int penetration;
    private Vector3 direction;


    void Awake() {
        rod = this;
        startSpeed = speed;
    }

    public void InitializeDirection() {
        direction = transform.rotation * Vector3.forward;
    }
    new void FixedUpdate() {
        base.FixedUpdate();

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    public void SetPenetration(int penetration_) { //giggiddy
        penetration = penetration_;
    }

    public override void HitTarget(bool endOfLife) {
        if(endOfLife) {
            ObjectPool.instance.ActivateEffect(EffectType.RodImpact, transform.position, transform.rotation, 1.0f);
            ObjectPool.instance.Deactivate(gameObject);
            return;
        }

        if(penetration < 0) {
            ObjectPool.instance.ActivateEffect(EffectType.RodImpact, transform.position, transform.rotation, 1.0f);
            ObjectPool.instance.Deactivate(gameObject);
            return;
        }

        ObjectPool.instance.ActivateEffect(EffectType.RodImpact, transform.position, transform.rotation, 1.0f);
        Damage(target);
        penetration--;
    }
}
