
using UnityEngine;

public class TankShot : Projectile {

    Vector3 direction;

    private void Awake() {
        tankShot = this;
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
