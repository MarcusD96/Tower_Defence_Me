using UnityEngine;

public class MissileTurret : ProjectileTurret {

    void Awake() {
        missileTurret = this;
        projectileTurret = this;
    }

    public override void ApplyUpgradeA() { //range++
        range *= ugA.upgradeFactorX;
    }

    public override void ApplyUpgradeB() {  //fireRate++, damage++, explosionRad+1, penetration +1
        fireRate *= ugB.upgradeFactorX;
        damage = Mathf.CeilToInt(damage * ugB.upgradeFactorY);
        explosionRadius += 1;
        penetration += 1;
    }
}
