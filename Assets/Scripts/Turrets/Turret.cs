using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour {
    #region Variables
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
    public Transform fireSpawn;
    public float range;
    public float manualRangeMultiplier, manualFirerateMultiplier;
    public float fireRate; //shots per second, higher is faster

    [HideInInspector]
    public float maxFireRate;

    public Camera turretCam, overlayCam;
    public GameObject turretView;
    public Image reloadIndicator;
    public string shootSound;
    public AimIndicator aimIndicator;

    private Camera mainCam;

    [Header("Setup")]
    public Transform pivot;
    public Animator gfxAnim, recoilAnim;
    public ParticleSystem muzzleFlash;

    protected float startRange;
    protected string enemyTag = "Enemy";
    protected string shootAnim = "Shoot";
    protected Node thisNode;
    protected BeamTurret beamTurret;
    protected ProjectileTurret projectileTurret;
    protected FireTurret fireTurret;

    [Header("Upgrades")]
    public Upgrade ugA;
    public Upgrade ugB, ugSpec;

    [Header("Projectiles")]
    protected float nextFire = 0.0f;

    [Header("Special")]
    public SpecialBar specialBar;
    public float specialRate;

    private string targetting;
    private int targettingMethod;
    #endregion

    void Start() {
        mainCam = Camera.main;
        turretCam.enabled = false;
        overlayCam.enabled = false;
        turretView.SetActive(false);
        specialBar.fillBar.fillAmount = 0;
        specialBar.gameObject.SetActive(false);
        targettingMethod = 0;
        startRange = range;
        UpdateTargettingName();
        if(aimIndicator) {
            aimIndicator.SetTurret(this);
        }
        gfxAnim = GetComponentInChildren<Animator>();
        maxFireRate = (fireRate + (ugB.upgradeFactorX * 3)) * manualFirerateMultiplier;
    }

    protected void Update() {
        if(manual) {
            ManualControl();
            CameraManager.UpdateCam(turretCam);
        } else {
            AutomaticControl();
        }
    }

    public void AttachNode(Node n) {
        thisNode = n;
    }

    protected bool FindEnemy() {
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
        if(target == null)
            return false;
        return true;
    }

    public string ChangeTargetting(bool next) {
        if(next)
            targettingMethod++;
        else
            targettingMethod--;

        if(targettingMethod > 3) {
            targettingMethod = 0;
        }
        if(targettingMethod < 0)
            targettingMethod = 3;

        UpdateTargettingName();
        return targetting;
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

    public string GetTargetting() {
        return targetting;
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

    public Transform GetTarget() {
        return target;
    }

    protected void RotateOnShoot() {
        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 euler = rotation.eulerAngles;
        pivot.rotation = Quaternion.Euler(0, euler.y, 0);
    }

    void AutomaticControl() {
        nextFire -= Time.deltaTime;

        if(projectileTurret) {
            if(nextFire <= 0.0f) {
                FindEnemy();
                if(target == null)
                    return;
                RotateOnShoot();
                projectileTurret.AutoShoot();
                AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);
                gfxAnim.SetTrigger(shootAnim);
                muzzleFlash.Play();
                nextFire = 1 / fireRate;
            }
        } else if(fireTurret) {
            if(nextFire <= 0.0f) {
                FindEnemy();
                if(target == null)
                    return;
                fireTurret.AutoShoot();
                AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);
                gfxAnim.SetTrigger(shootAnim);
                nextFire = 1 / fireRate;
            }
        } else if(beamTurret) {
            beamTurret.AutoShoot();
        } else
            Debug.LogError("non existant tower???");
    }

    void ManualControl() {
        nextFire -= Time.deltaTime;

        var manualFireRate = fireRate;
        if(!hasSpecial) {
            manualFireRate = fireRate * manualFirerateMultiplier;
        }

        if(projectileTurret) {
            if(Input.GetMouseButton(0)) {
                if(nextFire <= 0.0f) {
                    projectileTurret.ManualShoot();
                    AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);
                    gfxAnim.SetTrigger(shootAnim);
                    recoilAnim.SetTrigger("Shoot");
                    muzzleFlash.Play();
                    nextFire = 1 / manualFireRate;
                }
            }
        } else if(fireTurret) {
            if(Input.GetMouseButton(0)) {
                if(nextFire <= 0.0f) {
                    fireTurret.ManualShoot();
                    AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);
                    gfxAnim.SetTrigger(shootAnim);
                    recoilAnim.SetTrigger("Shoot");
                    nextFire = 1 / manualFireRate;
                }
            }
        } else if(beamTurret) {
            if(Input.GetMouseButton(0)) {
                beamTurret.ManualShoot();
            } else {
                beamTurret.LaserOff();
            }
        } else
            Debug.LogError("non existant tower???");

        reloadIndicator.fillAmount = nextFire / (1 / manualFireRate);
    }

    public void ApplyUpgradeA() {
        range += ugA.upgradeFactorX;
        if(aimIndicator) {
            aimIndicator.SetPositionAtRange();
        }
    }

    public virtual void ApplyUpgradeB() {
        Debug.Log("upgrade 2");
    }

    public void EnableSpecial() {
        fireRate = maxFireRate;
        hasSpecial = true;
        specialBar.gameObject.SetActive(true);
    }

    public virtual bool ActivateSpecial() {
        Debug.Log("Activate Special");
        return true;
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

    protected bool CheckEnemiesInRange() {
        if(manual)
            return true;

        GameObject tmpEnemy = null;
        foreach(var e in WaveSpawner.GetEnemyList_Static()) {
            if(!manual) {
                if(Vector3.Distance(pivot.position, e.transform.position) <= range) {
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
        turretCam.enabled = overlayCam.enabled = true;
        turretView.SetActive(true);
        CameraManager.UpdateCam(turretCam);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        manual = true;
    }

    public void RevertControl(bool roundEnd) {
        turretCam.enabled = overlayCam.enabled = false;
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