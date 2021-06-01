
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FireShot : MonoBehaviour {

    ParticleSystem fire;
    public Gradient mainGradient, specGradient;
    float radius, damage, burnDamage, burnInterval;
    int penetration, burnTime;

    bool special;

    public void Initialize(float damage_, float radius_, float burnDamage_, float burnInterval_, int burnTime_, int penetration_, bool special_) {
        damage = damage_;
        radius = radius_;
        burnDamage = burnDamage_;
        burnInterval = burnInterval_;
        burnTime = burnTime_;
        penetration = penetration_;
        special = special_;

        fire = GetComponent<ParticleSystem>();
        fire.Play();

        ParticleSystem.MainModule main = fire.main;
        main.startSpeed = (radius * 3) + 5;

        CheckHits();
    }

    void LateUpdate() {
        if(fire.isStopped) {
            ObjectPool.instance.Deactivate(gameObject);
        }
    }


    void CheckHits() {
        List<Collider> hits = new List<Collider>(Physics.OverlapSphere(transform.position, radius));

        if(hits.Count > 0) {
            //trim out non enemies
            for(int i = hits.Count - 1; i >= 0; i--) {
                if(!hits[i].CompareTag("Enemy")) {
                    hits.RemoveAt(i);
                }
            }

            //sort enemies by first spawned
            hits.Sort((a, b) => {
                return b.gameObject.GetComponent<Enemy>().distanceTravelled.CompareTo(a.gameObject.GetComponent<Enemy>().distanceTravelled);
            });

            //damage remaining enemies
            foreach(var h in hits) {
                if(h == null)
                    continue;

                if(penetration == 0)
                    break;
                var e = h.gameObject.GetComponent<Enemy>();

                ParticleSystem.ColorOverLifetimeModule grad = fire.colorOverLifetime;

                if(special) {
                    grad.color = specGradient;

                    if(e.isBoss) {
                        e.TakeDamage(damage * 100, Color.red);
                    } else {
                        e.TakeDamage(damage * 5, Color.red);
                    }

                    e.Burn(burnDamage, burnTime * 2, burnInterval);

                } else {

                    grad.color = mainGradient;

                    if(e.isBoss) {
                        e.TakeDamage(damage * 3, Color.red);
                    } else
                        e.TakeDamage(damage, Color.red);
                    e.Burn(burnDamage, burnTime, burnInterval);
                }

                penetration--;
            }
        }
    }
}
