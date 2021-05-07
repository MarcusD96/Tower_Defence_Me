
using UnityEngine;

public class ProjectileTurret : Turret {

    [Header("Projectile Stuff")]
    public GameObject projectilePrefab;
    protected BulletTurret bulletTurret;
    protected MissileTurret missileTurret;
    protected RailgunTurret railgunTurret;
    protected TankTurret tankTurret;

    public float damage, bossDamage;
    public int explosionRadius, penetration;

    public void AutoShoot() {
        if(tankTurret) {
            tankTurret.AutoShotgun();
            return;
        }

        GameObject prpojectileGO = Instantiate(projectilePrefab, fireSpawn.position, fireSpawn.rotation);
        Projectile proj = prpojectileGO.GetComponent<Projectile>();

        Ray ray = new Ray(pivot.position, target.position - pivot.position);
        proj.SetLifePositions(pivot.position, ray.GetPoint(range));
        proj.SetDamage(damage, bossDamage);

        if(missileTurret) {
            proj.GetMissile().SetExplosion(penetration, explosionRadius);
        } else if(railgunTurret) {
            proj.GetRod().SetPenetration(penetration);
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
        GameObject projGO = Instantiate(projectileTurret.projectilePrefab, fireSpawn.position, fireSpawn.rotation);
        Projectile proj = projGO.GetComponent<Projectile>();

        //set proj info
        proj.SetDamage(damage, bossDamage);

        //set more specific info based on the type of proj
        if(missileTurret) {
            proj.GetMissile().SetExplosion(penetration, explosionRadius);
        } else if(railgunTurret) {
            proj.GetRod().SetPenetration(penetration);
        }

        Ray ray = TryRayCastAndRay(manualRange);

        proj.SetLifePositions(pivot.position, ray.GetPoint(manualRange));

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