
using UnityEngine;

public class LaserTurret : BeamTurret {

    [Header("Laser")]
    [Range(0.1f, 1.0f)]
    public float slowFactor;
    public float slowDuration;
    public float damageOverTime;
    public float slowMultiplier;
    public ParticleSystem impactEffect;
    public Light impactLight;

    private bool isDamaging = false;
    private Transform lastTarget;
    private float manualSlowFactor, maxSlowFactor;

    [Header("Laser Special")]
    public SlowWave slowWave;
    private SlowWave tempSW;

    new void Awake() {
        base.Awake();
        beamTurret = this;
        laserTurret = this;
        impactEffect.Stop();
        impactLight.enabled = false;
        manualSlowFactor = slowFactor / slowMultiplier;
        maxSlowFactor = (slowFactor - (ugB.upgradeFactorY * 3)) / slowMultiplier;
    }

    Transform targetPrev; //used for sound purposes only
    new void Update() {
        base.Update();

        if(hasSpecial && manual) {
            if(Input.GetMouseButtonDown(1)) {
                ActivateSpecial();
            }
        }

        if(tempSW) {
            if(tempSW.done)
                Destroy(tempSW.gameObject);
        }

        if(specialBar.fillBar.fillAmount <= 0) {
            specialActivated = false;
        }
    }

    public override void AutoShoot() {
        if(lastTarget != target) {
            lastTarget = target;
            isDamaging = false;
        }

        RotateOnShoot();

        //apply new slow if not slowed
        if(targetEnemy.speed > slowFactor) {
            if(!hasSpecial)
                targetEnemy.Slow(slowFactor, slowDuration);
            else
                targetEnemy.Slow(maxSlowFactor, slowDuration);
        }

        if(!isDamaging) {
            isDamaging = true;
            targetEnemy.DamageOverTime(0, slowDuration);
        }

        if(target != targetPrev) {
            AudioManager.PlaySound(shootSound, transform.position);
            targetPrev = target;
        }

        //graphics
        if(!lineRenderer.enabled) {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }

        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.forward * Vector3.Distance(pivot.position, target.position));

        //rotation
        Vector3 direction = fireSpawn.position - target.position;
        impactEffect.transform.position = target.position + direction.normalized;
        impactEffect.transform.rotation = Quaternion.LookRotation(direction);
    }

    public override void ManualShoot() {
        if(lastTarget != target) {
            lastTarget = target;
            isDamaging = false;
        }

        lineRenderer.SetPosition(0, Vector3.zero);

        if(!lineRenderer.enabled) {
            AudioManager.PlaySound(shootSound, transform.position);
        }

        float manualRange = range * manualRangeMultiplier;

        RaycastHit hit;
        if(Physics.Raycast(pivot.position, pivot.forward, out hit, manualRange)) {
            if(hit.collider) {
                //set the target and get its information
                target = hit.transform;
                targetEnemy = target.GetComponent<Enemy>();

                if(targetEnemy) {
                    if(!targetEnemy.superSlow)
                        targetEnemy.Slow(manualSlowFactor, slowDuration);

                    if(!isDamaging) {
                        isDamaging = true;
                        targetEnemy.DamageOverTime(0, slowDuration);
                    }

                    if(target != targetPrev) {
                        AudioManager.PlaySound(shootSound, transform.position);
                        targetPrev = target;
                    }

                    //graphics
                    if(!lineRenderer.enabled) {
                        lineRenderer.enabled = true;
                        impactEffect.Play();
                        impactLight.enabled = true;
                    }
                }

                //set end position of the laser line renderer
                lineRenderer.SetPosition(1, Vector3.forward * hit.distance);

                //turn the particles towards the turret
                Vector3 direction = transform.position - target.transform.position;
                impactEffect.transform.position = target.position + direction.normalized;
                impactEffect.transform.rotation = Quaternion.LookRotation(direction);
            }
        } else {
            target = null;
            targetEnemy = null;
            lineRenderer.SetPosition(1, Vector3.forward * manualRange);
            lineRenderer.enabled = true;
            impactEffect.Stop();
            impactLight.enabled = false;
        }
    }

    public override void ApplyUpgradeB() {  //slow++, slow duration++, laser thiccc++
        slowDuration += ugB.upgradeFactorX;
        slowFactor -= ugB.upgradeFactorY;
        manualSlowFactor = slowFactor / slowMultiplier;   //update MANUAL
        lineRenderer.startWidth = lineRenderer.endWidth += 0.3f;
    }

    public override void ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0) {
            specialActivated = true;
            specialBar.fillBar.fillAmount = 1; //fully filled, on cooldown
            StartCoroutine(SpecialTime());
            tempSW = Instantiate(slowWave, transform.position, transform.rotation);
            tempSW.slowFactor = maxSlowFactor;
            tempSW.slowDuration = slowDuration * 5;
        }
    }
}