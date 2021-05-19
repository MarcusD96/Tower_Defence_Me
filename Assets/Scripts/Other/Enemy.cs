using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    public static float difficultyMultiplier = 1.0f;

    public GameObject deathEffect, indicator;
    public ParticleSystem slowEffect, stunEffect, burnEffect;

    public float baseSpeed, baseHp;
    public float startSpeed, startHp;

    public float currentHp, currentSpeed, distanceTravelled, moneyValue, currentMoneyValue;
    public int lifeValue;

    public EnemyType enemyType;

    private IEnumerator stun = null;
    private IEnumerator burn = null;

    [Header("Unity Stuff")]
    public Image healthBarL;
    public Image healthBarR;
    public bool superSlow, isDamaging, isDead, isSlow;

    [Header("Resistances")]
    public bool isBoss;
    public bool stunResist, slowResist, burnResist;

    void Awake() {
        startHp = baseHp * difficultyMultiplier;
        startSpeed = baseSpeed * difficultyMultiplier;

        currentSpeed = baseSpeed * difficultyMultiplier;
        currentHp = Mathf.RoundToInt(baseHp * difficultyMultiplier);

        currentMoneyValue = moneyValue;
        distanceTravelled = 0.0f;

        superSlow = isDamaging = isSlow = false;
        isDead = true;

        if(slowEffect != null) {
            slowEffect.gameObject.SetActive(false);
        }
        if(stunEffect != null) {
            stunEffect.gameObject.SetActive(false);
        }
        if(burnEffect != null) {
            burnEffect.gameObject.SetActive(false);
        }
    }

    public void DecreaseMoneyValue() {
        currentMoneyValue /= 2.0f;
    }

    public void TakeDamage(float amount, Color color, bool indicateDmg) {
        if(healthBarL && healthBarR) {
            currentHp -= amount;

            //if(indicateDmg) {
            //    GameObject indicatorInstance = Instantiate(indicator, transform.position, Quaternion.identity);
            //    var i = indicatorInstance.GetComponent<DamageIndicator>();
            //    i.damage = amount;
            //    i.color = color;
            //}

            healthBarL.fillAmount = currentHp / baseHp;
            healthBarR.fillAmount = currentHp / baseHp;

            if(currentHp <= 0 && !isDead)
                Die();
        }
    }

    public void Die() {
        isDead = true;
        PlayerStats.money += currentMoneyValue;
        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2.0f);

        WaveSpawner.enemiesAlive--;
        WaveSpawner.RemoveEnemyFromList_Static(this);
        GetComponent<EnemyMovement>().ResetPath();
        ResetEnemy();
        EnemyPool.instance.Deactivate(gameObject);
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
            duration /= 2;
            slowFactor *= 2;
        }

        if(!isSlow) { //first slow, start to slow and start particles
            StartCoroutine(SlowEnemy(slowFactor, duration));
            slowEffect.gameObject.SetActive(true);
            slowEffect.Play();
            isSlow = true;
        }
    }

    public void DamageOverTime(float dot, float duration) {
        if(isBoss)
            duration /= 2;

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
            TakeDamage(dot * Time.deltaTime, Color.white, false);
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
            TakeDamage(damage, Color.white, false);
        }
        burnEffect.Stop();
        burnEffect.gameObject.SetActive(false);
    }

    public void ResetEnemy() {
        stun = burn = null;
        Awake();
        healthBarL.fillAmount = healthBarR.fillAmount = 1;
        StopAllCoroutines();
    }
}
