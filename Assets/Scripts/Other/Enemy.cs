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
    public float distanceTravelled, moneyValue;
    [HideInInspector]
    public float currentSpeed, currentMoneyValue;

    public int lifeValue;

    public EnemyType enemyType;

    private float currentHp;

    private IEnumerator stun = null;
    private IEnumerator burn = null;

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
                Die();
        }
    }

    public void Die() {
        isDead = true;
        PlayerStats.money += currentMoneyValue;
        ObjectPool.instance.ActivateEffect(deathEffect, transform.position, Quaternion.identity);

        WaveSpawner.enemiesAlive--;
        WaveSpawner.RemoveEnemyFromList_Static(this);
        GetComponent<EnemyMovement>().ResetPath();
        ObjectPool.instance.Deactivate(gameObject);
        ResetEnemy();
    }

    public void Slow(float slowFactor, float duration) {
        if(slowEffect == null) {
            print("no slow effect");
            return;
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

        slowEffect.Stop();
        slowEffect.gameObject.SetActive(false);

        currentSpeed = baseSpeed;
        superSlow = false;
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

    public void Stun(float duration) {
        if(stunEffect == null) {
            print("no stun effect");
            return;
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
        stun = null;
    }

    public void Burn(float damage, int numBurns, float burnInterval) {
        if(burnEffect == null) {
            print("no burn effect");
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
        burnEffect.Stop();
        burnEffect.gameObject.SetActive(false);
    }

    public void ResetEnemy() {
        startHp = baseHp * difficultyMultiplier;
        startSpeed = baseSpeed * difficultyMultiplier;

        currentSpeed = baseSpeed * difficultyMultiplier;
        currentHp = Mathf.RoundToInt(baseHp * difficultyMultiplier);

        distanceTravelled = 0.0f;

        superSlow = isDamaging = isSlow = false;

        stun = burn = null;
        StopAllCoroutines();

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
