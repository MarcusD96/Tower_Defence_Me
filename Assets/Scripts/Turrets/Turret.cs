using UnityEngine;

public class Turret : MonoBehaviour {
    public Transform fireSpawn;

    protected Transform target;
    protected Enemy targetEnemy;
    protected bool manual = false;
    public int cost;

    [Header("Global")]
    public float range = 15.0f;
    public Camera turretCam;
    public GameObject turretView;
    public Enemy mockEnemy;

    [Header("Setup")]
    public Transform pivot;
    public string enemyTag = "Enemy";
    public float turnSpeed = 10.0f;
    public float manualTurnSpeed = 100.0f;
    protected LaserTurret laserTurret;
    protected ProjectileTurret projectileTurret;

    [Header("Upgrades")]
    public Upgrade ugA;
    public Upgrade ugB, ugSpec;

    [Header("Projectiles")]
    public float fireRate = 2.0f; //shots per second, higher is faster
    private float nextFire = 0.0f;

    void Start() {
        turretCam.enabled = false;
        mockEnemy.transform.position = new Vector3(fireSpawn.position.x, fireSpawn.position.y, transform.position.z + (range * 2));
    }

    void Update() {
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

    public int GetSellPrice() {
        return Mathf.RoundToInt((cost / 2) / 5) * 5; //rounds to nearest 5 value
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
            if(laserTurret)
                laserTurret.LaserOff();
            return;
        }

        RotateWithTarget();

        if(!laserTurret) {
            if(nextFire <= 0.0f) {
                projectileTurret.AutoShoot();
                nextFire = 1 / fireRate;
            }

            nextFire -= Time.deltaTime;
        } else
            laserTurret.AutoLaser();
    }

    void ManualControl() {
        if(IsInvoking("FindNearestTargetInRange"))
            CancelInvoke("FindNearestTargetInRange");

        ManualMovement();

        if(laserTurret) {
            if(Input.GetMouseButton(0)) {
                laserTurret.ManualLaser();
            } else {
                laserTurret.LaserOff();
            }
        } else {
            var manualFireRate = fireRate * 1.5f;
            if(Input.GetMouseButton(0)) {
                if(nextFire <= 0.0f) {
                    projectileTurret.ManualShoot();
                    nextFire = 1 / manualFireRate;
                }
                nextFire -= Time.deltaTime;
            }
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

    public virtual void ApplyUpgradeA() {
        Debug.Log("upgrade 1");
    }

    public virtual void ApplyUpgradeB() {
        Debug.Log("upgrade 2");
    }


    public virtual void EnableSpecial() {
        Debug.Log("upgrade Special");
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