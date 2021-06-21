using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTurret : ProjectileTurret {

    public float explosionRadius;
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

    public override void ApplyUpgradeB() {  //fireRate++, penetration++
        fireRate += ugB.upgradeFactorX;
        penetration += (int) (ugB.upgradeFactorY * ugB.GetLevel());
        explosionRadius += 2.5f;
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

    IEnumerator SpecialAbility() {
        StartCoroutine(SpecialTime());
        float dt = Time.deltaTime;
        float ff = FindObjectOfType<GameManager>().fastForward * dt;
        Vector3 spawn;
        spawn = new Vector3(transform.position.x, fireSpawn.position.y, transform.position.z);

        projectileType = WeaponType.Missile_Special;
        for(int i = 0; i < specialMissileCount; i++) {
            Missile[] m = new Missile[3];
            m[0] = ObjectPool.instance.ActivateProjectile(projectileType, spawn, Quaternion.Euler(new Vector3(0, 10 * i, 0))).GetComponent<Missile>();
            m[1] = ObjectPool.instance.ActivateProjectile(projectileType, spawn, Quaternion.Euler(new Vector3(0, (10 * i) + 120, 0))).GetComponent<Missile>();
            m[2] = ObjectPool.instance.ActivateProjectile(projectileType, spawn, Quaternion.Euler(new Vector3(0, (10 * i) + 240, 0))).GetComponent<Missile>();

            foreach(var mm in m) {
                mm.SetStats(damage / 2, bossDamage / 2, spawn, transform.forward * 60);
                mm.SetExplosion(penetration, 5);
            }
            if(Time.timeScale > 1) {
                yield return new WaitForSeconds(dt);
            } else
                yield return new WaitForSeconds(ff);
        }
        projectileType = WeaponType.Missile;
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