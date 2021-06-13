
using System.Collections;
using UnityEngine;

public class FireTurret : Turret {
    [Header("Fire Turret")]
    public FireShot fireShot;
    public float burnDamage, burnInterval;
    public int numFire, numBurns;

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
        fComp.Initialize(ugB.GetLevel(), numFire, range, burnDamage, burnInterval, numBurns, isUsingSpecial);
    }

    public void ManualShoot() {
        var f = ObjectPool.instance.ActivateProjectile(ProjectileType.FireShot, fireSpawn.position, fireShot.transform.rotation);
        var fComp = f.GetComponent<FireShot>();
        fComp.Initialize(ugB.GetLevel(), numFire, range, burnDamage, burnInterval, numBurns, isUsingSpecial);
    }

    public override void ApplyUpgradeB() {  //fireRate++, numFire*=
        fireRate += ugB.upgradeFactorX * ugB.GetLevel();
        numFire = Mathf.FloorToInt(numFire * ugB.upgradeFactorY);
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

        range *= 2.0f;
        thisNode.UpdateRange(this);

        var tmpPos = turretCam.transform.localPosition;
        turretCam.transform.localPosition *= 3;

        yield return new WaitForSeconds(specialTime);

        range = startRange + (ugA.upgradeFactorX * ugA.GetLevel());
        thisNode.UpdateRange(this);

        turretCam.transform.localPosition = tmpPos;

        isUsingSpecial = false;
    }
}
