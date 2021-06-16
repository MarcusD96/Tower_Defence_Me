using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTurret : ParticleTurret {


    [Header("Wind Turret")]
    public LayerMask enemyLayer;
    [Range(0, 1)]
    public float chanceToBlowback;
    public float blowBackDuration, damage;
    public int penetration;
    public WindWave special;


    private void Awake() {
        particleTurret = this;
        windTurret = this;
        maxFireRate = fireRate * 1.2f;
    }

    private new void Update() {
        base.Update();
        if(hasSpecial && manual) {
            if(Input.GetMouseButtonDown(1)) {
                ActivateSpecial();
            }
        }
    }

    WindShot windShotComp;
    public override void Shoot() {
        if(!manual) {
            RotateOnShoot();
        }
        currentShot = ObjectPool.instance.ActivateProjectile(shotPrefab.weapon, fireSpawn.position, pivot.rotation);
        windShotComp = currentShot.GetComponent<WindShot>();
        windShotComp.Initialize(ugA.GetLevel(), ugB.GetLevel(), range, damage, penetration, chanceToBlowback, blowBackDuration);
    }

    public override void ApplyUpgradeB() {
        damage += ugB.upgradeFactorX;
        chanceToBlowback += ugB.upgradeFactorY * ugB.GetLevel();
        blowBackDuration += 0.25f;
        penetration += 5;
    }

    public override bool ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0) {
            StartCoroutine(SpecialAbility());
            specialActivated = true;
            return true;
        }
        return false;
    }

    IEnumerator SpecialAbility() {
        StartCoroutine(SpecialTime());
        float saveFireRate = fireRate;
        nextFire = float.MaxValue;
        fireRate = 0;
        yield return new WaitForSeconds(5.0f);

        WindWave tmp = Instantiate(special, pivot.position, special.transform.rotation);
        tmp.Initialize(range * 2, damage * 2, blowBackDuration * 2, enemyLayer);
        yield return new WaitForSeconds(0.5f);

        nextFire = 0;
        fireRate = saveFireRate;
    }
}
