using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    public GameObject deathEffect;
    public ParticleSystem slowEffect, stunEffect;

    public float startSpeed, startHp;

    [HideInInspector]
    public float currentHp, speed;
    public float distanceTravelled = 0.0f;

    public int moneyValue, lifeValue;

    [Header("Unity Stuff")]
    public Image healthBar;

    private bool isDead = false, isSlowed = false, isStunned = false;

    void Start() {
        speed = startSpeed;
        currentHp = startHp;
        if(slowEffect) {
            slowEffect.gameObject.SetActive(false); 
        }
        if(stunEffect) {
            stunEffect.gameObject.SetActive(false); 
        }
    }

    public void TakeDamage(float amount) {
        if(healthBar) {
            currentHp -= amount;
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
        Destroy(gameObject);
    }

    public void Slow(float slowFactor, float duration) {
        if(!slowEffect)
            return;
        if(!isSlowed)
            StartCoroutine(SlowEnemy(slowFactor, duration));
    }

    public void Stun(float duration) {
        if(!stunEffect)
            return;
        if(!isStunned)
            StartCoroutine(StunEnemy(duration));
    }

    IEnumerator SlowEnemy(float slowFactor, float duration) {
        isSlowed = true;
        speed = startSpeed * slowFactor;
        slowEffect.gameObject.SetActive(true);
        slowEffect.Play();
        yield return new WaitForSeconds(duration);
        slowEffect.Stop();
        slowEffect.gameObject.SetActive(false);
        speed = startSpeed;
        isSlowed = false;
    }

    IEnumerator StunEnemy(float duration) {
        isStunned = true;
        speed = 0;
        stunEffect.gameObject.SetActive(true);
        stunEffect.Play();
        yield return new WaitForSeconds(duration);
        stunEffect.Stop();
        stunEffect.gameObject.SetActive(false);
        speed = startSpeed;
        isStunned = false;
    }
}
