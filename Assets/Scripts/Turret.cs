using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour {
    public Transform fireSpawn;

    private Transform target;
    private Enemy targetEnemy;
    private bool manual = false;

    [Header("Global")]
    public float range = 15.0f;
    public Camera turretCam;
    public GameObject turretView;

    [Header("Use Bullets")]
    public GameObject projectilePrefab;
    public Enemy mockEnemy;
    public float fireRate = 2.0f; //shots per second, higher is faster
    private float nextFire = 0.0f;

    [Header("Use Laser")]
    public bool useLaser = false;
    [Range(0.1f, 1.0f)]
    public float slowFactor = 0.8f;
    public int damageOverTime = 30;
    public LineRenderer lineRenderer;
    public Transform laserEnd;
    public ParticleSystem impactEffect;
    public Light impactLight;

    [Header("Setup")]
    public Transform pivot;
    public string enemyTag = "Enemy";
    public float turnSpeed = 10.0f;
    public float manualTurnSpeed = 100.0f;

    void Start() {
        turretCam.enabled = false;
        if(laserEnd) {
            laserEnd.position = new Vector3(laserEnd.position.x, laserEnd.position.y, transform.position.z + (range * 2));
        }
        if(mockEnemy) {
            mockEnemy.transform.position = new Vector3(fireSpawn.position.x, fireSpawn.position.y, transform.position.z + (range * 2));
        }
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Tab)) {
            if(Time.timeScale == 0.5f) {
                Time.timeScale = 1;
            } else
                Time.timeScale = 0.5f;
        }

        if(manual) {
            ManualControl();
        } else
            AutomaticControl();
    }

    void FindNearestTargetInRange() {
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
            targetEnemy = nearestEnemy.GetComponent<Enemy>();
        } else
            target = null;
    }

    void AutoShoot() {
        GameObject bulletGO = Instantiate(projectilePrefab, fireSpawn.position, fireSpawn.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if(bullet) {
            bullet.MakeTarget(target);
        }
    }

    void AutoLaser() {
        //damage
        targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
        targetEnemy.Slow(slowFactor);

        //graphics
        if(!lineRenderer.enabled) {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }
        lineRenderer.SetPosition(0, fireSpawn.position);
        lineRenderer.SetPosition(1, target.position);

        Vector3 direction = fireSpawn.position - target.position;
        impactEffect.transform.position = target.position + direction.normalized;
        impactEffect.transform.rotation = Quaternion.LookRotation(direction);
    }

    void RotateWithTarget() {
        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 euler = Quaternion.Lerp(pivot.rotation, rotation, Time.deltaTime * turnSpeed).eulerAngles;
        pivot.rotation = Quaternion.Euler(0, euler.y, 0);

    }

    void AutomaticControl() {
        if(!IsInvoking("FindNearestTargetInRange")) {
            InvokeRepeating("FindNearestTargetInRange", 0, 0.5f);
        }
        if(!target) {
            if(useLaser) {
                if(lineRenderer.enabled) {
                    lineRenderer.enabled = false;
                    impactEffect.Stop();
                    impactLight.enabled = false;
                }
            }
            return;
        }

        RotateWithTarget();

        if(useLaser) {
            AutoLaser();
        } else {
            if(nextFire <= 0.0f) {
                AutoShoot();
                nextFire = 1 / fireRate;
            }

            nextFire -= Time.deltaTime;
        }
    }

    void ManualControl() {
        if(IsInvoking("FindNearestTargetInRange"))
            CancelInvoke("FindNearestTargetInRange");

        ManualMovement();

        if(useLaser) {
            if(Input.GetMouseButton(0)) {
                ManualLaser();
            } else {
                lineRenderer.enabled = false;
                impactEffect.Stop();
                impactLight.enabled = false;
            }
        } else {
            var manualFireRate = fireRate * 1.5f;
            if(Input.GetMouseButton(0)) {
                if(nextFire <= 0.0f) {
                    ManualShoot();
                    nextFire = 1 / manualFireRate;
                }
                nextFire -= Time.deltaTime;
            }
        }
    }

    void ManualShoot() {
        float manualRange = range * 2;

        //spawn bullet, get the bullet info
        GameObject bulletGO = Instantiate(projectilePrefab, fireSpawn.position, fireSpawn.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        //get a target only if theres a hit, otherwise the target is the mockEnemy; 
        RaycastHit hit;
        if(Physics.Raycast(fireSpawn.position, pivot.forward, out hit, manualRange)) {
            if(hit.collider) {
                //set the target
                bullet.miss = false;
                target = hit.collider.transform;
            }
        } else {
            bullet.miss = true;
            target = mockEnemy.transform;
        }

        if(bullet) {
            bullet.MakeTarget(target);
        }
    }

    void ManualLaser() {
        lineRenderer.SetPosition(0, fireSpawn.position);

        float manualRange = range * 2, manualSlowFactor = slowFactor * 0.75f;
        int manualDoT = Mathf.RoundToInt(damageOverTime * 1.3f);


        RaycastHit hit;
        if(Physics.Raycast(fireSpawn.position, pivot.forward, out hit, manualRange)) {
            if(hit.collider) {
                //set the target and get its information
                target = hit.collider.transform;
                targetEnemy = target.GetComponent<Enemy>();

                if(targetEnemy) {
                    //apply damage and slow
                    targetEnemy.TakeDamage(manualDoT * Time.deltaTime);
                    targetEnemy.Slow(manualSlowFactor);
                }

                //set end position of the laser line renderer
                lineRenderer.SetPosition(1, hit.point);

                //enable the light and particles
                if(!impactEffect.isPlaying) {
                    impactEffect.Play();
                    impactLight.enabled = true;
                }

                //turn the particles towards the turret
                Vector3 direction = transform.position - target.transform.position;
                impactEffect.transform.position = target.position + direction.normalized;
                impactEffect.transform.rotation = Quaternion.LookRotation(direction);
            }
        } else {
            target = null;
            targetEnemy = null;
            lineRenderer.SetPosition(1, laserEnd.position);
            lineRenderer.enabled = true;
            impactEffect.Stop();
            impactLight.enabled = false;
        }
    }

    void ManualMovement() {
        if(Input.GetKey(KeyCode.A)) {
            pivot.Rotate(Vector3.down * manualTurnSpeed * Time.deltaTime); //rotate along the -y axis/left
        }
        if(Input.GetKey(KeyCode.D)) {
            pivot.Rotate(Vector3.up * manualTurnSpeed * Time.deltaTime); //rotate along the +y axis/right
        }

        float mouseInput = Input.GetAxis("Mouse X");
        Vector3 lookhere = new Vector3(0, mouseInput, 0);
        pivot.Rotate(lookhere);
    }

    public void AssumeControl() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        manual = true;
        turretCam.enabled = true;
        turretView.SetActive(true);
    }

    public void RevertControl() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        manual = false;
        turretCam.enabled = false;
        turretView.SetActive(false);
    }

    void OnDrawGizmosSelected() {
        Color color = Color.red;
        color.a = 0.3f;
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}