using UnityEngine;

public class Enemy : MonoBehaviour {

    public GameObject deathEffect;
    public float startSpeed = 10.0f, hp;
    [HideInInspector]
    public float speed;
    public int moneyValue = 10;

    void Start() {
        speed = startSpeed;
    }

    public void TakeDamage(float amount) {
        hp -= amount;

        if(hp <= 0)
            Die();
    }

    private void Die() {
        PlayerStats.money += moneyValue;
        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5.0f);
        Destroy(gameObject);
    }

    public void Slow(float slowFactor) {
        speed = startSpeed * slowFactor;
    }
}
