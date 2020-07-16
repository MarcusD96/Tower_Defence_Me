using UnityEngine;
using System.Collections;

public class StandardTurret : ProjectileTurret {
    public float specialTime = 1.0f;

    private bool specialActivated = false;

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
    }

    public override void ApplyUpgradeA() { //range++
        range *= ugA.upgradeFactorX;
    }

    public override void ApplyUpgradeB() {  //fireRate++, damage++
        fireRate *= ugB.upgradeFactorX;
        damage = Mathf.CeilToInt(damage * ugB.upgradeFactorY);
    }

    void ActivateBurst() {
        if(!specialActivated) {
            specialActivated = true;
            StartCoroutine(BulletBurst()); 
        }
    }

    IEnumerator BulletBurst() {
        var saveFireRate = fireRate;
        fireRate *= 10.0f;
        yield return new WaitForSeconds(specialTime);
        fireRate = saveFireRate;
        specialActivated = false;
    }
}