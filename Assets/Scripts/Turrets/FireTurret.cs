
using UnityEngine;

public class FireTurret : Turret {
    [Header("Fire Turret")]
    public FireShot fireShot;
    public float damage;
    public int penetration;

    void Awake() {
        fireTurret = this;
        print("TODO: burn effect on enemies");
    }

    public void AutoShoot() {
        var f = Instantiate(fireShot.gameObject, fireSpawn.position, fireShot.transform.rotation);
        f.GetComponent<FireShot>().SetStats(damage, range, penetration);
    }

    public void ManualShoot() {
        var f = Instantiate(fireShot.gameObject, fireSpawn.position, fireShot.transform.rotation);
        f.GetComponent<FireShot>().SetStats(damage, range, penetration);
    }
    public override void ApplyUpgradeB() {  //fireRate++, penetration++,
        fireRate += ugB.upgradeFactorX;
        penetration += (int) ugB.upgradeFactorY;
    }

    public override bool ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0) {
            print("TODO: fire special");
        }
        return false;
    }
}
