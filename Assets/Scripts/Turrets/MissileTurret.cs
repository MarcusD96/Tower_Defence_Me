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

        var bullet = projectilePrefab.GetComponent<Bullet>();
        bullet.damage = Mathf.CeilToInt(bullet.damage * ugB.upgradeFactorY);

        bullet.explosionRadius += 1;
        bullet.penetration += 1;
    }

    public override void EnableSpecial() {
        Debug.Log("TODO: missile barrage");
    }
}
