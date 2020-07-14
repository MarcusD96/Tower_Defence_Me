using UnityEngine;

public class StandardTurret : ProjectileTurret {

    void Awake() {
        standardTurret = this;
        projectileTurret = this;
    }

    public override void ApplyUpgradeA() { //range++
        range *= ugA.upgradeFactorX;
    }

    public override void ApplyUpgradeB() {  //fireRate++, damage++
        fireRate *= ugB.upgradeFactorX;

        var bullet = projectilePrefab.GetComponent<Bullet>();
        bullet.damage = Mathf.CeilToInt(bullet.damage * ugB.upgradeFactorY);
    }

    public override void EnableSpecial() {
        Debug.Log("TODO: bullet burst");
    }
}
