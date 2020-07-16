using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTurret : ProjectileTurret {
    int missileCount = 10;
    List<Enemy> targetList = new List<Enemy>();

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
    }

    public override void ApplyUpgradeA() { //range++
        range *= ugA.upgradeFactorX;
    }

    public override void ApplyUpgradeB() {  //fireRate++, damage++, explosionRad+1, penetration +1
        fireRate *= ugB.upgradeFactorX;
        damage = Mathf.CeilToInt(damage * ugB.upgradeFactorY);
        explosionRadius += 1;
        penetration += 1;
    }

    void ActivateBarrage() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0) {
            specialActivated = true;
            StartCoroutine(MissileBarrage());
        }
    }

    IEnumerator MissileBarrage() {
        FindAndSortEnemies();

        if(targetList.Count < missileCount) {
            for(int i = 0; i < targetList.Count; i++) {
                if(!targetList[i]) {
                    break;
                }
                target = targetList[i].transform;
                AutoShoot();
                yield return new WaitForSeconds(0.1f);
                //yield return new WaitForSeconds(0.01f);
            }
        } else {
            for(int i = 0; i < missileCount; i++) {
                target = targetList[i].transform;
                AutoShoot();
                yield return new WaitForSeconds(0.1f);
                //yield return new WaitForSeconds(0.01f);
            }
        }

        targetList = new List<Enemy>();
        specialActivated = false;
    }

    void FindAndSortEnemies() {
        foreach(var e in GameObject.FindGameObjectsWithTag(enemyTag)) {
            targetList.Add(e.GetComponent<Enemy>());
        }
        targetList.Sort((a, b) => { return Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)); });
    }
}
