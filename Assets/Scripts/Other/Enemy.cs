using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    public static float difficultyMultiplier = 1.0f;

    public GameObject indicator;
    public EffectType deathEffect;
    public ParticleSystem slowEffect, stunEffect, burnEffect;

    public float baseSpeed, baseHp;
    public float startSpeed, startHp;
    public float moneyValue, percentTrackCompleted;
    //[HideInInspector]
    public float currentSpeed, currentMoneyValue, distanceTravelled;

    public int lifeValue;
    public int pathIndex;

    public EnemyType enemyType;

    private float currentHp;

    private IEnumerator stun = null;
    private IEnumerator burn = null;
    [HideInInspector]
    public IEnumerator blowBack = null;

    [Header("Unity Stuff")]
    public Image healthBarL;
    public Image healthBarR;
    public bool superSlow, isDamaging, isSlow;
    [HideInInspector]
    public bool isDead;

    [Header("Resistances")]
    public bool stunResist;
    public bool slowResist, burnResist, isBoss;

    private void Awake() {
        currentMoneyValue = moneyValue;
    }

    public void TakeDamage(float amount, Color color) {
        if(healthBarL && healthBarR) {
            currentHp -= amount;

            healthBarL.fillAmount = currentHp / baseHp;
            healthBarR.fillAmount = currentHp / baseHp;

            if(currentHp <= 0 && !isDead)
                GetKilled();
        }
    }

    public void GetKilled() {
        isDead = true;
        PlayerStats.money += currentMoneyValue;
        ObjectPool.instance.ActivateEffect(deathEffect, transform.position, Quaternion.identity, 1.0f);

        WaveSpawner.enemiesAlive--;
        WaveSpawner.RemoveEnemyFromList_Static(this);
        GetComponent<EnemyMovement>().ResetPath();
        ObjectPool.instance.Deactivate(gameObject);
        ResetEnemy();
    }

    public void SlowAura(float slowFactor) {
        superSlow = true;
        currentSpeed = baseSpeed * slowFactor;

        if(!slowEffect.isPlaying) {
            slowEffect.gameObject.SetActive(true);
            slowEffect.Play(); 
            isSlow = true;
        }
    }

    public void RestoreSlow() {
        superSlow = false;
        currentSpeed = baseSpeed;

        slowEffect.gameObject.SetActive(false);
        isSlow = false;
    }

    int slowLevel = -1;
    public void Slow(float slowFactor, float duration, int level_) {
        if(slowEffect == null) {
            print("no slow effect");
            return;
        }

        if(level_ < slowLevel) {
            return;
        } else {
            slowLevel = level_;
        }

        if(!gameObject.activeSelf)
            return;

        if(slowResist)
            return;

        if(isBoss) {
            slowFactor *= 3.0f;
            slowFactor = Mathf.Clamp01(slowFactor);
        }

        if(!isSlow) { //first slow, start to slow and start particles
            StartCoroutine(SlowEnemy(slowFactor, duration));
            slowEffect.gameObject.SetActive(true);
            slowEffect.Play();
            isSlow = true;
        }
    }

    public void DamageOverTime(float dot, float duration) {
        if(gameObject.activeSelf)
            StartCoroutine(DoT(dot, duration));
    }

    IEnumerator SlowEnemy(float slowFactor, float duration) {
        isSlow = true;
        currentSpeed = baseSpeed * slowFactor;

        yield return new WaitForSeconds(duration);

        //slowEffect.Stop();
        slowEffect.gameObject.SetActive(false);

        currentSpeed = baseSpeed;
        superSlow = false;
        slowLevel = -1;
        isSlow = false;
    }

    IEnumerator DoT(float dot, float duration) {
        isDamaging = true;
        float endTime = Time.time + duration;
        while(Time.time < endTime) {
            TakeDamage(dot * Time.deltaTime, Color.white);
            yield return new WaitForEndOfFrame();
        }
        isDamaging = false;
    }

    int stunLevel = -1;
    public void Stun(float duration, int level_) {
        if(stunEffect == null) {
            print("no stun effect");
            return;
        }

        if(level_ < stunLevel) {
            return;
        } else {
            stunLevel = level_;
        }

        if(!gameObject.activeSelf)
            return;

        if(stunResist)
            return;

        if(isBoss)
            duration /= 2;

        if(stun != null) {
            stunEffect.gameObject.SetActive(false);
            currentSpeed = baseSpeed;
            StopCoroutine(stun);
        }
        stun = StunEnemy(duration);
        StartCoroutine(stun);
    }

    IEnumerator StunEnemy(float duration) {
        currentSpeed = 0;

        stunEffect.gameObject.SetActive(true);
        stunEffect.Play();

        yield return new WaitForSeconds(duration);

        stunEffect.Stop();
        stunEffect.gameObject.SetActive(false);

        currentSpeed = baseSpeed;
        stunLevel = -1;
        stun = null;
    }

    int burnLevel = -1;
    public void Burn(float damage, int numBurns, float burnInterval, int level_) {
        if(burnEffect == null) {
            print("no burn effect");
        }

        if(level_ < burnLevel) {
            return;
        } else {
            burnLevel = level_;
        }

        if(burnResist)
            return;

        if(!gameObject.activeSelf)
            return;

        if(isBoss)
            numBurns /= 2;

        //if enemy is currently burning
        if(burn != null) {
            //burnEffect.gameObject.SetActive(false);
            StopCoroutine(burn);
        }

        burn = BurnEnemy(damage, numBurns, burnInterval);
        StartCoroutine(burn);
        if(!burnEffect.isPlaying) {
            burnEffect.Play();
        }
    }

    IEnumerator BurnEnemy(float damage, int numBurns, float burnInterval) {
        burnEffect.gameObject.SetActive(true);
        for(int i = 0; i <= numBurns; i++) {
            yield return new WaitForSeconds(burnInterval);
            TakeDamage(damage, Color.white);
        }
        burnLevel = -1;
        burnEffect.Stop();
        burnEffect.gameObject.SetActive(false);
    }

    [HideInInspector]
    public int blowBackLevel = -1;
    public void BlowBack(float duration, int level_) {
        if(level_ < blowBackLevel) {
            return;
        } else {
            blowBackLevel = level_;
        }

        if(!gameObject.activeSelf)
            return;

        if(blowBack != null) {
            return;
        }

        if(isBoss)
            duration /= 2;

        blowBack = BlowBackEnemy(duration);
        StartCoroutine(blowBack);
    }

    IEnumerator BlowBackEnemy(float duration) {
        var m = GetComponent<EnemyMovement>();
        m.wayPointIndex -= 1;
        if(m.wayPointIndex < 0)
            m.wayPointIndex = 0;
        m.target = Paths.GetPathWaypoints(pathIndex)[m.wayPointIndex];
        currentSpeed *= 2;

        float endTime = Time.time + duration;
        while(Time.time < endTime) {
            if(m.wayPointIndex < 1)
                break;
            yield return null;
        }

        m.wayPointIndex += 1;
        m.target = Paths.GetPathWaypoints(pathIndex)[m.wayPointIndex];
        currentSpeed /= 2;
        blowBackLevel = -1;
        blowBack = null;
    }

    public void ResetEnemy() {
        startHp = baseHp * difficultyMultiplier;
        startSpeed = baseSpeed * difficultyMultiplier;

        currentSpeed = baseSpeed * difficultyMultiplier;
        currentHp = Mathf.RoundToInt(baseHp * difficultyMultiplier);

        distanceTravelled = percentTrackCompleted = 0.0f;

        superSlow = isDamaging = isSlow = false;

        stun = burn = blowBack = null;
        StopAllCoroutines();
        stunLevel = burnLevel = slowLevel = blowBackLevel = -1;

        if(slowEffect != null) {
            slowEffect.gameObject.SetActive(false);
        }
        if(stunEffect != null) {
            stunEffect.gameObject.SetActive(false);
        }
        if(burnEffect != null) {
            burnEffect.gameObject.SetActive(false);
        }

        healthBarL.fillAmount = healthBarR.fillAmount = 1;
    }
}
