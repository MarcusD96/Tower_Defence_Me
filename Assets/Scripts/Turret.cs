using UnityEngine;

public class Turret : MonoBehaviour {

    private Transform target;
    public Transform fireSpawn;

    [Header("Attributes")]
    public float range = 15.0f;

    [Header("Use Bullets")]
    public GameObject bulletPrefab;
    public float fireRate = 2.0f; //shots per second, higher is faster
    private float nextFire = 0.0f;

    [Header("Use Laser")]
    public bool useLaser = false;
    public LineRenderer lineRenderer;

    [Header("Setup")]
    public Transform pivot;
    public string enemyTag = "Enemy";
    public float turnSpeed = 10.0f;

    void Start() {
        InvokeRepeating("UpdateTarget", 0, 0.5f);
    }

    void Update() {
        if(!target) {
            if (useLaser) {
                if(lineRenderer.enabled)
                    lineRenderer.enabled = false;
            }
            return;
        }
        
        RotateWithTarget();

        if(useLaser) {
            Laser();
        } else {
            if(nextFire <= 0.0f) {
                //RotateOnShoot();
                Shoot();
                nextFire = 1 / fireRate;
            }

            nextFire -= Time.deltaTime;
        }

    }

    void UpdateTarget() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = float.MaxValue;
        GameObject nearestEnemy = null;

        foreach(var e in enemies) {
            float distance = Vector3.Distance(transform.position, e.transform.position);
            if(distance < shortestDistance) {
                shortestDistance = distance;
                nearestEnemy = e;
            }
        }

        if(nearestEnemy && shortestDistance <= range) {
            target = nearestEnemy.transform;
        } else
            target = null;
    }

    void Shoot() {
        GameObject bulletGO = Instantiate(bulletPrefab, fireSpawn.position, fireSpawn.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if(bullet) {
            bullet.Seek(target);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void Laser() {
        if(!lineRenderer.enabled)
            lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, fireSpawn.position);
        lineRenderer.SetPosition(1, target.position);
    }

    void RotateWithTarget() {
        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 euler = Quaternion.Lerp(pivot.rotation, rotation, Time.deltaTime * turnSpeed).eulerAngles;
        pivot.rotation = Quaternion.Euler(0, euler.y, 0);

    }

    void RotateOnShoot() {
        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        pivot.rotation = rotation;
    }

}