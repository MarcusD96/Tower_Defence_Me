using UnityEngine;

public class LaserTurret : Turret {

    [Header("Laser")]
    [Range(0.1f, 1.0f)]
    public float slowFactor = 0.8f;
    float slowDuration = 2.0f;
    public int damageOverTime = 30;
    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;
    public Light impactLight;

    [Header("Laser Special")]
    public SlowWave slowWave;
    private SlowWave temp;

    void Awake() {
        laserTurret = this;
        lineRenderer.enabled = false;
        impactEffect.Stop();
        impactLight.enabled = false;
    }

    new void Update() {
        base.Update();

        if(hasSpecial) {
            if(Input.GetMouseButtonDown(1)) {
                ActivateSlowWave();
            }
        }

        if(temp) {
            if(temp.done)
                Destroy(temp.gameObject);
        }

        if(specialBar.fillBar.fillAmount <= 0) {
            specialActivated = false;
        }
    }

    public void AutoLaser() {
        //damage
        targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
        targetEnemy.Slow(slowFactor, slowDuration);

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

    public void ManualLaser() {
        lineRenderer.SetPosition(0, fireSpawn.position);

        float manualRange = range * 2;
        int manualDoT = Mathf.RoundToInt(damageOverTime * 1.3f);


        RaycastHit hit;
        if(Physics.Raycast(fireSpawn.position, pivot.forward, out hit, manualRange)) {
            if(hit.collider) {
                //set the target and get its information
                target = hit.collider.transform;
                targetEnemy = target.GetComponent<Enemy>();

                if(targetEnemy) {
                    //apply damage and slow
                    targetEnemy.TakeDamage(manualDoT * Time.deltaTime);
                    targetEnemy.Slow(slowFactor, slowDuration);
                }

                //set end position of the laser line renderer
                lineRenderer.SetPosition(1, hit.point);

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
            lineRenderer.SetPosition(1, mockEnemy.transform.position);
            lineRenderer.enabled = true;
            impactEffect.Stop();
            impactLight.enabled = false;
        }
    }

    public void LaserOff() {
        lineRenderer.enabled = false;
        impactEffect.Stop();
        impactLight.enabled = false;
    }

    public override void ApplyUpgradeB() {  //dps++, slow++
        damageOverTime = Mathf.CeilToInt(damageOverTime + ugB.upgradeFactorX);
        slowFactor -= ugB.upgradeFactorY;
        slowDuration++;
        lineRenderer.startWidth = lineRenderer.endWidth += 1;
    }

    void ActivateSlowWave() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0) {
            specialActivated = true;
            specialBar.fillBar.fillAmount = 1; //fully filled, on cooldown
            StartCoroutine(SpecialTime(specialRate));
            temp = Instantiate(slowWave, transform.position, transform.rotation);
            temp.slowFactor = slowFactor / 2;
            temp.slowDuration = slowDuration * 10;
        }
    }
}