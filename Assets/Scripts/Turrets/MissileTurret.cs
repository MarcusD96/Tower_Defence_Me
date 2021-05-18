using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTurret : ProjectileTurret {

    public GameObject specialPrefab;

    public int specialMissileCount;
    private List<Enemy> targetList = new List<Enemy>();

    void Awake() {
        missileTurret = this;
        projectileTurret = this;
        maxFireRate = (fireRate + (ugB.upgradeFactorX * 6)) * manualFirerateMultiplier;
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
    }

    public override bool ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0 && CheckEnemiesInRange()) {
            specialActivated = true;
            //StartCoroutine(MissileBarrage());
            StartCoroutine(MissileMaelstrom());
            return true;
        }
        return false;
    }

    IEnumerator MissileBarrage() {
        GameObject tmp = projectilePrefab;
        projectilePrefab = specialPrefab;

        if(FindAndSortEnemies()) {

            StartCoroutine(SpecialTime());
            nextFire = 0;

            int timesRestarted = 1;
            for(int i = 0; i < specialMissileCount; i++) {

                int ii = i - (targetList.Count * (timesRestarted - 1));

                if(ii < targetList.Count) {
                    if(targetList[ii] != null) {
                        target = targetList[ii].transform;
                        damage = 2;
                        AutoShoot();
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

    IEnumerator MissileMaelstrom() {
        StartCoroutine(SpecialTime());
        var g = new GameObject("Missiles");
        print(g.transform.position);
        g.transform.parent = transform;
        g.transform.position = transform.position + Vector3.up;
        for(int i = 0; i < specialMissileCount; i++) {
            GameObject tmp = projectilePrefab;
            projectilePrefab = specialPrefab;

            Missile[] missiles = new Missile[3];
            missiles[0] = Instantiate(projectilePrefab, g.transform).GetComponent<Missile>();
            missiles[0].transform.rotation = Quaternion.Euler(0, 10 * i, 0);
            missiles[1] = Instantiate(projectilePrefab, g.transform).GetComponent<Missile>();
            missiles[1].transform.rotation = Quaternion.Euler(0, 10 * i + 120, 0);
            missiles[2] = Instantiate(projectilePrefab, g.transform).GetComponent<Missile>();
            missiles[2].transform.rotation = Quaternion.Euler(0, 10 * i + 240, 0);

            foreach(var m in missiles) {
                m.SetDamage(damage * 2, damage * 5);
                m.SetExplosion(penetration);
            }
            projectilePrefab = tmp;
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(g, 3.0f);
    }

    bool FindAndSortEnemies() {
        foreach(var e in WaveSpawner.GetEnemyList_Static()) {
            targetList.Add(e.GetComponent<Enemy>());
        }

        if(targetList.Count <= 0)
            return false;

        targetList.Sort((a, b) => {
            return b.distanceTravelled.CompareTo(a.distanceTravelled);
        });

        //targetList = Shuffle(targetList);

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