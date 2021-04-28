using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {
    public static int enemiesAlive = 0, maxWaves, currentWave;

    public Wave[] waves;

    [SerializeField]
    private int waveIndex = 0;
    private Transform spawnPoint;
    private GameManager gameManager;
    private Button startButton;
    private GameObject remainingText;
    private bool waveStarted, autoStart, firstRoundStarted = false;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public static WaveSpawner instance;

    void Awake() {
        foreach(var s in FindObjectsOfType<Transform>(true)) {
            if(s.CompareTag("Respawn")) {
                spawnPoint = s;
                break;
            }
        }
        gameManager = GetComponent<GameManager>();
        foreach(var s in FindObjectsOfType<Button>(true)) {
            if(s.CompareTag("StartButton")) {
                startButton = s;
                break;
            }
        }
        foreach(var s in FindObjectsOfType<GameObject>(true)) {
            if(s.CompareTag("Remaining")) {
                remainingText = s;
                break;
            }
        }
    }

    void Start() {
        if(instance) {
            Debug.LogError("more than 1 wavespawner, deleting current");
            Destroy(this);
            return;
        }
        instance = this;

        InitializeWaves();
    }

    public void InitializeWaves() {
        //string s = File.ReadAllText(Application.dataPath + "/waves.json");
        //waves = JsonHelper.FromJson<Wave>(s);
        enemiesAlive = currentWave = 0;
        maxWaves = waves.Length;
        waveStarted = false;
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

        autoStart = Settings.AutoStart;
        if(enemiesAlive == 0 && autoStart && firstRoundStarted) {
            StartWave();
        }
    }

    void SpawnEnemy(GameObject enemy) {
        spawnedEnemies.Add(EnemyPool.instance.Activate(enemy.GetComponent<Enemy>().enemyType, spawnPoint.position, spawnPoint.rotation));
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
        if(firstRoundStarted == false)
            firstRoundStarted = true;
        startButton.gameObject.SetActive(false);
        gameManager.LastControlled();
        remainingText.SetActive(true);
        if(!GameMode.survival) {
            SpawnWave();
        } else {
            //StartCoroutine(SurvivalWaves());
            print("TODO: survival");
        }
    }

    void SpawnWave() {
        PlayerStats.rounds++;
        Wave wave = waves[waveIndex];
        waveStarted = true;
        enemiesAlive = wave.GetTotalEnemies();

        foreach(var c in wave.chunks) {
            StartCoroutine(SpawnChunk(c));
        }

        waveIndex++;
    }

    IEnumerator SpawnChunk(WaveChunk c) {
        yield return new WaitForSeconds(c.startDelay);
        for(int i = 0; i < c.count; i++) {
            SpawnEnemy(c.enemyPrefab);
            if(c.spawnRate > 0) {
                yield return new WaitForSeconds(1 / c.spawnRate);
            } else
                yield return new WaitForEndOfFrame();
        }
    }

    /*IEnumerator SpawnWave() {
        PlayerStats.rounds++;

        Wave wave = waves[waveIndex];

        enemiesAlive = wave.count;
        waveStarted = true;

        for(int i = 0; i < wave.count; i++) {
            SpawnEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(1 / wave.spawnRate);
        }
        waveIndex++;
    }*/

    /*#region Survival Stuff

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
        //enemy.startHp += enemy.startHp * factor;
        //enemy.startSpeed += enemy.startSpeed * (factor / 2);
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
    #endregion*/
}
