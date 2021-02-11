using System.Collections;
using UnityEngine;

public class BulletTurret : ProjectileTurret {

    public GameObject specialPrefab;
    public float specialTime = 1.0f;

    void Awake() {
        standardTurret = this;
        projectileTurret = this;
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

    public override void ApplyUpgradeB() {  //fireRate++, damage++
        fireRate += ugB.upgradeFactorX;
        damage += ugB.upgradeFactorY;
    }

    public override void ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0 && CheckEnemiesInRange()) {
            specialActivated = true;
            StartCoroutine(BulletBurst());
        }
    }

    IEnumerator BulletBurst() {
        var special = SpecialTime();
        StartCoroutine(special);
        GameObject tmp = projectilePrefab;
        projectilePrefab = specialPrefab;
        var saveFireRate = fireRate;
        nextFire = 0;
        fireRate *= 3.0f;

        yield return new WaitForSeconds(specialTime);

        fireRate = saveFireRate;
        projectilePrefab = tmp;
    }
}