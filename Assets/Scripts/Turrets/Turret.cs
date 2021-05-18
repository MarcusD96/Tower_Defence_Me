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
    [SerializeField]
    protected bool specialActivated = false; //to restrain from activating again until the special bar is recharged
    [HideInInspector]
    public int sellPrice, sellPecent = 75;
    #endregion

    #region Headers
    [Header("Global")]
    public Transform fireSpawn;
    public float range;

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
    public Animator recoilAnim_Body;
    public ParticleSystem muzzleFlash;

    protected float startRange;
    protected string enemyTag = "Enemy";
    protected string shootAnim = "Shoot";
    protected Node thisNode;
    protected BeamTurret beamTurret;
    protected ProjectileTurret projectileTurret;
    protected FireTurret fireTurret;
    protected FarmTower farmTower;

    [Header("Upgrades")]
    public Upgrade ugA;
    public Upgrade ugB, ugSpec;

    [Header("Weapon Stats")]
    public float manualRangeMultiplier, manualFirerateMultiplier;
    public float fireRate; //shots per second, higher is faster
    protected float nextFire = 0.0f;

    [Header("Special")]
    public SpecialBar specialBar;
    public float specialRate;

    private string targetting;
    private int targettingMethod;
    #endregion

    protected void Start() {
        mainCam = Camera.main;
        turretCam.enabled = false;
        overlayCam.enabled = false;
        turretView.SetActive(false);
        specialBar.fillBar.fillAmount = 0;
        specialBar.gameObject.SetActive(false);
        targettingMethod = 0;
        startRange = range;
        UpdateTargettingName();
        if(aimIndicator)
            aimIndicator.SetTurret(this);
        if(muzzleFlash)
            muzzleFlash.Stop();
        recoilAnim_Body = GetComponentInChildren<Animator>();
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

    #region Targetting
    protected bool FindEnemy(bool global) {
        switch(targettingMethod) {
            case 0:         //first
                FindFirstTarget(global);
                break;
            case 1:         //last
                FindLastTarget(global);
                break;
            case 2:         //close
                FindNearestTarget();
                break;
            case 3:         //strong
                FindStrongestTarget(global);
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
                targetting = "Strong";
                break;

            default:
                break;
        }
    }

    public string GetTargetting() {
        return targetting;
    }

    List<GameObject> FilterEnemiesInRange(bool global) {
        List<GameObject> enemiesInRange = new List<GameObject>();

        //only see enemies in range
        if(global == false) {
            foreach(var e in WaveSpawner.GetEnemyList_Static()) {
                float d = Vector3.Distance(pivot.position, e.transform.position);
                if(d <= range)
                    enemiesInRange.Add(e);
            }
        } else { //see all enemies
            enemiesInRange = WaveSpawner.GetEnemyList_Static();
        }
        return enemiesInRange;
    }

    protected void FindFirstTarget(bool global) {
        //find the enemy with the greatest distanceTravelled variable
        float greatestDistanceTravelled = 0;
        GameObject firstEnemy = null;

        List<GameObject> eList = FilterEnemiesInRange(global);
        if(eList.Count < 1) {
            target = null;
            return;
        }

        foreach(var e in eList) {
            float time = e.GetComponent<Enemy>().distanceTravelled;
            if(time > greatestDistanceTravelled) {
                greatestDistanceTravelled = time;
                firstEnemy = e;
            }
        }

        //target the found enemy
        if(firstEnemy != null) {
            target = firstEnemy.transform;
            targetEnemy = firstEnemy.GetComponent<Enemy>();
        } else
            target = null;
    }

    protected void FindLastTarget(bool global) {
        //find the enemy with the least distanceTrvelled variable
        float leastDistanceTravelled = float.MaxValue;
        GameObject firstEnemy = null;

        List<GameObject> eList = FilterEnemiesInRange(global);
        if(eList.Count < 1) {
            target = null;
            return;
        }

        foreach(var e in eList) {
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

    protected void FindNearestTarget() {
        float shortestDistance = float.MaxValue;
        GameObject nearestEnemy = null;

        List<GameObject> eList = FilterEnemiesInRange(false);
        if(eList.Count < 1) {
            target = null;
            return;
        }

        foreach(var e in eList) {
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

    protected void FindStrongestTarget(bool global) {
        GameObject strongestEnemy = null;

        //get enemies in range
        //find the highest tier
        int rank = -1;
        List<GameObject> eList = FilterEnemiesInRange(global);
        if(eList.Count < 1) {
            target = null;
            return;
        }

        foreach(var enemy in eList) {
            Enemy e = enemy.GetComponent<Enemy>();
            int r = (int) e.enemyType;
            if(r > rank) {
                rank = r;
            }
        }

        //filter list to only enemies with highest rank
        for(int i = eList.Count - 1; i >= 0; i--) {
            if((int) eList[i].GetComponent<Enemy>().enemyType != rank) {
                eList.RemoveAt(i);
            }
        }

        float greatestDistanceTravelled = 0;
        //find the strongest enemy which has travelled the furthest
        foreach(var enemy in eList) {
            Enemy e = enemy.GetComponent<Enemy>();
            if(e.distanceTravelled > greatestDistanceTravelled) {
                greatestDistanceTravelled = e.distanceTravelled;
                strongestEnemy = enemy;
            }
        }

        //target that enemy if one was found
        if(strongestEnemy) {
            target = strongestEnemy.transform;
            targetEnemy = strongestEnemy.GetComponent<Enemy>();
        } else
            target = null;
    }

    protected bool CheckEnemiesInRange() {
        if(manual)
            return true;

        if(FilterEnemiesInRange(false).Count > 0) {
            return true;
        }

        return false;
    }
    #endregion

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
        nextFire = Mathf.Clamp(nextFire, 0, float.MaxValue);

        if(projectileTurret) {
            if(nextFire <= 0.0f && WaveSpawner.enemiesAlive > 0) {
                FindEnemy(false);
                if(target == null)
                    return;
                RotateOnShoot();
                projectileTurret.AutoShoot();
                AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);
                if(recoilAnim_Body) {
                    recoilAnim_Body.SetTrigger(shootAnim);
                }
                muzzleFlash.Play();
                nextFire = 1 / fireRate;
            }
        } else if(fireTurret) {
            if(nextFire <= 0.0f && WaveSpawner.enemiesAlive > 0) {
                FindEnemy(false);
                if(target == null)
                    return;
                fireTurret.AutoShoot();
                AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);
                if(recoilAnim_Body) {
                    recoilAnim_Body.SetTrigger(shootAnim);
                }
                nextFire = 1 / fireRate;
            }
        } else if(beamTurret) {
            if(WaveSpawner.enemiesAlive > 0) {
                beamTurret.AutoShoot();
            }
        } else
            Debug.LogError("non existant tower???");
    }

    void ManualControl() {
        nextFire -= Time.deltaTime;
        nextFire = Mathf.Clamp(nextFire, 0, float.MaxValue);

        var manualFireRate = fireRate;
        if(!hasSpecial) {
            manualFireRate = fireRate * manualFirerateMultiplier;
        }

        if(projectileTurret) {
            if(Input.GetMouseButton(0)) {
                if(nextFire <= 0.0f) {
                    projectileTurret.ManualShoot();
                    AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);
                    if(recoilAnim_Body) {
                        recoilAnim_Body.SetTrigger(shootAnim);
                    }
                    muzzleFlash.Play();
                    nextFire = 1 / manualFireRate;
                }
            }
        } else if(fireTurret) {
            if(Input.GetMouseButton(0)) {
                if(nextFire <= 0.0f) {
                    fireTurret.ManualShoot();
                    AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);
                    if(recoilAnim_Body) {
                        recoilAnim_Body.SetTrigger(shootAnim);
                    }
                    nextFire = 1 / manualFireRate;
                }
            }
        } else if(beamTurret) {
            if(Input.GetMouseButton(0)) {
                beamTurret.ManualShoot();
            } else {
                beamTurret.BeamOff();
            }
        } else
            Debug.LogError("non existant tower???");

        if(reloadIndicator != null) {
            reloadIndicator.fillAmount = nextFire / (1 / manualFireRate);
        }
    }

    #region Upgrades and Special
    public void ApplyUpgradeA() {
        if(farmTower != null) {
            farmTower.ApplyUpgradeA();
            return;
        }
        range += ugA.upgradeFactorX;
        if(aimIndicator != null) {
            aimIndicator.SetPositionAtRange();
        }
    }

    public virtual void ApplyUpgradeB() {
        Debug.Log("upgrade 2");
    }

    public void ApplySpecial() {
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
    #endregion

    public void AssumeControl() {
        if(farmTower)
            return;

        ChangeLayer(9);
        if(aimIndicator != null) {
            aimIndicator.SetPositionAtRange();
        }
        turretCam.enabled = overlayCam.enabled = true;
        turretView.SetActive(true);
        CameraManager.UpdateCam(turretCam);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        manual = true;
    }

    public void RevertControl(bool roundEnd) {
        if(farmTower)
            return;

        ChangeLayer(0);
        if(aimIndicator != null) {
            aimIndicator.SetPositionAtRange();
        }
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

    void ChangeLayer(int layerNum) {
        foreach(var c in gameObject.GetComponentsInChildren<Transform>(true)) {
            c.gameObject.layer = layerNum;
        }
    }
}