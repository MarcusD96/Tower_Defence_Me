
using System.Collections;
using UnityEngine;

public class RailgunTurret : ProjectileTurret {

    public GameObject specialPrefab;

    void Awake() {
        projectileTurret = this;
        railgunTurret = this;
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

    public override void ApplyUpgradeB() { //fireRate++, penetration++;
        fireRate += ugB.upgradeFactorX;
        penetration += (int) ugB.upgradeFactorY;
    }

    public override void ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0 && CheckEnemiesInRange()) {
            specialActivated = true;
            StartCoroutine(AttachCharges());
        }
    }

    IEnumerator AttachCharges() {
        StartCoroutine(SpecialTime());

        GameObject tmp = projectilePrefab;
        projectilePrefab = specialPrefab;
        nextFire = 0; //reset fire and fire special right away

        yield return new WaitForSeconds(5.0f);

        projectilePrefab = tmp;
    }
}
