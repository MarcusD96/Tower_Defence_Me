using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour {

    public Transform target;
    public int wayPointIndex = 0;
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

        if(enemy.blowBack == null)
            MoveForward(curPos);
        else
            MoveBack(curPos);
    }

    void MoveForward(Vector3 curPos_) {
        enemy.distanceTravelled += Vector3.Distance(curPos_, transform.position);
        enemy.percentTrackCompleted = enemy.distanceTravelled / Paths.GetPathLength(enemy.pathIndex);

        if(Vector3.Distance(transform.position, target.position) <= 0.5f)
            GetNextWayPoint();
    }

    void MoveBack(Vector3 curPos_) {
        enemy.distanceTravelled -= Vector3.Distance(curPos_, transform.position);
        enemy.percentTrackCompleted = enemy.distanceTravelled / Paths.GetPathLength(enemy.pathIndex);

        if(Vector3.Distance(transform.position, target.position) <= 0.5f)
            GetPreviousWayPoint();
    }

    void GetNextWayPoint() {
        if(wayPointIndex >= Paths.GetPathWaypoints(enemy.pathIndex).Count - 1) {
            ReachEndPath();
            return;
        }
        wayPointIndex++;
        target = Paths.GetPathWaypoints(enemy.pathIndex)[wayPointIndex];
    }

    void GetPreviousWayPoint() {
        wayPointIndex--;
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
