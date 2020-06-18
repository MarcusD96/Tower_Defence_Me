using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour {

    private Transform target;
    private int wayPointIndex = 0;
    private Enemy enemy;

    // Start is called before the first frame update
    void Start() {
        target = Path.waypoints[0];
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update() {
        Vector3 direction = target.position - transform.position;
        transform.Translate(direction.normalized * enemy.speed * Time.deltaTime, Space.World);

        if(Vector3.Distance(transform.position, target.position) <= 0.4f) {
            GetNextWayPoint();
        }

        enemy.speed = enemy.startSpeed;
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
        WaveSpawner.enemiesAlive--;
        Destroy(gameObject);
    }

}
