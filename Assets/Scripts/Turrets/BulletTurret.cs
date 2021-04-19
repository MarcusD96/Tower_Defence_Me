using System.Collections;
using UnityEngine;

public class BulletTurret : ProjectileTurret {

    public GameObject specialPrefab;
    public float specialTime, specialFireRate;

    void Awake() {
        projectileTurret = this;
        bulletTurret = this;
        maxFireRate = (fireRate + (ugB.upgradeFactorX * 3)) * manualFirerateMultiplier;
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

    public override void ApplyUpgradeB() {  //fireRate++, bossDamage++
        fireRate += ugB.upgradeFactorX;
        bossDamage += (int) ugB.upgradeFactorY;
    }

    public override bool ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0 && CheckEnemiesInRange()) {
            specialActivated = true;
            StartCoroutine(BulletBurst());
            return true;
        }
        return false;
    }

    IEnumerator BulletBurst() {
        var special = SpecialTime();
        StartCoroutine(special);
        GameObject tmp = projectilePrefab;

        var saveFireRate = fireRate;
        fireRate = 0;
        var tmpCamPos = turretCam.transform.localPosition;

        //zoom camera in slowly
        float endTime = Time.time + 1.5f;
        for(float i = Time.time; i <= endTime; i += Time.deltaTime) {
            var pos = turretCam.transform.localPosition;
            pos.z = Mathf.Lerp(pos.z, pos.z + 3, Time.deltaTime);
            turretCam.transform.localPosition = pos;
            yield return null;
        }

        nextFire = 0;
        projectilePrefab = specialPrefab;
        fireRate = specialFireRate;        
        turretCam.transform.localPosition = tmpCamPos;
        yield return new WaitForSeconds(specialTime);

        //reset back to normal
        fireRate = saveFireRate;
        projectilePrefab = tmp;
    }
}