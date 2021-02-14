using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {
    public static int enemiesAlive = 0, maxWaves, currentWave;

    public Wave[] waves;
    public Transform spawnPoint;
    public GameManager gameManager;
    public Button startButton;
    public GameObject remainingText, simple, tank, quick, boss;

    [SerializeField]
    private int waveIndex = 0;
    private bool waveStarted;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public static WaveSpawner instance;

    void Start() {
        enemiesAlive = currentWave = 0;
        maxWaves = waves.Length;
        waveStarted = false;
        if(instance) {
            Debug.LogError("more than 1 wavespawner, deleting current");
            Destroy(this);
            return;
        }
        instance = this;
    }

    void Update() {
        if(enemiesAlive > 0)
            return;

        if(!GameMode.survival) {
            if(waveIndex == waves.Length) {
                gameManager.WinLevel();
                this.enabled = false; //disables 'this' script 
            }
        }

        if(enemiesAlive == 0) {
            startButton.gameObject.SetActive(true);
            remainingText.SetActive(false);
            if(waveStarted) {
                waveStarted = false;
                PlayerStats.money += 100 + (waveIndex * 5);
                currentWave++;
            }
        }

        if(GameMode.survival && enemiesAlive == 0) {
            StartWave();
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            StartWave();
        }
    }

    void SpawnEnemy(GameObject enemy) {
        spawnedEnemies.Add(Instantiate(enemy, spawnPoint.position, spawnPoint.rotation));
    }

    void RemoveEnemyFromList(Enemy e) {
        spawnedEnemies.Remove(e.gameObject);
        spawnedEnemies.TrimExcess();
        if(spawnedEnemies.Count <= 0) {
            spawnedEnemies = new List<GameObject>();
        }
    }

    public static void RemoveEnemyFromList_Static(Enemy e) {
        instance.RemoveEnemyFromList(e);
    }

    List<GameObject> GetEnemyList() {
        return spawnedEnemies;
    }

    public static List<GameObject> GetEnemyList_Static() {
        return instance.GetEnemyList();
    }

    public void StartWave() {
        startButton.gameObject.SetActive(false);
        gameManager.LastControlled();
        remainingText.SetActive(true);
        if(!GameMode.survival) {
            StartCoroutine(SpawnWave());
        } else {
            StartCoroutine(SurvivalWaves());
        }
    }

    IEnumerator SpawnWave() {
        PlayerStats.rounds++;

        Wave wave = waves[waveIndex];

        enemiesAlive = wave.count;
        waveStarted = true;

        for(int i = 0; i < wave.count; i++) {
            SpawnEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(1 / wave.spawnRate);
        }
        waveIndex++;
    }

    //Survival//

    IEnumerator SurvivalWaves() {
        PlayerStats.rounds++;
        waveStarted = true;
        waveIndex++;

        Wave wave = new Wave();
        MakeWave(wave);
        for(int i = 0; i < wave.count; i++) {
            SpawnEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(1 / wave.spawnRate);
        }
    }

    void IncreaseDifficulty(float factor) {
        IncreaseEnemy(simple.GetComponent<Enemy>(), factor);
        IncreaseEnemy(tank.GetComponent<Enemy>(), factor);
        IncreaseEnemy(quick.GetComponent<Enemy>(), factor);
        IncreaseEnemy(boss.GetComponent<Enemy>(), factor);
    }

    void IncreaseEnemy(Enemy enemy, float factor) {
        enemy.startHp += enemy.startHp * factor;
        enemy.startSpeed += enemy.startSpeed * (factor / 2);
    }

    void MakeWave(Wave wave) {
        if(waveIndex % 10 == 0) {                           //boss round
            wave.enemyPrefab = boss;
            wave.count = Mathf.CeilToInt(waveIndex / 10);
            wave.spawnRate = waveIndex * 0.01f;
            IncreaseDifficulty(0.25f);
        } else if(waveIndex % 4 == 0) {                       //tank
            wave.enemyPrefab = tank;
            wave.count = waveIndex;
            wave.spawnRate = waveIndex * 0.2f;
        } else if(waveIndex % 3 == 0) {                       //quick
            wave.enemyPrefab = quick;
            wave.count = Mathf.CeilToInt(waveIndex * 1.25f);
            wave.spawnRate = waveIndex * 0.5f;
        } else {                                              //simple
            wave.enemyPrefab = simple;
            wave.count = Mathf.CeilToInt(waveIndex * 5);
            wave.spawnRate = waveIndex;
        }
        enemiesAlive = wave.count;
    }
}
