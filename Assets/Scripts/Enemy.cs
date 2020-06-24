using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    public GameObject deathEffect;

    public float startSpeed, startHp;

    [HideInInspector]
    public float currentHp, speed;

    public int moneyValue;

    [Header("Unity Stuff")]
    public Image healthBar;

    private bool isDead = false;

    void Start() {
        speed = startSpeed;
        currentHp = startHp;
    }

    public void TakeDamage(float amount) {
        currentHp -= amount;
        healthBar.fillAmount = currentHp / startHp;

        if(currentHp <= 0 && !isDead)
            Die();
    }

    private void Die() {
        isDead = true;
        PlayerStats.money += moneyValue;
        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5.0f);

        WaveSpawner.enemiesAlive--;
        Destroy(gameObject);
    }

    public void Slow(float slowFactor) {
        speed = startSpeed * slowFactor;
    }
}
