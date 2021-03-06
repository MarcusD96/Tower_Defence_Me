﻿using System.Collections;
using UnityEngine;

public class LaserTurret : BeamTurret {

    [Header("Laser")]
    public float slowFactor;
    public float slowDuration; 
    public float damageOverTime;
    public ParticleSystem impactEffect;
    public Light impactLight;
    public Gradient mainGrad, specGrad;

    private Transform lastTarget;
    private float maxSlowFactor;

    [Header("Laser Special")]
    public LaserWave special;

    new void Awake() {
        base.Awake();
        beamTurret = this;
        laserTurret = this;
        impactEffect.Stop();
        shootEffect.Stop();
        impactLight.enabled = shootLight.enabled = false;
        maxSlowFactor = (slowFactor - (ugB.upgradeFactorY * 6));
        startFR = fireRate;
    }

    Transform targetPrev; //used for sound purposes only
    protected new void Update() {
        base.Update();
        if(hasSpecial && manual) {
            if(Input.GetMouseButtonDown(1)) {
                ActivateSpecial();
            }
        }
    }

    float slowDurationEnd = float.MaxValue;
    public override void AutoShoot() {

        #region check if can shoot
        if(nextFire <= 0.0f && WaveSpawner.enemiesAlive > 0) {
            FindEnemy(false);
            if(target == null)
                return;
            nextFire = 1 / fireRate;
        }
        #endregion

        #region if changed target
        if(lastTarget != target || Time.time >= slowDurationEnd) {
            lastTarget = target;
            BeamOff();
            slowDurationEnd = Time.time + slowDuration;
        }
        #endregion

        #region target falls out of range, stop firing
        if(target == null || Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.position.x, target.position.z)) > range) {
            target = null;
            BeamOff();
            return;
        }
        #endregion

        RotateOnShoot();

        #region apply new slow if not slowed
        if(targetEnemy.currentSpeed > slowFactor) {
            if(!hasSpecial)
                targetEnemy.Slow(slowFactor, slowDuration, ugB.GetLevel());
            else
                targetEnemy.Slow(maxSlowFactor, slowDuration, ugB.GetLevel());
        }
        #endregion

        #region apply DoT to new enemies if its not already being damaged by the laser
        if(!targetEnemy.isDamaging)
            targetEnemy.DamageOverTime(damageOverTime, slowDuration);
        #endregion

        #region play sound if new target is hit
        if(target != targetPrev) {
            AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);
            targetPrev = target;
        }
        #endregion

        #region graphics
        if(!lineRenderer.enabled) {
            lineRenderer.enabled = true;
            impactEffect.Play();
            shootEffect.Play();
            impactLight.enabled = shootLight.enabled = true;
        }
        #endregion

        #region set line positions
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.forward * Vector3.Distance(transform.InverseTransformVector(fireSpawn.position), transform.InverseTransformVector(target.position)));
        #endregion

        #region laser impact effect rotation
        Vector3 direction = fireSpawn.position - target.position;
        impactEffect.transform.position = target.position + direction.normalized;
        impactEffect.transform.rotation = Quaternion.LookRotation(direction);
        #endregion
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
            Debug.DrawRay(pivot.position, (pivot.rotation * Vector3.forward).normalized, Color.red, 5);
            if(hit.collider) {
                //set the target and get its information
                target = hit.transform;
                targetEnemy = target.GetComponent<Enemy>();

                if(targetEnemy) {
                    if(!targetEnemy.superSlow)
                        targetEnemy.Slow(slowFactor, slowDuration, ugB.GetLevel());

                    if(!targetEnemy.isDamaging) {
                        targetEnemy.DamageOverTime(damageOverTime, slowDuration);
                    }

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
        } 
        else {
            target = null;
            targetEnemy = null;
            lineRenderer.SetPosition(1, (Vector3.forward * manualRange) + new Vector3(0, 0, fireSpawn.localPosition.z * transform.localScale.z));
            lineRenderer.enabled = true;
            impactEffect.Stop();
            impactLight.enabled = false;
        }
    }

    float startFR;
    public override void ApplyUpgradeB() {
        //finds enemies faster
        fireRate = startFR * ugB.GetLevel() * 5;

        //slows enemies longer
        slowDuration += ugB.upgradeFactorX;

        //slows enemies more
        slowFactor -= ugB.upgradeFactorY * ugB.GetLevel();

        //make laser wider
        lineRenderer.startWidth = lineRenderer.endWidth += 0.3f;
    }

    public override bool ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0) {
            specialActivated = true;
            specialBar.fillBar.fillAmount = 1; //fully filled, on cooldown
            StartCoroutine(SpecialTime());
            StartCoroutine(Special());
            return true;
        }
        return false;
    }

    IEnumerator Special() {
        LaserWave tempSW = Instantiate(special, transform.position, special.transform.rotation);
        tempSW.Initialize(slowFactor, range * 2, specialTime, this);
        lineRenderer.colorGradient = specGrad;
        damageOverTime *= 3;
        yield return new WaitForSeconds(specialTime);
        damageOverTime /= 3;
        lineRenderer.colorGradient = mainGrad;
    }
}