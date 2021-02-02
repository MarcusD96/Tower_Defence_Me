
using UnityEngine;

public class Projectile : MonoBehaviour {

    [Header("Projectile Properties")]
    public GameObject impactEffect;
    public float speed = 100.0f;
    public bool miss, special;

    protected Missile missile;
    protected Bullet bullet;
    protected Rod rod;

    protected Transform target;
    protected float distanceThisFrame;
    protected float damage;

    protected Vector3 startPos, endPos;

    void Start() {
        startPos = transform.position;
    }

    protected void Update() {
        //Debug.DrawLine(startPos, endPos, Color.cyan);
        //Debug.DrawLine(startPos, transform.position, Color.green);

        if(!special) { //special projectiles may not need this feauture
            if(Vector3.Distance(transform.position, startPos) >= Vector3.Distance(endPos, startPos)) { //if the projectile has gone to its max range, die
                HitTarget(true);
                return;
            }
        }

        distanceThisFrame = speed * Time.deltaTime;
    }

    public Missile GetMissile() {
        return missile;
    }

    public Bullet GetBullet() {
        return bullet;
    }

    public Rod GetRod() {
        return rod;
    }

    public void SetDamage(float damage_) {
        damage = damage_;
    }

    public void SetLifePositions(Vector3 startPos_, Vector3 endPos_) {
        startPos = startPos_;
        endPos = endPos_;
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
            e.TakeDamage(damage, true);
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
