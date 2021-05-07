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
        maxFireRate = (fireRate + (ugB.upgradeFactorX * 3)) * manualFirerateMultiplier;
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
        fireRate += ugB.upgradeFactorX * ugB.GetLevel();
        penetration += (int) ugB.upgradeFactorY;
        explosionRadius += 3;
    }

    public override bool ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0 && CheckEnemiesInRange()) {
            specialActivated = true;
            StartCoroutine(MissileBarrage());
            return true;
        }
        return false;
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
                        recoilAnim_Cam.SetTrigger("Shoot");
                        AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);
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
            targetList.Add(e.GetComponent<Enemy>());
        }

        if(targetList.Count <= 0)
            return false;

        targetList.Sort((a, b) => {
            return Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)); 
        });

        targetList = Shuffle(targetList);

        return true;
    }

    List<Enemy> Shuffle(List<Enemy> list) {
        int n = list.Count;
        System.Random rng = new System.Random();
        while(n > 1) {
            n--;
            int k = rng.Next(n + 1);
            Enemy value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
}