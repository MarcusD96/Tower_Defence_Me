﻿
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
    }

    public void SetPenetration(int penetration_) { //giggiddy
        penetration = penetration_;
    }

    public override void HitTarget(bool endOfLife) {
        if(endOfLife) {
            Destroy(gameObject);
            GameObject effect = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effect, 3.0f);
            return;
        }

        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 3.0f);

        if(!explosive) {
            Damage(target);
        } else {
            Explode();
            GameObject explosiveEffectInstance = Instantiate(rodExplodeEffect, transform.position, transform.rotation);
            Destroy(explosiveEffectInstance, 3.0f);
        }        

        penetration--;

        if(penetration <= 0) {
            Destroy(gameObject);
            GameObject effect = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effect, 3.0f);
            return;
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
}
