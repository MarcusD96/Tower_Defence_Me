using UnityEngine;

public class ProjectileTurret : Turret {

    [Header("Projectile Stuff")]
    public GameObject projectilePrefab;
    protected StandardTurret standardTurret;
    protected MissileTurret missileTurret;

    public int damage = 50, explosionRadius = 0, penetration = 0;

    public void AutoShoot() {
        GameObject bulletGO = Instantiate(projectilePrefab, fireSpawn.position, fireSpawn.rotation);
        Missile bullet = bulletGO.GetComponent<Missile>();
        bullet.SetDamage(damage);
        if(missileTurret) {
            bullet.SetExplosion(penetration, explosionRadius);
        }

        if(bullet) {
            bullet.MakeTarget(target);
        }
    }

    public void ManualShoot() {
        float manualRange = range * 2;

        //spawn bullet, get the bullet info
        GameObject bulletGO = Instantiate(projectileTurret.projectilePrefab, fireSpawn.position, fireSpawn.rotation);
        Missile bullet = bulletGO.GetComponent<Missile>();
        bullet.SetDamage(damage);
        if(missileTurret) {
            bullet.SetExplosion(penetration, explosionRadius); 
        }

        //get a target only if theres a hit, otherwise the target is the mockEnemy; 
        RaycastHit hit;
        if(Physics.Raycast(fireSpawn.position, pivot.forward, out hit, manualRange)) {
            if(hit.collider) {
                //set the target
                bullet.miss = false;
                target = hit.collider.transform;
            }
        } else {
            bullet.miss = true;
            target = mockEnemy.transform;
        }

        if(bullet) {
            bullet.MakeTarget(target);
        }
    }
}
