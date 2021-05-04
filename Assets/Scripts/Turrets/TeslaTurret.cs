using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaTurret : BeamTurret {

    [Header("Tesla")]
    public float damage;
    public float stunDuration, superFirerate;

    [Header("Lightning")]
    public GameObject flash;
    public Gradient mainGrad, specGrad;
    public float arcLength;
    public float arcVar, inaccuracy;
    public int maxArcs;
    public float specialTime;
    [SerializeField]
    private IEnumerator fade;
    private bool abilityActivation = false;

    new void Awake() {
        base.Awake();
        fade = FadeOut();
        beamTurret = this;
        teslaTurret = this;
        maxFireRate = (fireRate + (ugB.upgradeFactorX * 3)) * manualFirerateMultiplier;
    }

    new void Update() {
        base.Update();

        if(hasSpecial && manual) {
            if(Input.GetMouseButtonDown(1)) {
                ActivateSpecial();
            }
        }

        if(specialBar.fillBar.fillAmount <= 0) {
            specialActivated = false;
        }
    }

    public override void AutoShoot() {
        if(!FindEnemy(false)) {
            BeamOff();
            return;
        }

        if(nextFire <= 0.0f) {
            nextFire = 1 / fireRate;
        } else {
            return;
        }

        //aim at target
        RotateOnShoot();
        gfxAnim.SetTrigger(shootAnim);

        //particle effects
        shootEffect.Play();
        shootLight.enabled = true;

        //build gfx
        if(!abilityActivation) {
            BuildLightning();
            targetEnemy.TakeDamage(damage, Color.yellow, true);
            targetEnemy.Stun(stunDuration);
        } else {
            Enemy[] enemies = BuildSuperChargedLightning();
            foreach(var e in enemies) {
                e.TakeDamage(damage, Color.yellow, true);
                e.Stun(stunDuration);
            }
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

        RaycastHit hit;
        if(Physics.Raycast(fireSpawn.position, pivot.forward, out hit, range * manualRangeMultiplier)) {
            if(hit.collider) {
                nextFire = 1 / manualFireRate;
                target = hit.transform;
                targetEnemy = target.GetComponent<Enemy>();

                shake.shakeDuration = 0.1f;

                if(!abilityActivation) {
                    BuildLightning();
                    if(targetEnemy) {
                        gfxAnim.SetTrigger(shootAnim);
                        targetEnemy.TakeDamage(damage, Color.yellow, true);
                        targetEnemy.Stun(stunDuration);
                    }
                } else {
                    Enemy[] enemies = BuildSuperChargedLightning();
                    gfxAnim.SetTrigger(shootAnim);
                    foreach(var e in enemies) {
                        e.TakeDamage(damage, Color.yellow, true);
                        e.Stun(stunDuration);
                    }
                }
                AudioManager.StaticPlayEffect(AudioManager.instance.sounds, shootSound, transform.position);

                if(!lineRenderer.enabled) {
                    lineRenderer.enabled = true;
                }

                //particle effects
                shootEffect.Play();
                shootLight.enabled = true;

                Fade();
            }
        }
    }

    void BuildLightning() {
        RestoreAlpha();

        var lastPoint = fireSpawn.position;
        var lineVert = 1;

        lineRenderer.SetPosition(0, lastPoint);    //make the origin of the lineRenderer the same as the transform

        while(Vector3.Distance(target.position, lastPoint) > 4) {    //was the last arc not close to the target
            lineRenderer.positionCount = lineVert + 1;     //new vertex
            var fwd = target.position - lastPoint;    //gives the direction to our target from the end of the last arc
            fwd.Normalize();
            fwd = RandomizeArc(fwd, inaccuracy);   //we don't want a straight line to the target though
            fwd *= Random.Range(arcLength * arcVar, arcLength);     //nature is never too uniform
            fwd += lastPoint;   //point + distance * direction = new point. this is where our new arc ends
            if(fwd.y < 1.6f) { //point must be above the ground
                fwd.y = 1.6f;
            }
            lineRenderer.SetPosition(lineVert, fwd);
            lineVert++;
            lastPoint = fwd;    //so we know where we are starting from for the next arc
        }

        lineRenderer.SetPosition(lineRenderer.positionCount - 1, target.position); //last point is always the target
        var tmp = Instantiate(flash, target.position, Quaternion.identity);
        Destroy(tmp, 1.0f);

        if(!lineRenderer.enabled)
            lineRenderer.enabled = true;
    }

    Enemy[] BuildSuperChargedLightning() {
        List<Enemy> enemyList = new List<Enemy>();

        Vector3 startPos;

        //find first target to hit
        if(FindEnemy(false)) {
            startPos = target.position;
            enemyList.Add(targetEnemy);

        } else
            return null; //no close enough target, abort special

        //keep trying to find new enemies to hit in range of current enemy until max enemies is hit or no more enemies are found
        for(int i = 0; i < maxArcs; i++) {
            var closest = float.MaxValue;

            //find the closest enemy in the radius
            Enemy next = null;

            foreach(var c in Physics.OverlapSphere(startPos, 10)) {

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
                var tmp = Instantiate(flash, target.position, Quaternion.identity);
                Destroy(tmp, 1.0f);
            }
        }

        if(!lineRenderer.enabled)
            lineRenderer.enabled = true;

        enemyList = Shuffle(enemyList);

        return enemyList.ToArray();
    }

    void RestoreAlpha() {
        //bring alpha back up to 1
        Color start = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g, lineRenderer.startColor.b, 1);
        Color end = new Color(lineRenderer.endColor.r, lineRenderer.endColor.g, lineRenderer.endColor.b, 1);
        lineRenderer.startColor = start;
        lineRenderer.endColor = end;
    }

    public override bool ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0 && BuildSuperChargedLightning() != null) {
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
        damage += ugB.upgradeFactorY;
        lineRenderer.startWidth = lineRenderer.endWidth += 0.2f;
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

            start.a = Mathf.Lerp(start.a, -1, Time.unscaledDeltaTime);
            end.a = Mathf.Lerp(end.a, -1, Time.unscaledDeltaTime);

            lineRenderer.startColor = start;
            lineRenderer.endColor = end;

            yield return null;
        }
        shootLight.enabled = false;
    }

    IEnumerator SpecialAbility() {
        StartCoroutine(SpecialTime());
        abilityActivation = true;
        lineRenderer.colorGradient = specGrad;
        var tmpFR = fireRate;
        fireRate *= superFirerate;
        nextFire = 0;
        yield return new WaitForSeconds(specialTime);
        fireRate = tmpFR;
        abilityActivation = false;
        lineRenderer.colorGradient = mainGrad;
    }
}