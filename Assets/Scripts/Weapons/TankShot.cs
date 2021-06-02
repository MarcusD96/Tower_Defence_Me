
using UnityEngine;

public class TankShot : Projectile {

    Vector3 direction;

    private void Awake() {
        tankShot = this;
        startSpeed = speed;
    }

    private new void FixedUpdate() {
        base.FixedUpdate();

        if(direction != Vector3.zero) {
            transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        }
    }

    public void SetDirection(Vector3 d) {
        direction = d;
    }

    public override void HitTarget(bool endOfLife) {
        if(!endOfLife) {
            Damage(target);
            ObjectPool.instance.ActivateEffect(EffectType.TankShotImpact, transform.position, transform.rotation);
            ObjectPool.instance.Deactivate(gameObject);
            return;
        }
        ObjectPool.instance.ActivateEffect(EffectType.TankShotImpact, transform.position, transform.rotation);
        ObjectPool.instance.Deactivate(gameObject);
    }

}
