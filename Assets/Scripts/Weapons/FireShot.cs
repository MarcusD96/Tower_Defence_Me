
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FireShot : MonoBehaviour {

    public Gradient mainGradient, specGradient;
    public LayerMask enemyLayer;

    ParticleSystem fire;
    float range, burnDamage, burnInterval;
    int numFire, burnTime, level;
    bool special;
    Gradient currentGrad;

    public void Initialize(int level_, int numFire_, float range_, float burnDamage_, float burnInterval_, int burnTime_, bool special_) {
        level = level_;
        numFire = numFire_;
        range = range_;
        burnDamage = burnDamage_;
        burnInterval = burnInterval_;
        burnTime = burnTime_;
        if(special_)
            currentGrad = specGradient;
        else
            currentGrad = mainGradient;

        fire = GetComponent<ParticleSystem>();
        ParticleSystem.ColorOverLifetimeModule grad = fire.colorOverLifetime;
        grad.color = currentGrad;

        ParticleSystem.MainModule main = fire.main;
        main.startSpeed = (range * 3) + 5;
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0, (short) numFire_);
        fire.emission.SetBurst(0, burst);

        fire.Play();

        CheckHits();
    }

    void LateUpdate() {
        if(fire.isStopped) {
            ObjectPool.instance.Deactivate(gameObject);
        }
    }

    Ray ray;
    Enemy e;
    RaycastHit hit;
    RaycastHit[] hits;
    void CheckHits() {

        for(int i = 0; i < numFire; i++) {
            ray = new Ray(transform.position, Quaternion.AngleAxis(360 / numFire * i, Vector3.up) * Vector3.forward);

            if(!special) {
                if(Physics.SphereCast(ray, 1, out hit, range, enemyLayer)) {
                    e = hit.collider.gameObject.GetComponent<Enemy>();
                    if(e.burnResist)
                        e.TakeDamage(burnDamage / 2, Color.red);

                    if(e.isBoss) {
                        e.TakeDamage(burnDamage * 5, Color.white);
                        e.Burn(burnDamage * 2, burnTime, burnInterval, level);
                    } else {
                        e.TakeDamage(burnDamage, Color.white);
                        e.Burn(burnDamage, burnTime, burnInterval, level);
                    }
                }
            }
            else {
                hits = Physics.SphereCastAll(ray, 1, range, enemyLayer);
                foreach(RaycastHit h in hits) {
                    e = h.collider.GetComponent<Enemy>();
                    if(e.isBoss) {
                        e.TakeDamage(burnDamage * 75, Color.red);
                        e.Burn(burnDamage * 10, burnTime * 5, burnInterval, level);
                    } else {
                        e.TakeDamage(burnDamage * 3, Color.red);
                        e.Burn(burnDamage, burnTime * 2, burnInterval / 2, level);
                    }
                }
            }
        }
    }
}
