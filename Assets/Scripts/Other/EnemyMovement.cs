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
    void LateUpdate() {
        Vector3 direction = target.position - transform.position;
        Vector3 curPos = transform.position;
        transform.Translate(direction.normalized * enemy.currentSpeed * Time.deltaTime, Space.World);

        enemy.distanceTravelled += Vector3.Distance(curPos, transform.position);

        if(Vector3.Distance(transform.position, target.position) <= 0.5f) {
            GetNextWayPoint();
        }
    }

    void GetNextWayPoint() {
        if(wayPointIndex >= Path.waypoints.Capacity - 1) {
            EndPath();
            return;
        }
        wayPointIndex++;
        target = Path.waypoints[wayPointIndex];
    }

    public void ResetPath() {
        wayPointIndex = 0;
        target = Path.waypoints[0];
        enemy.ResetEnemy();
    }

    void EndPath() {
        PlayerStats.lives -= enemy.lifeValue;
        PlayerStats.lives = Mathf.Clamp(PlayerStats.lives, 0, PlayerStats.maxLives);
        WaveSpawner.RemoveEnemyFromList_Static(enemy);
        WaveSpawner.enemiesAlive--;
        Destroy(Instantiate(enemy.deathEffect, transform.position, transform.rotation), 5.0f);
        ResetPath();
        EnemyPool.instance.Deactivate(gameObject);
    }
}
