using System.Collections.Generic;
using UnityEngine;

public class Missile : Projectile {

    public float speedExponent;

    private int penetration;
    private float explosionRadius;

    void Awake() {
        missile = this;
    }

    public void SetExplosion(int penetration_, float radius_) {
        penetration = penetration_;
        explosionRadius = radius_;
    }

    private new void FixedUpdate() {
        base.FixedUpdate();

        speed = Mathf.Pow(speed, speedExponent);
        speed = Mathf.Clamp(speed, 0, 100);
        if(target) {
            Vector3 direction = target.position - transform.position;
            transform.Translate(direction.normalized * distanceThisFrame, Space.World);
            transform.LookAt(target);
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
            if(Vector3.Distance(transform.position, e[i].transform.position) > explosionRadius) {
                e.RemoveAt(i);
            }
        }
        e.TrimExcess();

        //sort enemies based on distance to target/explosion
        e.Sort((a, b) => {
            return Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position));
        });


        //if penetration is smaller then it will use max pentration, but if there arent as many viable enemies, then the penetration will match the enemy count
        penetration = Mathf.Min(penetration, e.Count);

        var s = WaveSpawner.GetEnemyList_Static();
        for(int i = 0; i < penetration; i++) {
            if(Vector3.Distance(transform.position, e[i].transform.position) <= explosionRadius) {
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
