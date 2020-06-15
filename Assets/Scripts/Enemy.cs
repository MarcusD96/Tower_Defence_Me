using UnityEngine;

public class Enemy : MonoBehaviour {

    public GameObject deathEffect;
    public float speed = 10.0f;
    public int hp, moneyValue = 10;

    private Transform target;
    private int wayPointIndex = 0;

    // Start is called before the first frame update
    void Start() {
        target = Path.waypoints[0];
    }

    // Update is called once per frame
    void Update() {
        Vector3 direction = target.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        if(Vector3.Distance(transform.position, target.position) <= 0.4f) {
            GetNextWayPoint();
        }
    }

    public void TakeDamage(int amount) {
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

    void GetNextWayPoint() {
        if(wayPointIndex >= Path.waypoints.Capacity - 1) {
            EndPath();
            return;
        }
        wayPointIndex++;
        target = Path.waypoints[wayPointIndex];
    }

    void EndPath() {
        PlayerStats.lives--;
        PlayerStats.lives = Mathf.Clamp(PlayerStats.lives, 0, PlayerStats.maxLives);
        Destroy(gameObject);
    }
}
