using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {
    public static int enemiesAlive = 0, maxWaves, currentWave;

    [SerializeField]
    private List<GameObject> enemyTypes;

    [SerializeField]
    Wave[] waves;

    [SerializeField]
    private int waveIndex = 0;
    private Transform spawnPoint;
    private GameManager gameManager;
    private Button startButton, FFButton;
    private bool waveStarted, autoStart, firstRoundStarted = false;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public static WaveSpawner instance;

    void Awake() {
        if(instance) {
            Debug.LogError("more than 1 wavespawner, deleting current");
            Destroy(this);
            return;
        }
        instance = this;

        gameManager = GetComponent<GameManager>();
        foreach(var s in FindObjectsOfType<Button>(true)) {
            if(s.CompareTag("StartButton")) {
                startButton = s;
                break;
            }
        }
        foreach(var s in FindObjectsOfType<Button>(true)) {
            if(s.CompareTag("FF")) {
                FFButton = s;
                break;
            }
        }
    }

    void Start() {
        spawnPoint = Path.waypoints[0];
        InitializeWaves();
    }

    public void InitializeWaves() {
        string dataPath;
#if UNITY_EDITOR
        dataPath = Application.dataPath;
#else
        dataPath = Application.streamingAssetsPath;
        if(!Directory.Exists(dataPath)) {
            Directory.CreateDirectory(dataPath);
        }
#endif

        //set file
        File.WriteAllText(System.IO.Path.Combine(dataPath, "waves.json"), JsonHelper.ToJson(waves, true));

        //get file
        string s = File.ReadAllText(System.IO.Path.Combine(dataPath, "waves.json"));
        waves = JsonHelper.FromJson<Wave>(s);

        foreach(var w in waves) {
            w.WaveWorth();
        }

        //File.WriteAllText(System.IO.Path.Combine(dataPath, "waves.json"), JsonHelper.ToJson(waves, true));

        enemiesAlive = currentWave = 0;
        maxWaves = PlayerStats.maxRounds;
        waveStarted = false;
    }

    public Wave GetCurrentWave() {
        return waves[waveIndex];
    }

    GameObject ChooseEnemyPrefab(EnemyType e) {
        foreach(var t in enemyTypes) {
            if(t.GetComponent<Enemy>().enemyType == e) {
                return t;
            }
        }
        print("No enemy type found!");
        return null;
    }

    void LateUpdate() {
        if(enemiesAlive > 0)
            return;

        if(GameManager.gameEnd)
            return;

        if(!GameMode.survival) {
            if(waveIndex == maxWaves) {
                gameManager.WinLevel();
                this.enabled = false; //disables 'this' script 
                return;
            }
        }

        if(enemiesAlive <= 0) {
            startButton.GetComponent<Button>().interactable = true;
            FFButton.GetComponent<Button>().interactable = false;
            if(waveStarted) {
                waveStarted = false;
                PlayerStats.money += 100 + (waveIndex * 5);
                currentWave++;
            }
        }

        autoStart = Settings.AutoStart;
        if((GameMode.survival && enemiesAlive == 0)
            || Input.GetKeyDown(KeyCode.Space)
            || (enemiesAlive == 0 && autoStart && firstRoundStarted)) {
            StartWave();
        }
    }

    void SpawnEnemy(GameObject enemy) {
        Enemy e = enemy.GetComponent<Enemy>();
        e.ResetEnemy();
        spawnedEnemies.Add(EnemyPool.instance.Activate(e.enemyType, spawnPoint.position, spawnPoint.rotation));
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
        startButton.GetComponent<Button>().interactable = false;
        FFButton.GetComponent<Button>().interactable = true;
        gameManager.LastControlled();

        int r = PlayerStats.rounds;
        if(r + 1 >= 10) {
            Enemy.difficultyMultiplier += 0.02f;

        }
        if(((r + 1) >= 10) && ((r - 4) % 10 == 0)) { //over round 10 in intervals of 10
            foreach(Enemy e in FindObjectsOfType<Enemy>(true)) {
                e.DecreaseMoneyValue();
            }
        }

        if(!GameMode.survival) {
            SpawnWave();
        } else {
            //StartCoroutine(SurvivalWaves());
            print("TODO: survival");
        }
    }

    void SpawnWave() {
        Wave wave = waves[waveIndex];
        waveIndex++;
        waveStarted = true;
        enemiesAlive = wave.GetTotalEnemies();
        PlayerStats.rounds++;

        foreach(var c in wave.chunks) {
            StartCoroutine(SpawnChunk(c));
        }
    }

    IEnumerator SpawnChunk(WaveChunk c) {
        yield return new WaitForSeconds(c.startDelay);
        for(int i = 0; i < c.count; i++) {
            SpawnEnemy(ChooseEnemyPrefab(c.type));
            if(c.spawnRate > 0) {
                yield return new WaitForSeconds(1 / c.spawnRate);
            } else
                yield return new WaitForEndOfFrame();
        }
    }

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