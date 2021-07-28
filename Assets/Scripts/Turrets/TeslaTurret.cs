using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaTurret : BeamTurret {

    [Header("Tesla")]
    public float damage;
    public float stunDuration;

    [Header("Lightning")]
    public Gradient mainGrad, specGrad;
    public float arcLength;
    public float arcVar, inaccuracy, detectionRad;
    public int maxArcs, specialArcs;
    [SerializeField]
    private IEnumerator fade;

    new void Awake() {
        base.Awake();
        fade = FadeOut();
        beamTurret = this;
        teslaTurret = this;
        maxFireRate = (fireRate + (ugB.upgradeFactorX * 6)) * manualFirerateMultiplier;
    }

    new void Update() {
        base.Update();

        if(hasSpecial && manual) {
            if(Input.GetMouseButtonDown(1)) {
                ActivateSpecial();
            }
        }
    }

    public override void AutoShoot() {
        if(!FindEnemy(false)) {
            BeamOff();
            return;
        }

        if(nextFire <= 0.0f) {
            nextFire = 1 / fireRate;
        }
        else {
            return;
        }

        //aim at target
        RotateOnShoot();
        recoilAnim_Body.SetTrigger(shootAnim);

        //particle effects
        shootEffect.Play();
        shootLight.enabled = true;

        //build gfx
        Enemy[] enemies = BuildLightning(targetEnemy);
        foreach(var e in enemies) {
            e.TakeDamage(damage, Color.yellow);
            e.Stun(stunDuration, ugB.GetLevel());
        }

        AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);

        Fade();
    }

    public override void ManualShoot() {
        //gotta do the reverse since we cannot reset the timer if the shot is missed.
        if(nextFire > 0) {
            return;
        }

        var manualFireRate = fireRate;
        if(!hasSpecial) {
            manualFireRate = fireRate * manualFirerateMultiplier;
        }

        range *= manualRangeMultiplier;
        RaycastHit hit;
        if(Physics.Raycast(fireSpawn.position, pivot.forward, out hit, range)) {
            if(hit.collider) {
                Enemy[] enemies = BuildLightning(hit.collider.GetComponent<Enemy>());

                nextFire = 1 / manualFireRate;
                shake.shakeDuration = 0.1f;
                recoilAnim_Body.SetTrigger(shootAnim);
                AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);

                foreach(var e in enemies) {
                    e.TakeDamage(damage, Color.yellow);
                    e.Stun(stunDuration, ugB.GetLevel());
                }

                if(!lineRenderer.enabled) {
                    lineRenderer.enabled = true;
                }

                //particle effects
                shootEffect.Play();
                shootLight.enabled = true;

                Fade();
            }
        }
        range /= manualRangeMultiplier;
    }

    List<Enemy> enemyList = new List<Enemy>();
    Enemy[] BuildLightning(Enemy first) {
        enemyList.Clear();

        enemyList.Add(first);

        Vector3 startPos = first.transform.position;

        //keep trying to find new enemies to hit in range of current enemy until max enemies is hit or no more enemies are found
        for(int i = 0; i < maxArcs; i++) {
            var closest = float.MaxValue;

            //find the closest enemy in the radius
            Enemy next = null;

            foreach(var c in Physics.OverlapSphere(startPos, detectionRad)) {

                if(c.CompareTag(enemyTag)) {
                    if(enemyList.Contains(c.GetComponent<Enemy>())) //dont hit the same enemy more than once
                        continue;

                    var distance = Vector3.Distance(startPos, c.transform.position);
                    if(distance < closest) {
                        closest = distance;
                        startPos = c.transform.position;
                        next = c.GetComponent<Enemy>();
                    }
                }
            }

            if(closest == float.MaxValue) { //no other enemies found, break out
                break;
            }

            enemyList.Add(next);
        }

        //draw the actual lightning
        {
            RestoreAlpha();

            var lastPoint = fireSpawn.position; //the first point
            var lineVert = 1;
            lineRenderer.SetPosition(0, lastPoint); //the first point is the firing position

            for(int currentT = 0; currentT < enemyList.Count; currentT++) {
                target = enemyList[currentT].transform;

                while(Vector3.Distance(target.position, lastPoint) > 4) {
                    lineRenderer.positionCount = lineVert + 1;
                    var fwd = target.position - lastPoint;
                    fwd.Normalize();
                    fwd = RandomizeArc(fwd, inaccuracy);
                    fwd *= Random.Range(arcLength * arcVar, arcLength);
                    fwd += lastPoint;
                    if(fwd.y < 1.6f) {
                        fwd.y = 1.6f;
                    }
                    lineRenderer.SetPosition(lineVert, fwd);
                    lineVert++;
                    lastPoint = fwd;
                }
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, target.position); //set the position to the target
                ObjectPool.instance.ActivateEffect(EffectType.Flash, target.position + Vector3.up, Quaternion.identity, 0.25f);
            }
        }

        if(!lineRenderer.enabled)
            lineRenderer.enabled = true;


        return enemyList.ToArray();
    }

    void RestoreAlpha() {
        //bring alpha back up to 1
        Color start = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g, lineRenderer.startColor.b, 1);
        Color end = new Color(lineRenderer.endColor.r, lineRenderer.endColor.g, lineRenderer.endColor.b, 1);
        lineRenderer.startColor = start;
        lineRenderer.endColor = end;
        shootLight.intensity = 75;
    }

    public override bool ActivateSpecial() {
        if(!FindEnemy(false))
            return false;

        if(!specialActivated && WaveSpawner.enemiesAlive > 0 && BuildLightning(targetEnemy) != null) {
            specialActivated = true;
            StartCoroutine(SpecialAbility());
            return true;
        }
        return false;
    }

    Vector3 RandomizeArc(Vector3 v, float inaccuracy_) {
        v += new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f) * inaccuracy_);
        v.Normalize();
        return v;
    }

    List<Enemy> Shuffle(List<Enemy> list) {
        int n = list.Count;
        System.Random rng = new System.Random();
        while(n > 1) {
            n--;
            int k = rng.Next(n + 1);
            Enemy value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    public override void ApplyUpgradeB() { //upgrade firerate and damage
        fireRate += ugB.upgradeFactorX * ugB.GetLevel();
        maxArcs += (int) ugB.upgradeFactorY;
        detectionRad += 2;
    }

    void Fade() {
        StopCoroutine(fade);
        fade = FadeOut();
        StartCoroutine(fade);
    }

    IEnumerator FadeOut() {
        while(lineRenderer.startColor.a > 0) {
            Color start = lineRenderer.startColor;
            Color end = lineRenderer.endColor;

            start.a = Mathf.Lerp(start.a, -1, Time.deltaTime);
            end.a = Mathf.Lerp(end.a, -1, Time.deltaTime);

            shootLight.intensity -= 12.5f;

            lineRenderer.startColor = start;
            lineRenderer.endColor = end;

            yield return null;
        }
        shootLight.intensity = 0;
    }

    IEnumerator SpecialAbility() {
        if(!FindEnemy(true))
            yield break;

        StartCoroutine(SpecialTime());

        Fade();
        nextFire = float.MaxValue;
        yield return new WaitForSeconds(2.5f);

        lineRenderer.colorGradient = specGrad;

        //boost stats
        var tmpArcs = maxArcs;
        maxArcs = specialArcs;
        detectionRad *= 5.0f;
        damage *= 5.0f;
        stunDuration *= 2.0f;
        nextFire = 0.0f;

        AutoShoot();

        yield return new WaitForSeconds(1.0f);

        lineRenderer.colorGradient = mainGrad;

        //revert boosted stats
        maxArcs = tmpArcs;
        detectionRad /= 5.0f;
        damage /= 5.0f;
        stunDuration /= 2.0f;
    }
}