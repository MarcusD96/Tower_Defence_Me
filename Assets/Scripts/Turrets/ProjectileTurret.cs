
using UnityEngine;

public class ProjectileTurret : Turret {

    [Header("Projectile Stuff")]
    public GameObject projectilePrefab;
    protected BulletTurret bulletTurret;
    protected MissileTurret missileTurret;
    protected RailgunTurret railgunTurret;

    public float damage, bossDamage;
    public int explosionRadius, penetration;

    public void AutoShoot() {
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

        if(proj) {
            proj.MakeTarget(target);
        }
    }

    public void ManualShoot() {
        float manualRange = range * manualRangeMultiplier;

        //spawn bullet, get the bullet info
        GameObject projGO = Instantiate(projectileTurret.projectilePrefab, fireSpawn.position, fireSpawn.rotation);
        Projectile proj = projGO.GetComponent<Projectile>();


        proj.SetDamage(damage, bossDamage);

        if(missileTurret) {
            proj.GetMissile().SetExplosion(penetration, explosionRadius);
        } else if(railgunTurret) {
            proj.GetRod().SetPenetration(penetration);
        }

        //get a target only if theres a hit, otherwise the target is the mockEnemy
        RaycastHit hit;
        if(Physics.Raycast(transform.position, pivot.forward, out hit, manualRange)) {
            if(hit.collider) {
                //set the target
                proj.miss = false;
                target = hit.collider.transform;
            }
        } else {
            proj.miss = true;
            target = null;
        }

        Ray ray;
        if(target) {
            ray = new Ray(pivot.position, target.position - pivot.position);
        } else {
            ray = new Ray(pivot.position, pivot.forward);
        }

        proj.SetLifePositions(pivot.position, ray.GetPoint(manualRange));
        if(railgunTurret) {
            proj.SetLifePositions(pivot.position, ray.GetPoint(manualRange * 2));
        }

        if(proj) {
            proj.MakeTarget(target);
        }
    }
}