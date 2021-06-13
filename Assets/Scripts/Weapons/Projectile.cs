
using UnityEngine;

public class Projectile : MonoBehaviour {

    [Header("Projectile Properties")]
    public GameObject impactEffect;
    public float speed;
    protected float startSpeed;
    public bool special;

    protected Missile missile;
    protected Bullet bullet;
    protected Rod rod;
    protected TankShot tankShot;

    protected Transform target;
    protected float distanceThisFrame;
    protected float damage, bossDamage;
    protected bool isCollided = false;

    protected Vector3 startPos, endPos;

    void Start() {
        startPos = transform.position;
    }

    protected void FixedUpdate() {
        if(Vector3.Distance(transform.position, startPos) >= Vector3.Distance(endPos, startPos)) { //if the projectile has gone to its max range, die
            HitTarget(true);
            return;
        }

        distanceThisFrame = speed * Time.fixedDeltaTime;
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

    public TankShot GetTankShot() {
        return tankShot;
    }

    public void SetDamage(float damage_, float bossDamage_) {
        damage = damage_;
        bossDamage = bossDamage_;
    }

    public void SetLifePositions(Vector3 startPos_, Vector3 endPos_) {
        startPos = startPos_;
        endPos = endPos_;
    }

    public void SetStats(float damage_, float bossDamage_, Vector3 startPos_, Vector3 endPos_) {
        SetDamage(damage_, bossDamage_);
        SetLifePositions(startPos_, endPos_);
        speed = startSpeed;
        isCollided = false;
    }

    public void MakeTarget(Transform _target) {
        target = _target;
    }

    public virtual void HitTarget(bool endOfLife) {
        Debug.Log("Projectile.HitTarget()");
    }

    protected void Damage(Transform enemy) {
        Enemy e = enemy.GetComponent<Enemy>();
        if(e) {
            if(e.isBoss && bossDamage > 0)
                e.TakeDamage(bossDamage, Color.grey);
            else {
                e.TakeDamage(damage, Color.grey);
            }
        }
    }

    protected void TryFindNewTargetInfront() {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, float.MaxValue)) {
            if(hit.collider.gameObject.CompareTag("Enemy")) {
                target = hit.collider.transform;
                return;
            }
        }
        transform.Translate(transform.forward * distanceThisFrame, Space.World);
    }

    protected void OnTriggerEnter(Collider other) {
        if(!isCollided) {
            if(other.gameObject.CompareTag("Enemy")) {
                target = other.transform;
                HitTarget(false);
                if(rod || bullet)
                    return;
                else
                    isCollided = true;

            }
        }
    }
}
