
using System.Collections;
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
        shootEffect.Stop();
        impactLight.enabled = shootLight.enabled = false;
        manualSlowFactor = slowFactor / slowMultiplier;
        maxSlowFactor = (slowFactor - (ugB.upgradeFactorY * 3)) / slowMultiplier;
    }

    Transform targetPrev; //used for sound purposes only
    protected new void Update() {
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
        if(nextFire <= 0.0f && WaveSpawner.enemiesAlive > 0) {
            FindEnemy(false);
            if(target == null)
                return;
            nextFire = 1 / fireRate;
        }

        //changed target, enable damage again
        if(lastTarget != target || Time.time >= slowDurationEnd) {
            lastTarget = target;
            BeamOff();
            slowDurationEnd = Time.time + slowDuration;
        }

        //target falls out of range
        if(target == null || Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.position.x, target.position.z)) > range) {
            target = null;
            BeamOff();
            return;
        }

        RotateOnShoot();

        //apply new slow if not slowed
        if(targetEnemy.currentSpeed > slowFactor) {
            if(!hasSpecial)
                targetEnemy.Slow(slowFactor, slowDuration);
            else
                targetEnemy.Slow(maxSlowFactor, slowDuration);
        }

        //only apply DoT to new enemies
        if(!targetEnemy.isDamaging) {
            targetEnemy.DamageOverTime(damageOverTime, slowDuration);
        }

        //play sound if new target is hit
        if(target != targetPrev) {
            AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);
            targetPrev = target;
        }

        //graphics
        if(!lineRenderer.enabled) {
            lineRenderer.enabled = true;
            impactEffect.Play();
            shootEffect.Play();
            impactLight.enabled = shootLight.enabled = true;
        }

        //set line positions
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.forward * Vector3.Distance(transform.InverseTransformVector(fireSpawn.position), transform.InverseTransformVector(target.position)));

        //rotation
        Vector3 direction = fireSpawn.position - target.position;
        impactEffect.transform.position = target.position + direction.normalized;
        impactEffect.transform.rotation = Quaternion.LookRotation(direction);
    }

    public override void ManualShoot() {
        if(lastTarget != target) {
            lastTarget = target;
            BeamOff();
        }

        lineRenderer.SetPosition(0, Vector3.zero);
        shake.shakeDuration = 0.1f;

        if(!shootEffect.isPlaying) {
            shootEffect.Play();
            shootLight.enabled = true;
        }

        if(!lineRenderer.enabled) {
            AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);
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

                    targetEnemy.TakeDamage(damageOverTime * Time.deltaTime, Color.white, false);

                    if(target != targetPrev) {
                        AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);
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
                lineRenderer.SetPosition(1, transform.forward * hit.distance);

                //turn the particles towards the turret
                Vector3 direction = transform.position - target.transform.position;
                impactEffect.transform.position = target.position + direction.normalized;
                impactEffect.transform.rotation = Quaternion.LookRotation(direction);
            }
        } else {
            target = null;
            targetEnemy = null;
            //lineRenderer.SetPosition(1, Vector3.forward * manualRange);
            lineRenderer.SetPosition(1, (Vector3.forward * manualRange) + new Vector3(0, 0, fireSpawn.localPosition.z * transform.localScale.z));
            lineRenderer.enabled = true;
            impactEffect.Stop();
            impactLight.enabled = false;
        }
    }

    public override void ApplyUpgradeB() {
        //slows enemies longer
        slowDuration += ugB.upgradeFactorX;

        //finds enemies faster
        switch(ugB.GetLevel()) {
            case 1:
                fireRate = 1.0f / 0.7f;
                break;
            case 2:
                fireRate = 1.0f / 0.4f;
                break;
            case 3:
                fireRate = 1.0f / 0.1f;
                break;
        }

        //slows enemies more
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