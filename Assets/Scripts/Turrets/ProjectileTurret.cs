using UnityEngine;

public class ProjectileTurret : Turret {

    public GameObject projectilePrefab;
    protected StandardTurret standardTurret;
    protected MissileTurret missileTurret;

    public void AutoShoot() {
        GameObject bulletGO = Instantiate(projectilePrefab, fireSpawn.position, fireSpawn.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if(bullet) {
            bullet.MakeTarget(target);
        }
    }

    public void ManualShoot() {
        float manualRange = range * 2;

        //spawn bullet, get the bullet info
        GameObject bulletGO = Instantiate(projectileTurret.projectilePrefab, fireSpawn.position, fireSpawn.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

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
