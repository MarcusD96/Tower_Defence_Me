using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    public static float speedDifficultyMultiplier = 1.0f, hpDifficultyMultiplier = 1.0f;

    public GameObject deathEffect, indicator;
    public ParticleSystem slowEffect, stunEffect;

    public float startSpeed, startHp;

    //[HideInInspector]
    public float currentHp, speed, distanceTravelled = 0.0f;
    public int moneyValue, lifeValue;

    private Coroutine slow;
    private IEnumerator stun = null;

    [Header("Unity Stuff")]
    public Image healthBar;
    public bool superSlow = false;
    private bool isDead = false, isSlow = false;

    [Header("Boss")]
    public bool stunResist;
    public bool slowResist;

    void Start() {
        speed = startSpeed * speedDifficultyMultiplier;
        startHp = currentHp = Mathf.RoundToInt(startHp * hpDifficultyMultiplier);

        if(slowEffect) {
            slowEffect.gameObject.SetActive(false);
        }
        if(stunEffect) {
            stunEffect.gameObject.SetActive(false);
        }
    }

    public void TakeDamage(float amount, bool indicateDmg) {
        if(healthBar) {
            currentHp -= amount;

            if(indicateDmg) {
                GameObject indicatorInstance = Instantiate(indicator, transform.position, Quaternion.identity);
                indicatorInstance.GetComponent<DamageIndicator>().damage = amount;
            }

            healthBar.fillAmount = currentHp / startHp;

            if(currentHp <= 0 && !isDead)
                Die();
        }
    }

    public void Die() {
        isDead = true;
        PlayerStats.money += moneyValue;
        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5.0f);

        WaveSpawner.enemiesAlive--;
        WaveSpawner.RemoveEnemyFromList_Static(this);
        Destroy(gameObject);
    }

    public void Slow(float slowFactor, float duration) {
        if(!slowEffect)
            return;
        if(slowResist)
            return;

        if(!isSlow) { //first slow, start to slow and start particles
            slow = StartCoroutine(SlowEnemy(slowFactor, duration));
            slowEffect.gameObject.SetActive(true);
            slowEffect.Play();
        } else { //restart the slow, but keep particles on
            StopAllCoroutines();
            slow = StartCoroutine(SlowEnemy(slowFactor, duration));
        }
    }

    IEnumerator SlowEnemy(float slowFactor, float duration) {
        speed = startSpeed * slowFactor;

        yield return new WaitForSeconds(duration);

        if(isSlow)
            yield break;

        slowEffect.Stop();
        slowEffect.gameObject.SetActive(false);

        speed = startSpeed;
        superSlow = false;
        isSlow = false;
        slow = null;
    }

    public void Stun(float duration) {
        if(!stunEffect)
            return;

        if(stunResist)
            return;

        if(stun != null) {
            stunEffect.gameObject.SetActive(false);
            speed = startSpeed;
            StopCoroutine(stun);
        }
        stun = StunEnemy(duration);
        StartCoroutine(stun);
    }

    IEnumerator StunEnemy(float duration) {
        speed = 0;

        stunEffect.gameObject.SetActive(true);
        stunEffect.Play();

        yield return new WaitForSeconds(duration);

        stunEffect.Stop();
        stunEffect.gameObject.SetActive(false);

        speed = startSpeed;
        stun = null;
    }
}
