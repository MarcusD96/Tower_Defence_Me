
using UnityEngine;

public class Rod : Projectile {

    public GameObject rodExplodeEffect;

    private int penetration;
    public bool explosive;
    private Vector3 direction;


    void Awake() {
        rod = this;
        direction = transform.forward;
    }

    new void Update() {
        base.Update();

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);

        if(penetration <= 0) {
            Destroy(gameObject);
        }
    }

    public void SetPenetration(int penetration_) { //giggiddy
        penetration = penetration_;
    }

    protected override void HitTarget(bool endOfLife) {
        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 3.0f);

        if(endOfLife) {
            Destroy(gameObject);
            return;
        }

        if(!explosive) {
            Damage(target);
        } else {
            Explode();
            GameObject explosiveEffectInstance = Instantiate(rodExplodeEffect, transform.position, transform.rotation);
            Destroy(explosiveEffectInstance, 3.0f);
        }
    }

    void Explode() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 4);
        int i = 3;
        foreach(var c in colliders) {
            if(c.gameObject.CompareTag("Enemy")) {
                if(i > 0) {
                    Damage(c.transform);
                } else
                    break;
                i--;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Enemy")) {
            target = other.transform;
            HitTarget(false);
            penetration--;
        }
    }
}
