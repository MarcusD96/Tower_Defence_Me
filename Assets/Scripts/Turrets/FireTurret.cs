
using System.Collections;
using UnityEngine;

public class FireTurret : Turret {
    [Header("Fire Turret")]
    public FireShot fireShot;
    public float damage, burnDamage, burnInterval;
    public int penetration, numBurns;

    void Awake() {
        fireTurret = this;
        maxFireRate = (fireRate + (ugB.upgradeFactorX * 6)) * manualFirerateMultiplier;
    }

    private new void Update() {
        base.Update();
        if(hasSpecial && manual) {
            if(Input.GetMouseButtonDown(1)) {
                ActivateSpecial();
            }
        }
    }

    public void AutoShoot() {
        var f = ObjectPool.instance.ActivateProjectile(ProjectileType.FireShot, fireSpawn.position, fireShot.transform.rotation);
        var fComp = f.GetComponent<FireShot>();
        fComp.Initialize(damage, range, burnDamage, burnInterval, numBurns, penetration, isUsingSpecial);
    }

    public void ManualShoot() {
        var f = ObjectPool.instance.ActivateProjectile(ProjectileType.FireShot, fireSpawn.position, fireShot.transform.rotation);
        var fComp = f.GetComponent<FireShot>();
        fComp.Initialize(damage, range, burnDamage, burnInterval, numBurns, penetration, isUsingSpecial);
    }
    public override void ApplyUpgradeB() {  //fireRate++, penetration++, burn++,
        fireRate += ugB.upgradeFactorX * ugB.GetLevel();
        penetration += (int) ugB.upgradeFactorY;
        numBurns += 2;
    }

    public override bool ActivateSpecial() {
        range *= 3;
        if(!specialActivated && WaveSpawner.enemiesAlive > 0 && CheckEnemiesInRange()) {
            range /= 3;
            StartCoroutine(SpecialAbility());
            specialActivated = true;
            return true;
        }
        range /= 3;
        return false;
    }

    bool isUsingSpecial = false;
    IEnumerator SpecialAbility() {
        StartCoroutine(SpecialTime());
        isUsingSpecial = true;

        range *= 3.0f;
        thisNode.UpdateRange(this);

        var tmpPos = turretCam.transform.localPosition;
        turretCam.transform.localPosition *= 3;

        penetration *= 2;

        yield return new WaitForSeconds(specialTime);

        range = startRange + (ugA.upgradeFactorX * ugA.GetLevel());
        thisNode.UpdateRange(this);

        turretCam.transform.localPosition = tmpPos;

        penetration /= 2;

        isUsingSpecial = false;
    }
}
