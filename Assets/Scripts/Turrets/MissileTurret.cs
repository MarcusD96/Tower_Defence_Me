using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTurret : ProjectileTurret {

    public GameObject specialPrefab;

    private int missileCount = 20;
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

    public override void ApplyUpgradeB() {  //fireRate++, penetration++, expl.rad. + 5
        fireRate += ugB.upgradeFactorX;
        penetration += (int) ugB.upgradeFactorY;
        explosionRadius += 3;
    }

    public override void ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0) {
            specialActivated = true;
            StartCoroutine(MissileBarrage());
        }
    }

    IEnumerator MissileBarrage() {
        GameObject tmp = projectilePrefab;
        projectilePrefab = specialPrefab;

        if(FindAndSortEnemies()) {

            IEnumerator special = SpecialTime();
            StartCoroutine(special);
            nextFire = 0;

            int timesRestarted = 1;
            for(int i = 0; i < missileCount; i++) {

                int ii = i - (targetList.Count * (timesRestarted - 1));

                if(ii < targetList.Count) {
                    if(targetList[ii] != null) {
                        target = targetList[ii].transform;
                        damage = 2;
                        AutoShoot();
                        damage = 1;
                        yield return new WaitForSeconds(0.1f);
                    }
                }

                if(targetList.Count == 1) {
                    timesRestarted++;
                } else if(i % targetList.Count == 0) {
                    if(i != 0)
                        timesRestarted++;
                }
            }
        }

        projectilePrefab = tmp;
        targetList = new List<Enemy>();
        yield return null;
    }

    bool FindAndSortEnemies() {
        foreach(var e in WaveSpawner.GetEnemyList_Static()) {
            if(Vector3.Distance(e.gameObject.transform.position, transform.position) >= range) { //check to see if any targets are in range
                continue;
            }
            targetList.Add(e.GetComponent<Enemy>());
        }

        if(targetList.Count <= 0)
            return false;

        targetList.Sort((a, b) => {
            return Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)); 
        });

        return true;
    }
}