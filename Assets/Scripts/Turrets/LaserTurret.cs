
using UnityEngine;

public class LaserTurret : BeamTurret {

    [Header("Laser")]
    public float slowFactor;
    public float slowDuration;
    public float damageOverTime;
    public float slowMultiplier;
    public ParticleSystem impactEffect;
    public Light impactLight;
    
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

    float slowDurationEnd = float.MaxValue;
    public override void AutoShoot() {
        if(!IsInvoking())
            InvokeRepeating(nameof(FindEnemy), 0.01f, fireRate);

        //changed target, enable damage again
        if(lastTarget != target || Time.time >= slowDurationEnd) {
            lastTarget = target;
            LaserOff();
            slowDurationEnd = Time.time + slowDuration;
        }

        if(target == null) {
            LaserOff();
            return;
        }

        RotateOnShoot();

        //apply new slow if not slowed
        if(targetEnemy.speed > slowFactor) {
            if(!hasSpecial)
                targetEnemy.Slow(slowFactor, slowDuration);
            else
                targetEnemy.Slow(maxSlowFactor, slowDuration);
        }

        //only apply DoT to new enemies
        if(!targetEnemy.isDamaging) {
            targetEnemy.DamageOverTime(damageOverTime, slowDuration);
        }

        if(target != targetPrev) {
            AudioManager.StaticPlay(shootSound, transform.position);
            targetPrev = target;
        }

        //graphics
        if(!lineRenderer.enabled) {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }

        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.forward * Vector3.Distance(transform.InverseTransformVector(fireSpawn.position), transform.InverseTransformVector(target.position)));

        //rotation
        Vector3 direction = fireSpawn.position - target.position;
        impactEffect.transform.position = target.position + direction.normalized;
        impactEffect.transform.rotation = Quaternion.LookRotation(direction);
    }

    public override void ManualShoot() {
        if(IsInvoking())
            CancelInvoke();

        if(lastTarget != target) {
            lastTarget = target;
            LaserOff();
        }

        lineRenderer.SetPosition(0, Vector3.zero);

        if(!lineRenderer.enabled) {
            AudioManager.StaticPlay(shootSound, transform.position);
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

                    if(!targetEnemy.isDamaging) {
                        targetEnemy.DamageOverTime(damageOverTime, slowDuration);
                    }

                    targetEnemy.TakeDamage(damageOverTime * Time.deltaTime, false);

                    if(target != targetPrev) {
                        AudioManager.StaticPlay(shootSound, transform.position);
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

    public override void ApplyUpgradeB() {
        if(IsInvoking())
            CancelInvoke();

        //slows enemies longer
        slowDuration += ugB.upgradeFactorX;

        //finds enemies faster(also update invoke for when auto shoot) and slows enemies more
        fireRate -= ugB.upgradeFactorY;
        InvokeRepeating(nameof(FindEnemy), 0, fireRate);
        slowFactor -= ugB.upgradeFactorY;

        //make laser wider and update manual slow factor
        manualSlowFactor = slowFactor / slowMultiplier;   
        lineRenderer.startWidth = lineRenderer.endWidth += 0.3f;//update MANUAL
    }

    public override bool ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0) {
            specialActivated = true;
            specialBar.fillBar.fillAmount = 1; //fully filled, on cooldown
            StartCoroutine(SpecialTime());
            tempSW = Instantiate(slowWave, transform.position, transform.rotation);
            tempSW.slowFactor = maxSlowFactor;
            tempSW.slowDuration = slowDuration * 5;
            return true;
        }
        return false;
    }
}