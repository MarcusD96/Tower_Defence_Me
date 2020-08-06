using UnityEngine;
using System.Collections;

public class BulletTurret : ProjectileTurret {
    public float specialTime = 1.0f;

    void Awake() {
        standardTurret = this;
        projectileTurret = this;
    }

    new void Update() {
        base.Update();
        if(hasSpecial) {
            if(Input.GetMouseButtonDown(1)) {
                ActivateBurst();
            }
        }

        if(specialBar.fillBar.fillAmount <= 0) {
            specialActivated = false;
        }
    }

    public override void ApplyUpgradeB() {  //fireRate++, damage++
        fireRate *= ugB.upgradeFactorX;
        damage = Mathf.CeilToInt(damage * ugB.upgradeFactorY);
    }

    void ActivateBurst() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0 && manual) {
            specialActivated = true;
            specialBar.fillBar.fillAmount = 1; //fully filled, on cooldown
            StartCoroutine(BulletBurst());
        }
    }

    IEnumerator BulletBurst() {
        StartCoroutine(SpecialTime(specialRate));
        var saveFireRate = fireRate;
        fireRate *= 10.0f;
        yield return new WaitForSeconds(specialTime);
        fireRate = saveFireRate;
    }
}