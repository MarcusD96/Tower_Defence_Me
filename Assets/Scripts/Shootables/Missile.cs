using System.Collections.Generic;
using UnityEngine;

public class Missile : Projectile {

    private int penetration;
    private float explosionRadius;

    void Awake() {
        missile = this;
    }

    public void SetExplosion(int penetration_, float radius_) {
        penetration = penetration_;
        explosionRadius = radius_;
    }

    new void Update() {
        base.Update();

        speed = Mathf.Pow(speed, 1.015f);
        speed = Mathf.Clamp(speed, 0, 100);
        if(!miss) { //has target that is not the mock enemy, the bullet is locked in on an enemy, still need to check if target died on the way           
            if(target) {
                Vector3 direction = target.position - transform.position;
                transform.Translate(direction.normalized * distanceThisFrame, Space.World);
                transform.LookAt(target);
            } else {
                miss = true;
                TryFindNewTargetInfront();
            }
        } else {
            TryFindNewTargetInfront();
        }
    }

    public override void HitTarget(bool endOfLife) {
        Explode();
        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 3.0f);
    }

    void Explode() {
        //save the enemy list
        List<GameObject> e = new List<GameObject>(WaveSpawner.GetEnemyList_Static());

        //remove enemies not in explosion range
        for(int i = 0; i < e.Count - 1; i++) {
            if(!target)
                continue;
            if(Vector3.Distance(target.position, e[i].transform.position) > explosionRadius) {
                e.RemoveAt(i);
            }
        }
        e.TrimExcess();

        //sort enemies based on distance to target/explosion
        e.Sort((a, b) => {
            if(target) {
                return Vector3.Distance(target.position, a.transform.position).CompareTo(Vector3.Distance(target.position, b.transform.position));
            } else
                return 0;
        });


        //if penetration is smaller then it will use max pentration, but if there arent as many viable enemies, then the penetration will match the enemy count
        penetration = Mathf.Min(penetration, e.Count);

        var s = WaveSpawner.GetEnemyList_Static();
        for(int i = 0; i < penetration; i++) {
            if(!target)
                continue;
            if(Vector3.Distance(target.position, e[i].transform.position) <= explosionRadius) {
                if(target.GetComponent<Enemy>().isBoss) {
                    damage *= 2;
                    Damage(e[i].transform);
                    damage /= 2;
                } else
                    Damage(e[i].transform);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
