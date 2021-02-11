using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour {
    #region Variables
    public Transform fireSpawn;

    public bool manual = false;
    protected Transform target;
    protected Enemy targetEnemy;
    protected bool hasSpecial = false;
    protected bool specialActivated = false; //to restrain from activating again until the special bar is recharged
    [HideInInspector]
    public int sellPrice, sellPecent = 75;
    #endregion

    #region Headers
    [Header("Global")]
    public float range;
    public float manualRangeMultiplier = 1.75f;
    public Camera turretCam;
    private Camera mainCam;
    public GameObject turretView;
    public Image reloadedIndicator;
    public AimIndicator aimIndicator;

    [Header("Setup")]
    public Transform pivot;
    public string enemyTag = "Enemy";
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
        specialBar.fillBar.fillAmount = 0;
        specialBar.gameObject.SetActive(false);
        targettingMethod = 0;
        aimIndicator.SetTurret(this);
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

    List<GameObject> enemiesInRange;
    protected void FindFirstTargetInRange() {
        //find all enemies
        GameObject[] enemies = WaveSpawner.GetEnemyList_Static().ToArray();
        enemiesInRange = new List<GameObject>();

        //filter found enemies in range
        foreach(var e in enemies) {
            float distance = Vector3.Distance(pivot.position, e.transform.position);
            if(distance <= range) {
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
        GameObject[] enemies = WaveSpawner.GetEnemyList_Static().ToArray();
        enemiesInRange = new List<GameObject>();

        //filter found enemies in range
        foreach(var e in enemies) {
            float distance = Vector3.Distance(transform.position, e.transform.position);
            if(distance <= range) {
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

    protected void FindNearestTargetInRange() {
        GameObject[] enemies = WaveSpawner.GetEnemyList_Static().ToArray();
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

    protected void FindFurthestTargetInRange() {
        //find all enemies
        GameObject[] enemies = WaveSpawner.GetEnemyList_Static().ToArray();
        enemiesInRange = new List<GameObject>();

        //filter found enemies in range
        foreach(var e in enemies) {
            float distance = Vector3.Distance(transform.position, e.transform.position);
            if(distance <= range) {
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

    public int GetSellPrice() {
        return Mathf.RoundToInt((sellPrice * sellPecent / 100) / 5) * 5; //rounds to nearest 5 value
    }

    protected void RotateOnShoot() {
        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 euler = rotation.eulerAngles;
        pivot.rotation = Quaternion.Euler(0, euler.y, 0);
    }

    void AutomaticControl() {
        if(nextFire > 0.0f) {
            nextFire -= Time.fixedDeltaTime * Time.timeScale;
        }

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

        } else {
            beamTurret.AutoShoot();
        }
    }

    void ManualControl() {
        ManualMovement();

        if(nextFire >= 0.0f) {
            nextFire -= Time.fixedDeltaTime * Time.timeScale;
        }

        var manualFireRate = fireRate * 1.5f;

        if(beamTurret) {
            if(Input.GetMouseButton(0)) {
                beamTurret.ManualShoot();
            } else {
                beamTurret.LaserOff();
            }
        } else {
            if(Input.GetMouseButton(0)) {
                if(nextFire <= 0.0f) {
                    projectileTurret.ManualShoot();
                    nextFire = 1 / manualFireRate;
                }
            }
        }
        reloadedIndicator.fillAmount = nextFire / (1 / manualFireRate);
    }

    void ManualMovement() {

        float mouseInput = Input.GetAxis("Mouse X");
        Vector3 lookhere = new Vector3(0, mouseInput * Time.timeScale * 0.8f, 0); //apply sensitivity in settings later
        pivot.Rotate(lookhere);
    }

    public void ApplyUpgradeA() {
        range += ugA.upgradeFactorX;
        aimIndicator.SetPositionAtRange();
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
                fillTime -= Time.fixedDeltaTime * Time.timeScale;
                specialBar.fillBar.fillAmount = fillTime / specialRate;
            }
            yield return null;
        }
    }

    protected bool CheckEnemiesInRange() {
        GameObject tmpEnemy = null;
        foreach(var e in WaveSpawner.GetEnemyList_Static()) {
            if(!manual) {
                if(Vector3.Distance(pivot.position, e.transform.position) <= range) {
                    tmpEnemy = e;
                    break;
                }
            } else {
                if(Vector3.Distance(transform.position, e.transform.position) <= range * manualRangeMultiplier) {
                    tmpEnemy = e;
                    break;
                }
            }
        }
        if(tmpEnemy != null)
            return true;
        else
            return false;
    }

    public void AssumeControl() {
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