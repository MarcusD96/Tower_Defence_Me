
using UnityEngine;

public class Projectile : MonoBehaviour {

    [Header("Projectile Properties")]
    public GameObject impactEffect;
    public GameObject indicator, type;
    public float speed = 100.0f;
    public bool miss;


    protected Transform target;
    protected float lifeEnd, distanceThisFrame;
    protected int damage; //penetration, explosionRadius;

    public void SetDamage(int damage_) {
        damage = damage_;
    }

    protected void Update() {
        if(Time.time > lifeEnd) {
            HitTarget(true);
            return;
        }

        distanceThisFrame = speed * Time.deltaTime;

        if(!miss) { //has target that is not the mock enemy, the bullet is locked in on an enemy, still need to check if target died on the way           
            if(target) {
                Vector3 direction = target.position - transform.position;
                if(direction.magnitude <= distanceThisFrame) {
                    HitTarget(false);
                    return;
                }
                transform.Translate(direction.normalized * distanceThisFrame, Space.World);
                transform.LookAt(target);
            } else {
                miss = true;
                TryFindNewTargetInfront();
            }
            return;
        } else {
            TryFindNewTargetInfront();
        }
    }

    public void MakeTarget(Transform _target) {
        target = _target;
    }

    protected virtual void HitTarget(bool endOfLife) {
        Debug.Log("acquire a target");
    }

    protected void Damage(Transform enemy) {
        Enemy e = enemy.GetComponent<Enemy>();
        if(e) {
            e.TakeDamage(damage);
        }
    }

    protected void TryFindNewTargetInfront() {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, float.MaxValue)) {
            if(hit.collider.gameObject.CompareTag("Enemy")) {
                target = hit.collider.transform;
                miss = false;
                return;
            }
        }
        transform.Translate(transform.forward * distanceThisFrame, Space.World);
    }
}
