﻿using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour {

    private Transform target;
    private int wayPointIndex = 0;
    private Enemy enemy;

    // Start is called before the first frame update
    void Start() {
        target = Path.waypoints[0];
        enemy = GetComponent<Enemy>();
        enemy.speed = enemy.startSpeed;
    }

    // Update is called once per frame
    void Update() {
        Vector3 direction = target.position - transform.position;
        transform.Translate(direction.normalized * enemy.speed * Time.fixedDeltaTime * Time.timeScale, Space.World);

        enemy.distanceTravelled += (direction * enemy.speed * Time.fixedDeltaTime * Time.timeScale).magnitude;

        if(Vector3.Distance(transform.position, target.position) <= 0.4f) {
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

    void EndPath() {
        PlayerStats.lives -= enemy.lifeValue;
        PlayerStats.lives = Mathf.Clamp(PlayerStats.lives, 0, PlayerStats.maxLives);
        WaveSpawner.RemoveEnemyFromList_Static(enemy);
        WaveSpawner.enemiesAlive--;
        Destroy(Instantiate(enemy.deathEffect, transform.position, transform.rotation), 5.0f);
        Destroy(gameObject);
    }

}
