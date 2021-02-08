
using UnityEngine;

public class LaserTurret : BeamTurret {

    [Header("Laser")]
    public GameObject indicator;
    [Range(0.1f, 1.0f)]
    public float slowFactor = 0.8f;
    public float slowDuration = 2.0f;
    public float damageOverTime = 0.5f;
    public ParticleSystem impactEffect;
    public Light impactLight;

    [Header("Laser Special")]
    public SlowWave slowWave;
    private SlowWave tempSW;

    new void Awake() {
        base.Awake();
        beamTurret = this;
        laserTurret = this;
        impactEffect.Stop();
        impactLight.enabled = false;
    }

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
        RotateOnShoot();

        //damage
        targetEnemy.TakeDamage(damageOverTime * Time.deltaTime, false);

        //apply new slow if not slowed
        if(targetEnemy.speed > slowFactor) {
            targetEnemy.Slow(slowFactor, slowDuration);
        }

        //graphics
        if(!lineRenderer.enabled) {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }
        lineRenderer.SetPosition(0, fireSpawn.position);
        lineRenderer.SetPosition(1, target.position);

        //rotation
        Vector3 direction = fireSpawn.position - target.position;
        impactEffect.transform.position = target.position + direction.normalized;
        impactEffect.transform.rotation = Quaternion.LookRotation(direction);
    }

    public override void ManualShoot() {
        lineRenderer.SetPosition(0, Vector3.zero);

        float manualRange = range * manualRangeMultiplier;


        RaycastHit hit;
        if(Physics.Raycast(pivot.position, pivot.forward, out hit, manualRange)) {
            if(hit.collider) {
                //set the target and get its information
                target = hit.transform;
                targetEnemy = target.GetComponent<Enemy>();

                if(targetEnemy) {
                    //apply damage and slow
                    targetEnemy.TakeDamage(damageOverTime * Time.deltaTime, false);
                    if(!targetEnemy.superSlow) {
                        targetEnemy.Slow(slowFactor, slowDuration);
                    }

                    //graphics
                    if(!lineRenderer.enabled) {
                        lineRenderer.enabled = true;
                        impactEffect.Play();
                        impactLight.enabled = true;
                    }
                }

                //set end position of the laser line renderer
                lineRenderer.SetPosition(1, Vector3.forward * Vector3.Distance(Vector3.zero, hit.point));

                //enable the light and particles
                if(!impactEffect.isPlaying) {
                    impactEffect.Play();
                    impactLight.enabled = true;
                }

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

    public override void ApplyUpgradeB() {  //dps++, slow++
        damageOverTime += ugB.upgradeFactorX;
        slowFactor -= ugB.upgradeFactorY;
        slowDuration++;
        lineRenderer.startWidth = lineRenderer.endWidth += 0.5f;
    }

    public override void ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0) {
            specialActivated = true;
            specialBar.fillBar.fillAmount = 1; //fully filled, on cooldown
            StartCoroutine(SpecialTime());
            tempSW = Instantiate(slowWave, transform.position, transform.rotation);
            tempSW.slowFactor = slowFactor / 2;
            tempSW.slowDuration = slowDuration * 3.5f;
        }
    }
}