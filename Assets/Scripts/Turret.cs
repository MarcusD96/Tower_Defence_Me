using System;
using UnityEngine;

public class Turret : MonoBehaviour {

    public Transform fireSpawn;
    public bool manual = false;

    private Transform target;
    private Enemy targetEnemy;

    [Header("Global")]
    public float range = 15.0f;
    public Camera turretCam;
    public GameObject turretView;

    [Header("Use Bullets")]
    public GameObject projectilePrefab;
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
        InvokeRepeating("FindNearestTargetInRange", 0, 0.5f);
        turretCam.enabled = false;
        if(laserEnd) {
            laserEnd.position = new Vector3(laserEnd.position.x, laserEnd.position.y, range);
        }
    }

    void LateUpdate() {
        if(manual) {
            turretView.SetActive(true);
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
            bullet.Seek(target);
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
            Debug.DrawLine(fireSpawn.position, laserEnd.position, Color.green);
            if(Input.GetMouseButton(0)) {
                ManualLaser();
            } else {
                lineRenderer.enabled = false;
                impactEffect.Stop();
                impactLight.enabled = false;
            }
        } else {
            if(Input.GetMouseButton(0)) {

            }
        }
    }

    void ManualShoot() {

    }

    void ManualLaser() {
        lineRenderer.SetPosition(0, fireSpawn.position);

        RaycastHit hit;
        if(Physics.Raycast(fireSpawn.position, pivot.forward, out hit, float.MaxValue)) {
            if(hit.collider) {
                if(hit.distance <= range) {
                    target = hit.collider.transform;
                    targetEnemy = target.GetComponent<Enemy>();
                    targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
                    targetEnemy.Slow(slowFactor);

                    if(!lineRenderer.enabled) {
                        lineRenderer.enabled = true;
                        impactEffect.Play();
                        impactLight.enabled = true;
                    }

                    lineRenderer.SetPosition(0, fireSpawn.position);
                    lineRenderer.SetPosition(1, hit.point);

                    Vector3 direction = fireSpawn.position - hit.point;
                    impactEffect.transform.position = target.position + direction.normalized;
                    impactEffect.transform.rotation = Quaternion.LookRotation(direction);
                }
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
        Vector2 screenPos = turretCam.WorldToViewportPoint(pivot.position);
        Vector2 mouseOnScreen = turretCam.ScreenToViewportPoint(Input.mousePosition);
        float angle = Mathf.Atan2(screenPos.y - mouseOnScreen.y, screenPos.x - mouseOnScreen.x) * Mathf.Rad2Deg;
        pivot.rotation = Quaternion.Euler(new Vector3(0, -angle * PlayerPrefs.GetFloat("Sensitivity", 5)));
        
        if(Input.mousePosition.x >= 50)
            pivot.Rotate(Vector3.down * manualTurnSpeed * Time.deltaTime);
        if(Input.mousePosition.x <= Screen.width - 50)
            pivot.Rotate(Vector3.down * manualTurnSpeed * Time.deltaTime);
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