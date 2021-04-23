
using System.Collections;
using UnityEngine;

public class FireTurret : Turret {
    [Header("Fire Turret")]
    public FireShot fireShot;
    public float damage, burnDamage, burnInterval;
    public int penetration, numBurns, specialTime;

    void Awake() {
        fireTurret = this;
        maxFireRate = (fireRate + (ugB.upgradeFactorX * 3)) * manualFirerateMultiplier;
    }

    private new void Update() {
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

    public void AutoShoot() {
        var f = Instantiate(fireShot.gameObject, fireSpawn.position, fireShot.transform.rotation);
        f.GetComponent<FireShot>().SetStats(damage, range, burnDamage, burnInterval, numBurns, penetration, isUsingSpecial);
    }

    public void ManualShoot() {
        var f = Instantiate(fireShot.gameObject, fireSpawn.position, fireShot.transform.rotation);
        f.GetComponent<FireShot>().SetStats(damage, range, burnDamage, burnInterval, numBurns, penetration, isUsingSpecial);
    }
    public override void ApplyUpgradeB() {  //fireRate++, penetration++, burn++,
        fireRate += ugB.upgradeFactorX;
        penetration += (int) ugB.upgradeFactorY;
        numBurns += 2;
    }

    public override bool ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0 && CheckEnemiesInRange()) {
            StartCoroutine(Inferno());
            specialActivated = true;
            return true;
        }
        return false;
    }

    bool isUsingSpecial = false;
    IEnumerator Inferno() {
        StartCoroutine(SpecialTime());
        isUsingSpecial = true;

        range *= 1.5f;
        thisNode.UpdateRange(this);

        var tmpPos = turretCam.transform.localPosition;
        turretCam.transform.localPosition *= 2;

        penetration *= 2;

        yield return new WaitForSeconds(specialTime);

        range = startRange + (ugA.upgradeFactorX * ugA.GetLevel());
        thisNode.UpdateRange(this);

        turretCam.transform.localPosition = tmpPos;

        penetration /= 2;

        isUsingSpecial = false;
    }
}
