
using UnityEngine;

public class ProjectileTurret : Turret {

    [Header("Projectile Stuff")]
    public WeaponType projectileType;
    public float damage, bossDamage;
    public int penetration;

    protected BulletTurret bulletTurret;
    protected MissileTurret missileTurret;
    protected RailgunTurret railgunTurret;
    protected TankTurret tankTurret;

    public void AutoShoot() {
        if(tankTurret) {
            tankTurret.AutoShotgun();
            return;
        }

        GameObject projGO = ObjectPool.instance.ActivateProjectile(projectileType, fireSpawn.position, fireSpawn.rotation);
        Projectile proj = projGO.GetComponent<Projectile>();

        Ray ray = new Ray(pivot.position, target.position - pivot.position);
        proj.SetStats(damage, bossDamage, pivot.position, ray.GetPoint(range));

        if(bulletTurret) {
            proj.GetBullet().Initialize(penetration);
        }
        else if(missileTurret) {
            proj.GetMissile().SetExplosion(penetration, missileTurret.explosionRadius);
        } else if(railgunTurret) {
            proj.GetRod().SetPenetration(penetration);
            proj.GetRod().InitializeDirection();
            proj.SetLifePositions(pivot.position, ray.GetPoint(range * 2));
        }

        proj.MakeTarget(target);
    }

    public void ManualShoot() {
        if(tankTurret) {
            tankTurret.ManualShotgun();
            return;
        }

        float manualRange = range * manualRangeMultiplier;

        //spawn proj, get the proj info
        GameObject projGO = ObjectPool.instance.ActivateProjectile(projectileType, fireSpawn.position, fireSpawn.rotation);
        Projectile proj = projGO.GetComponent<Projectile>();

        //set more specific info based on the type of proj
        if(bulletTurret) {
            proj.GetBullet().Initialize(penetration);
        }
        else if(missileTurret) {
            proj.GetMissile().SetExplosion(penetration, missileTurret.explosionRadius);
        } else if(railgunTurret) {
            proj.GetRod().SetPenetration(penetration);
            proj.GetRod().InitializeDirection();
        }

        Ray ray = TryRayCastAndRay(manualRange);

        //set proj info
        proj.SetStats(damage, bossDamage, pivot.position, ray.GetPoint(range));

        if(railgunTurret) {
            proj.SetLifePositions(pivot.position, ray.GetPoint(manualRange * 2));
        }

        if(proj) {
            proj.MakeTarget(target);
        }
    }

    protected Ray TryRayCastAndRay(float manRange) {
        //raycast to find target
        RaycastHit hit;
        if(Physics.Raycast(fireSpawn.position, pivot.forward, out hit, manRange)) {
            if(hit.collider.CompareTag("Enemy")) {
                //set the target
                target = hit.collider.transform;
                targetEnemy = target.GetComponent<Enemy>();
            } else {
                target = null;
                targetEnemy = null;
            }
        } else {
            target = null;
            targetEnemy = null;
        }

        if(target) {
            return new Ray(fireSpawn.position, target.position - pivot.position);
        } else {
            return new Ray(fireSpawn.position, pivot.forward);
        }

    }
}