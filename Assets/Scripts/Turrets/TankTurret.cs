using UnityEngine;

public class TankTurret : ProjectileTurret {

    [Header("Tank")]
    public Animator recoilAnim_Barrel;
    public int projectileNum;
    [Range(0, 0.5f)]
    public float spreadVariance;

    [Header("Special")]
    public TankRunner specialPrefab;
    private TankRunner tankSpecial = null;

    private Vector3 shotDirection = Vector3.zero;

    private void Awake() {
        projectileTurret = this;
        tankTurret = this;
        maxFireRate = (fireRate + (ugB.upgradeFactorX * 6)) * manualFirerateMultiplier;
    }

    private new void Update() {
        base.Update();

        if(hasSpecial && manual) {
            if(Input.GetMouseButtonDown(1)) {
                ActivateSpecial();
            }
        }
    }

    public void AutoShotgun() {
        for(int i = 0; i < projectileNum; i++) {
            shotDirection = RandomDirection();

            GameObject projGO = ObjectPool.instance.ActivateProjectile(ProjectileType.TankShot, fireSpawn.position, fireSpawn.rotation);
            Projectile proj = projGO.GetComponent<Projectile>();

            muzzleFlash.Play();

            Ray ray = new Ray(pivot.position, shotDirection);
            proj.SetStats(damage, bossDamage, pivot.position, ray.GetPoint(range));
            proj.GetTankShot().SetDirection(shotDirection);

            recoilAnim_Barrel.SetTrigger("Shoot");
        }
    }

    public void ManualShotgun() {
        float manualRange = range * manualRangeMultiplier;

        for(int i = 0; i < projectileNum; i++) {
            shotDirection = RandomDirection();

            //spawn proj, get the proj info
            GameObject projGO = ObjectPool.instance.ActivateProjectile(ProjectileType.TankShot, fireSpawn.position, fireSpawn.rotation);
            Projectile proj = projGO.GetComponent<Projectile>();

            muzzleFlash.Play();

            //set proj info
            Ray ray = TryRayCastAndRay(manualRange);
            proj.SetStats(damage, bossDamage, pivot.position, ray.GetPoint(range));
            proj.GetTankShot().SetDirection(shotDirection);

            recoilAnim_Barrel.SetTrigger("Shoot");
        }
    }

    Vector3 RandomDirection() {
        float r = Random.Range(-spreadVariance, spreadVariance);
        Vector3 direction = new Vector3(Vector3.forward.x + r, Vector3.forward.y, Vector3.forward.z);
        direction.Normalize();
        var localForward = pivot.rotation * direction;
        return localForward;
    }

    public override void ApplyUpgradeB() {  //fireRate++, bossDamage++
        fireRate += ugB.upgradeFactorX * ugB.GetLevel();
        projectileNum += (int) ugB.upgradeFactorY;
        spreadVariance += 0.125f;
    }

    public override bool ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0) {
            thisNode.RevertTurret(false);
            tankSpecial = Instantiate(specialPrefab);
            tankSpecial.SetOwner(gameObject, this);
            return true;
        }
        return false;
    }

    public void SetSpecial(bool activated) {
        specialActivated = activated;
    }

    public void StartSpecialTime() {
        StartCoroutine(SpecialTime());
    }
}
