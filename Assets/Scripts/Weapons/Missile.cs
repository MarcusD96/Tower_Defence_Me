using System.Collections.Generic;
using UnityEngine;

public class Missile : Projectile {

    public float speedExponent;

    private float explosionRadius, posY;
    private int penetration;

    void Awake() {
        missile = this;
        startSpeed = speed;
    }

    public void SetExplosion(int penetration_, float explRadius) {
        penetration = penetration_;
        explosionRadius = explRadius;
        posY = transform.position.y;
    }

    private new void FixedUpdate() {
        base.FixedUpdate();

        speed = Mathf.Pow(speed, speedExponent);
        speed = Mathf.Clamp(speed, 0, 100);
        if(target) {
            if(target.gameObject.activeSelf != true) {
                target = null;
                TryFindNewTargetInfront();
                return;
            }
            Vector3 direction;
            if(!special)
                direction = target.position - transform.position;
            else
                direction = transform.rotation * Vector3.forward;   
            
            transform.Translate(direction.normalized * distanceThisFrame, Space.World);
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
            StayOnY();
        } else {
            TryFindNewTargetInfront();
        }
    }

    void StayOnY() {
        Vector3 pos = transform.position;
        pos.y = posY;
        transform.position = pos;
    }

    public override void HitTarget(bool endOfLife) {
        Quaternion rot = Quaternion.Euler(impactEffect.transform.rotation.eulerAngles.x, Random.Range(0, 360), 0);
        GameObject e = ObjectPool.instance.ActivateEffect(EffectType.MissileExplosion, transform.position, rot, 1.0f);
        e.transform.localScale = new Vector3(explosionRadius, explosionRadius, 1);
        Explode();
    }

    void Explode() {
        //save the enemy list copy
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
        target = null;
        ObjectPool.instance.Deactivate(gameObject);
    }
}
