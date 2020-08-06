
using UnityEngine;

public class RailgunTurret : ProjectileTurret {

    void Awake() {
        projectileTurret = this;
        railgunTurret = this;
    }

    new void Update() {
        base.Update();
        //special stuff maybe 
    }

    public override void ApplyUpgradeB() { //fireRate++, penetration++;
        fireRate *= ugB.upgradeFactorX;
        penetration = Mathf.CeilToInt(penetration + ugB.upgradeFactorY);
    }
}
