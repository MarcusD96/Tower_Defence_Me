using UnityEngine;

public class Turret : MonoBehaviour {

    private Transform target;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    [Header("Attributes")]
    public float range = 15.0f;
    public float fireRate = 2.0f; //shots per second, higher is faster
    private float nextFire = 0.0f;

    [Header("Other")]
    public Transform pivot;
    public string enemyTag = "Enemy";
    public float turnSpeed = 10.0f;

    void Start() {
        InvokeRepeating("UpdateTarget", 0, 0.5f);
    }

    void Update() {
        if(!target)
            return;
        
        //target lock on and rotate smoothly
        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 euler = Quaternion.Lerp(pivot.rotation, rotation, Time.deltaTime * turnSpeed).eulerAngles;
        pivot.rotation = Quaternion.Euler(0, euler.y, 0);        

        if(nextFire <= 0.0f) {
            /*
            //rotate only when shooting
            Vector3 direction = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            pivot.rotation = rotation;*/
            
            Shoot();
            nextFire = 1 / fireRate;
        }
        nextFire -= Time.deltaTime;
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
        GameObject bulletGO = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if(bullet) {
            bullet.Seek(target);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}