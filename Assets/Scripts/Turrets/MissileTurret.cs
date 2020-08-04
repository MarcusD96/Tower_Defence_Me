﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTurret : ProjectileTurret {

    private int missileCount = 10;
    private List<Enemy> targetList = new List<Enemy>();

    void Awake() {
        missileTurret = this;
        projectileTurret = this;
    }

    new void Update() {
        base.Update();
        if(hasSpecial) {
            if(Input.GetMouseButtonDown(1)) {
                ActivateBarrage();
            }
        }

        if(specialBar.fillBar.fillAmount <= 0) {
            specialActivated = false;
        }
    }

    public override void ApplyUpgradeB() {  //fireRate++, damage++, explosionRad+1, penetration +1
        fireRate *= ugB.upgradeFactorX;
        damage = Mathf.CeilToInt(damage * ugB.upgradeFactorY);
        penetration += 2;
    }

    void ActivateBarrage() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0 && manual) {
            specialActivated = true;
            StartCoroutine(MissileBarrage());
        }
    }

    IEnumerator MissileBarrage() {
        StartCoroutine(SpecialTime(specialRate));
        FindAndSortEnemies();

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
                    break;
                }
                target = targetList[i].transform;
                AutoShoot();
                yield return new WaitForSeconds(0.1f);
            }
        }

        targetList = new List<Enemy>();
    }

    void FindAndSortEnemies() {
        foreach(var e in GameObject.FindGameObjectsWithTag(enemyTag)) {
            targetList.Add(e.GetComponent<Enemy>());
        }
        targetList.Sort((a, b) => { return Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)); });
    }
}