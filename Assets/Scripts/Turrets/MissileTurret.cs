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
    }

    public override void ApplyUpgradeB() {  //fireRate++, penetration++, expl.rad. + 5
        fireRate += ugB.upgradeFactorX * ugB.GetLevel();
        penetration += (int) ugB.upgradeFactorY;
    }

    public override bool ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0 && CheckEnemiesInRange()) {
            specialActivated = true;
            //StartCoroutine(MissileBarrage());
            StartCoroutine(SpecialAbility());
            return true;
        }
        return false;
    }

    //old special
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

    IEnumerator SpecialAbility() {
        StartCoroutine(SpecialTime());
        Vector3 spawn;
        spawn = new Vector3(transform.position.x, fireSpawn.position.y, transform.position.z);
        for(int i = 0; i < specialMissileCount; i++) {
            Missile[] m = new Missile[3];
            m[0] = ObjectPool.instance.ActivateProjectile(ProjectileType.Missile_Special, spawn, Quaternion.Euler(new Vector3(0, 5 * i, 0))).GetComponent<Missile>();
            m[1] = ObjectPool.instance.ActivateProjectile(ProjectileType.Missile_Special, spawn, Quaternion.Euler(new Vector3(0, 5 * i + 120, 0))).GetComponent<Missile>();
            m[2] = ObjectPool.instance.ActivateProjectile(ProjectileType.Missile_Special, spawn, Quaternion.Euler(new Vector3(0, 5 * i + 240, 0))).GetComponent<Missile>();

            foreach(var mm in m) {
                mm.SetStats(damage * 2, bossDamage * 3, spawn, transform.forward * 60);
                mm.SetExplosion(penetration);
            }

            yield return new WaitForSeconds(0.005f);
        }
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
        return true;
    }

    //old
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