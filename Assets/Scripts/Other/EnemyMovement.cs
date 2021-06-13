using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour {

    private Transform target;
    private int wayPointIndex = 0;
    private Enemy enemy;

    private void Awake() {
        enemy = GetComponent<Enemy>();        
    }

    // Start is called before the first frame update
    public void Initialize() {
        target = Paths.GetPathWaypoints(enemy.pathIndex)[wayPointIndex];
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 direction = target.position - transform.position;
        Vector3 curPos = transform.position;
        transform.Translate(direction.normalized * enemy.currentSpeed * Time.deltaTime, Space.World);

        enemy.distanceTravelled += Vector3.Distance(curPos, transform.position);
        enemy.percentTrackCompleted = enemy.distanceTravelled / Paths.GetPathLength(enemy.pathIndex);

        if(Vector3.Distance(transform.position, target.position) <= 0.5f) {
            GetNextWayPoint();
        }
    }

    void GetNextWayPoint() {
        if(wayPointIndex >= Paths.GetPathWaypoints(enemy.pathIndex).Count - 1) {
            ReachEndPath();
            return;
        }
        wayPointIndex++;
        target = Paths.GetPathWaypoints(enemy.pathIndex)[wayPointIndex];
    }

    public void ResetPath() {
        wayPointIndex = 0;
        target = null;
    }

    void ReachEndPath() {
        PlayerStats.lives -= enemy.lifeValue;
        PlayerStats.lives = Mathf.Clamp(PlayerStats.lives, 0, PlayerStats.maxLives);
        WaveSpawner.RemoveEnemyFromList_Static(enemy);
        WaveSpawner.enemiesAlive--;
        ObjectPool.instance.ActivateEffect(enemy.deathEffect, transform.position, Quaternion.identity, 1.0f);
        ResetPath();
        ObjectPool.instance.Deactivate(gameObject);
    }
}
