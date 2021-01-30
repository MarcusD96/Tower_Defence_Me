using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTurret : ProjectileTurret {

    public GameObject specialPrefab;

    private int missileCount = 10;
    private List<Enemy> targetList = new List<Enemy>();

    void Awake() {
        missileTurret = this;
        projectileTurret = this;
    }

    new void Update() {
        base.Update();
        if(hasSpecial && manual) {
            if(Input.GetMouseButtonDown(1)) {
                ActivateSpecial();
            }
        }

        if(specialBar.fillBar.fillAmount <= 0) {
            specialActivated = false;
        }
    }

    public override void ApplyUpgradeB() {  //fireRate++, damage++, penetration +1
        fireRate += ugB.upgradeFactorX;
        penetration += (int) ugB.upgradeFactorY;
        explosionRadius += (int) ugB.upgradeFactorY * 3;
    }

    public override void ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0) {
            specialActivated = true;
            StartCoroutine(MissileBarrage());
        }
    }

    IEnumerator MissileBarrage() {
        StartCoroutine(SpecialTime());
        nextFire = 0;
        FindAndSortEnemies();
        GameObject tmp = projectilePrefab;
        projectilePrefab = specialPrefab;
        if(targetList.Count < missileCount) {
            for(int i = 0; i < targetList.Count; i++) {
                if(!targetList[i]) {
                    break;
                }
                target = targetList[i].transform;
                AutoShoot();
                yield return new WaitForSeconds(0.1f);
            }
        } else {
            for(int i = 0; i < missileCount; i++) {
                if(!targetList[i]) {
                    i = 1;
                }
                target = targetList[i].transform;
                AutoShoot();
                yield return new WaitForSeconds(0.1f);
            }
        }

        projectilePrefab = tmp;
        targetList = new List<Enemy>();
    }

    void FindAndSortEnemies() {
        foreach(var e in GameObject.FindGameObjectsWithTag(enemyTag)) {
            targetList.Add(e.GetComponent<Enemy>());
        }
        targetList.Sort((a, b) => { return Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)); });
    }
}
