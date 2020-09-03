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
        fireRate *= ugB.upgradeFactorX;
        damage = Mathf.CeilToInt(damage * ugB.upgradeFactorY);
    }

    public override void ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0) {
            specialActivated = true;
            specialBar.fillBar.fillAmount = 1; //fully filled, on cooldown
            StartCoroutine(BulletBurst());
        }
    }

    IEnumerator BulletBurst() {
        StartCoroutine(SpecialTime());

        GameObject tmp = projectilePrefab;
        projectilePrefab = specialPrefab;
        var saveFireRate = fireRate;
        fireRate *= 8.0f;

        yield return new WaitForSeconds(specialTime);

        fireRate = saveFireRate;
        projectilePrefab = tmp;
    }
}