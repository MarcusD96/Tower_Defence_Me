
using System.Collections;
using UnityEngine;

public class RailgunTurret : ProjectileTurret {

    public GameObject specialPrefab;

    void Awake() {
        projectileTurret = this;
        railgunTurret = this;
        maxFireRate = fireRate;
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
        damage += ugB.upgradeFactorX;
        penetration += (int) ugB.upgradeFactorY;
    }

    public override bool ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0) {
            specialActivated = true;
            StartCoroutine(MegaShot());
            return true;
        }
        return false;
    }

    IEnumerator MegaShot() {
        StartCoroutine(SpecialTime());

        //save info
        var tmpCamPos = turretCam.transform.localPosition;
        var tmpDamage = damage;
        var tmpPenetration = penetration;

        //delay tower from shooting
        nextFire += 2.5f;

        //zoom camera in slowly
        float endTime = Time.time + 2.0f;
        turretCam.transform.localPosition += Vector3.forward * 3;
        for(float i = Time.time; i <= endTime; i += Time.deltaTime) {
            var pos = turretCam.transform.localPosition;
            pos.z = Mathf.Lerp(pos.z, pos.z - 3, Time.deltaTime);
            turretCam.transform.localPosition = pos;
            yield return null;
        }

        //do special
        GameObject tmp = projectilePrefab;
        projectilePrefab = specialPrefab;
        damage = 100;
        penetration = 100;

        if(manual)
            ManualShoot();
        else {
            if(!FindEnemy(true)) {
                ManualShoot();
            } else {
                RotateOnShoot();
                AutoShoot();
            }
        }

        //return to normal
        projectilePrefab = tmp;
        turretCam.transform.localPosition = tmpCamPos;
        damage = tmpDamage;
        penetration = tmpPenetration;
    }
}
