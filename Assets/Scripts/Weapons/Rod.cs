
using UnityEngine;

public class Rod : Projectile {

    private int penetration;
    private Vector3 direction;


    void Awake() {
        rod = this;
        direction = transform.forward;
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
            Destroy(gameObject);
            GameObject effect = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effect, 2.0f);
            return;
        }

        if(penetration <= 0) {
            Destroy(gameObject);
            GameObject effect = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effect, 2.0f);
            return;
        }

        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 3.0f);


        Damage(target);

        penetration--;
    }
}
