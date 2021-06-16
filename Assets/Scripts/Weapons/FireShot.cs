
using System.Collections.Generic;
using UnityEngine;

public class FireShot : ParticleShot {
    [Header("FireShot")]
    public Gradient mainGradient, specGradient;

    float burnDamage, burnInterval;
    int numFire, burnTime;
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

        ps = GetComponent<ParticleSystem>();
        ParticleSystem.ColorOverLifetimeModule grad = ps.colorOverLifetime;
        grad.color = currentGrad;

        ParticleSystem.MainModule main = ps.main;
        main.startSpeed = (range * 3) + 5;
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0, (short) numFire_);
        ps.emission.SetBurst(0, burst);

        ps.Play();

        CheckHits();
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
                    }
                    else {
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
                    }
                    else {
                        e.TakeDamage(burnDamage * 3, Color.red);
                        e.Burn(burnDamage, burnTime * 2, burnInterval / 2, level);
                    }
                }
            }
        }
    }
}
