using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {
    #region Variables
    public Transform fireSpawn;

    public bool manual = false;
    protected Transform target;
    protected Enemy targetEnemy;
    protected bool hasSpecial = false;
    protected bool specialActivated = false; //to restrain from activating again until the special bar is recharged
    [HideInInspector]
    public int sellPrice;
    #endregion

    #region Headers
    [Header("Global")]
    public float range = 15.0f;
    public Camera turretCam;
    private Camera mainCam;
    public GameObject turretView;
    public Enemy mockEnemy;

    [Header("Setup")]
    public Transform pivot;
    public string enemyTag = "Enemy";
    public float turnSpeed = 10.0f;
    public float manualTurnSpeed = 100.0f;
    protected BeamTurret beamTurret;
    protected ProjectileTurret projectileTurret;

    [Header("Upgrades")]
    public Upgrade ugA;
    public Upgrade ugB, ugSpec;

    [Header("Projectiles")]
    public float fireRate = 2.0f; //shots per second, higher is faster
    protected float nextFire = 0.0f;

    [Header("Special")]
    public SpecialBar specialBar;
    public float specialRate;

    [Header("Targetting")]
    public string targetting = "First";
    private int targettingMethod;
    #endregion

    void Start() {
        mainCam = Camera.main;
        turretCam.enabled = false;
        turretView.SetActive(false);
        mockEnemy.transform.position = new Vector3(fireSpawn.position.x, fireSpawn.position.y, transform.position.z + (range * 2.5f)); //********
        specialBar.fillBar.fillAmount = 0;
        specialBar.gameObject.SetActive(false);
        targettingMethod = 0;
    }

    protected void Update() {
        if(manual) {
            ManualControl();
            CameraManager.UpdateCam(turretCam);
        } else {
            AutomaticControl();
        }
    }

    void FindEnemy() {
        switch(targettingMethod) {
            case 0:         //first
                FindFirstTargetInRange();
                break;
            case 1:         //last
                FindLastTargetInRange();
                break;
            case 2:         //close
                FindNearestTargetInRange();
                break;
            case 3:         //far
                FindFurthestTargetInRange();
                break;
            default:
                break;
        }
    }

    public void NextTargettingOption() {
        targettingMethod++;
        if(targettingMethod > 3) {
            targettingMethod = 0;
        }
        UpdateTargettingName();
    }

    public void LastTargettingOption() {
        targettingMethod--;
        if(targettingMethod < 0) {
            targettingMethod = 3;
        }
        UpdateTargettingName();
    }

    void UpdateTargettingName() {
        switch(targettingMethod) {
            case 0:
                targetting = "First";
                break;

            case 1:
                targetting = "Last";
                break;

            case 2:
                targetting = "Close";
                break;

            case 3:
                targetting = "Far";
                break;

            default:
                break;
        }
    }

    protected void FindNearestTargetInRange() {
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

        if(nearestEnemy && shortestDistance <= range * 2.5f) {
            target = nearestEnemy.transform;
            targetEnemy = nearestEnemy.GetComponent<Enemy>();
        } else
            target = null;
    }

    protected void FindFurthestTargetInRange() {
        //find all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        List<GameObject> enemiesInRange = new List<GameObject>();

        //filter found enemies in range
        foreach(var e in enemies) {
            float distance = Vector3.Distance(transform.position, e.transform.position);
            if(distance <= range * 2.5f) {
                enemiesInRange.Add(e);
            }
        }

        //find the enemy with the greatest distance
        float longestDistance = 0;
        GameObject furthestEnemy = null;
        foreach(var e in enemiesInRange) {
            float distance = Vector3.Distance(transform.position, e.transform.position);
            if(distance > longestDistance) {
                longestDistance = distance;
                furthestEnemy = e;
            }
        }

        //target that enemy if one was found
        if(furthestEnemy) {
            target = furthestEnemy.transform;
            targetEnemy = furthestEnemy.GetComponent<Enemy>();
        } else
            target = null;
    }

    protected void FindFirstTargetInRange() {
        //find all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        List<GameObject> enemiesInRange = new List<GameObject>();

        //filter found enemies in range
        foreach(var e in enemies) {
            float distance = Vector3.Distance(transform.position, e.transform.position);
            if(distance <= range * 2.5f) {
                enemiesInRange.Add(e);
            }
        }

        //find the enemy with the greatest distanceTravelled variable
        float greatestDistanceTravelled = 0;
        GameObject firstEnemy = null;
        foreach(var e in enemiesInRange) {
            float time = e.GetComponent<Enemy>().distanceTravelled;
            if(time > greatestDistanceTravelled) {
                greatestDistanceTravelled = time;
                firstEnemy = e;
            }
        }

        //target the found enemy
        if(firstEnemy) {
            target = firstEnemy.transform;
            targetEnemy = firstEnemy.GetComponent<Enemy>();
        } else
            target = null;
    }

    protected void FindLastTargetInRange() {
        //find all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        List<GameObject> enemiesInRange = new List<GameObject>();

        //filter found enemies in range
        foreach(var e in enemies) {
            float distance = Vector3.Distance(transform.position, e.transform.position);
            if(distance <= range * 2.5f) {
                enemiesInRange.Add(e);
            }
        }

        //find the enemy with the least distanceTrvelled variable
        float leastDistanceTravelled = float.MaxValue;
        GameObject firstEnemy = null;
        foreach(var e in enemiesInRange) {
            float time = e.GetComponent<Enemy>().distanceTravelled;
            if(time < leastDistanceTravelled) {
                leastDistanceTravelled = time;
                firstEnemy = e;
            }
        }

        //target the found enemy
        if(firstEnemy) {
            target = firstEnemy.transform;
            targetEnemy = firstEnemy.GetComponent<Enemy>();
        } else
            target = null;
    }

    public int GetSellPrice() {
        return Mathf.RoundToInt((sellPrice / 2) / 5) * 5; //rounds to nearest 5 value
    }

    protected void RotateOnShoot() {
        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 euler = rotation.eulerAngles;
        pivot.rotation = Quaternion.Euler(0, euler.y, 0);
    }

    void AutomaticControl() {
        FindEnemy();
        if(!target) {
            if(beamTurret)
                beamTurret.LaserOff();
            return;
        }

        if(!beamTurret) {
            if(nextFire <= 0.0f) {
                RotateOnShoot();
                projectileTurret.AutoShoot();
                nextFire = 1 / fireRate;
            }

            nextFire -= Time.deltaTime;
        } else {
            beamTurret.AutoShoot();
        }
    }

    void ManualControl() {
        ManualMovement();

        if(beamTurret) {
            if(Input.GetMouseButton(0)) {
                beamTurret.ManualShoot();
            } else {
                beamTurret.LaserOff();
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

    public void ApplyUpgradeA() {
        range += ugA.upgradeFactorX;
    }

    public virtual void ApplyUpgradeB() {
        Debug.Log("upgrade 2");
    }

    public void EnableSpecial() {
        hasSpecial = true;
        specialBar.gameObject.SetActive(true);
    }

    public virtual void ActivateSpecial() {
        Debug.Log("Activate Special");
    }

    protected IEnumerator SpecialTime() {
        float fillTime = specialRate;
        while(fillTime > 0) {
            if(WaveSpawner.enemiesAlive > 0) {
                fillTime -= Time.deltaTime;
                specialBar.fillBar.fillAmount = fillTime / specialRate;
            }
            yield return null;
        }
    }

    public void AssumeControl() {
        GameManager.lastControlled = this;
        turretCam.enabled = true;
        turretView.SetActive(true);
        CameraManager.UpdateCam(turretCam);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        manual = true;
    }

    public void RevertControl(bool roundEnd) {

        turretCam.enabled = false;
        turretView.SetActive(false);
        mainCam.enabled = true;
        CameraManager.UpdateCam(mainCam);

        if(!roundEnd)
            GameManager.lastControlled = null;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        manual = false;
    }
}