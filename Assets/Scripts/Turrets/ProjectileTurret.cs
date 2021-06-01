
using UnityEngine;

public class ProjectileTurret : Turret {

    [Header("Projectile Stuff")]
    public GameObject projectilePrefab;
    protected BulletTurret bulletTurret;
    protected MissileTurret missileTurret;
    protected RailgunTurret railgunTurret;
    protected TankTurret tankTurret;

    public float damage, bossDamage;
    public int penetration;

    ProjectileType CheckType() {
        //bullet
        if(bulletTurret)
            return ProjectileType.Bullet;
        else if(bulletTurret && specialActivated)
            return ProjectileType.Bullet_Special;

        //missile
        else if(missileTurret)
            return ProjectileType.Missile;
        else if(missileTurret && specialActivated)
            return ProjectileType.Missile_Special;

        //rod
        else if(railgunTurret)
            return ProjectileType.Rod;
        else if(railgunTurret && specialActivated)
            return ProjectileType.Rod_Special;

        //error
        else
            return ProjectileType.Bullet;
    }

    public void AutoShoot() {
        if(tankTurret) {
            tankTurret.AutoShotgun();
            return;
        }

        GameObject projGO = ObjectPool.instance.ActivateProjectile(CheckType(), fireSpawn.position, fireSpawn.rotation);
        Projectile proj = projGO.GetComponent<Projectile>();

        Ray ray = new Ray(pivot.position, target.position - pivot.position);
        proj.SetStats(damage, bossDamage, pivot.position, ray.GetPoint(range));

        if(bulletTurret) {
            proj.GetBullet().InitializeDirection();
        }
        else if(missileTurret) {
            proj.GetMissile().SetExplosion(penetration);
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
        GameObject projGO = ObjectPool.instance.ActivateProjectile(ProjectileType.Bullet, fireSpawn.position, fireSpawn.rotation);
        Projectile proj = projGO.GetComponent<Projectile>();

        //set more specific info based on the type of proj
        if(bulletTurret) {
            proj.GetBullet().InitializeDirection();
        }
        else if(missileTurret) {
            proj.GetMissile().SetExplosion(penetration);
        } else if(railgunTurret) {
            proj.GetRod().SetPenetration(penetration);
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