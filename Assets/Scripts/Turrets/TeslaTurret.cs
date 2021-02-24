using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaTurret : BeamTurret {

    [Header("Tesla")]
    public float damage;
    public float stunDuration, superFirerate;

    [Header("Lightning")]
    public GameObject flash;
    public float arcLength;
    public float arcVar, inaccuracy;
    public int maxArcs;

    public float specialTime;
    private bool abilityActivation = false;

    new void Awake() {
        base.Awake();
        beamTurret = this;
        teslaTurret = this;
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
        if(nextFire <= 0.0f) {
            nextFire = 1 / fireRate;
        } else {
            return;
        }

        //aim at target
        RotateOnShoot();
        gfxAnim.SetTrigger(shootAnim);

        //build gfx
        if(!abilityActivation) {
            BuildLightning();
            targetEnemy.TakeDamage(damage, true);
            targetEnemy.Stun(stunDuration);
        } else {
            Enemy[] enemies = BuildSuperchargedLightning();
            foreach(var e in enemies) {
                e.TakeDamage(damage, true);
                e.Stun(stunDuration);
            }
        }
        AudioManager.PlaySound(shootSound, transform.position);

        StopCoroutine("FadeOut");
        StartCoroutine(FadeOut());
    }

    public override void ManualShoot() {
        //gotta do the reverse since we cannot reset the timer if the shot is missed.
        if(nextFire >= 0) {
            return;
        }

        var manualFireRate = fireRate;
        if(!hasSpecial) {
            manualFireRate = fireRate * manualFirerateMultiplier;
        }

        RaycastHit hit;
        if(Physics.Raycast(turretCam.transform.position, pivot.forward, out hit, range * manualRangeMultiplier)) {
            if(hit.collider) {
                nextFire = 1 / manualFireRate;
                target = hit.transform;
                targetEnemy = target.GetComponent<Enemy>();

                if(!abilityActivation) {
                    BuildLightning();
                    if(targetEnemy) {
                        gfxAnim.SetTrigger(shootAnim);
                        targetEnemy.TakeDamage(damage, true);
                        targetEnemy.Stun(stunDuration);
                    }
                } else {
                    Enemy[] enemies = BuildSuperchargedLightning();
                    gfxAnim.SetTrigger(shootAnim);
                    foreach(var e in enemies) {
                        e.TakeDamage(damage, true);
                        e.Stun(stunDuration);
                    }
                }
                AudioManager.PlaySound(shootSound, transform.position);

                if(!lineRenderer.enabled) {
                    lineRenderer.enabled = true;
                }

                StopCoroutine("FadeOut");
                StartCoroutine(FadeOut());
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

    Enemy[] BuildSuperchargedLightning() {
        List<Enemy> targetList = new List<Enemy>();

        //finding and sorting enemies
        {
            //find all the enemies on screen
            foreach(var e in GameObject.FindGameObjectsWithTag(enemyTag)) {
                targetList.Add(e.GetComponent<Enemy>());
            }
            //sort them by how far they have travelled
            targetList.Sort((a, b) => { return b.distanceTravelled.CompareTo(a.distanceTravelled); });

            //reduce list to only the first max enemies
            while(targetList.Count > maxArcs) {
                targetList.RemoveAt(targetList.Count - 1);
                targetList.TrimExcess();
            }
            targetList.Capacity = targetList.Count;

            //randomize the order of the filtered list
            targetList = Shuffle(targetList);
        }

        //build the lightning using the positions of the targets
        {
            RestoreAlpha();

            var lastPoint = fireSpawn.position; //the first point
            var lineVert = 1;
            lineRenderer.SetPosition(0, lastPoint); //the first point is the firing position

            for(int currentT = 0; currentT < targetList.Count; currentT++) {
                target = targetList[currentT].transform;

                while(Vector3.Distance(target.position, lastPoint) > 4) {
                    lineRenderer.positionCount = lineVert + 1;
                    var fwd = target.position - lastPoint;
                    fwd.Normalize();
                    fwd = RandomizeArc(fwd, inaccuracy);
                    fwd *= UnityEngine.Random.Range(arcLength * arcVar, arcLength);
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

        //return the list of filtered enemies so we can damage and stun them
        return targetList.ToArray();
    }

    void RestoreAlpha() {
        //bring alpha back up to 1
        Color start = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g, lineRenderer.startColor.b, 1);
        Color end = new Color(lineRenderer.endColor.r, lineRenderer.endColor.g, lineRenderer.endColor.b, 1);
        lineRenderer.startColor = start;
        lineRenderer.endColor = end;
    }

    public override void ActivateSpecial() {
        if(!specialActivated && WaveSpawner.enemiesAlive > 0 && CheckEnemiesInRange()) {
            specialActivated = true;
            StartCoroutine(SpecialAbility());
        }
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
        fireRate += ugB.upgradeFactorX;
        damage += ugB.upgradeFactorY;
    }

    IEnumerator FadeOut() {
        while(lineRenderer.startColor.a > 0) {
            Color start = lineRenderer.startColor;
            Color end = lineRenderer.endColor;

            start.a = Mathf.Lerp(start.a, -1, Time.deltaTime);
            end.a = Mathf.Lerp(end.a, -1, Time.deltaTime);

            lineRenderer.startColor = start;
            lineRenderer.endColor = end;

            yield return null;
        }
    }

    IEnumerator SpecialAbility() {
        StartCoroutine(SpecialTime());
        abilityActivation = true;
        var tmpFR = fireRate;
        fireRate *= superFirerate;
        nextFire = 0;
        yield return new WaitForSeconds(specialTime);
        fireRate = tmpFR;
        abilityActivation = false;
    }
}