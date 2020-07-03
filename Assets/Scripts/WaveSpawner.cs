using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {
    public static int enemiesAlive = 0, maxWaves, currentWave;

    public Wave[] waves;
    public Transform spawnPoint;
    public GameManager gameManager;
    public Button startButton;
    public GameObject remainingText, normal, heavy, fast, boss;

    private int waveIndex = 0;
    private bool waveStarted;

    void Start() {
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
                if(!GameMode.survival)
                    PlayerStats.money += 100 + (waveIndex * 5);
                currentWave++;
            }
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            StartWave();
        }
    }

    void SpawnEnemy(GameObject enemy) {
        Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
    }

    public void StartWave() {
        startButton.gameObject.SetActive(false);
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

    IEnumerator SurvivalWaves() {
        PlayerStats.rounds++;
        waveStarted = true;
        waveIndex++;

        Wave wave = new Wave();
        if(waveIndex % 15 == 0) {
            wave.count = Mathf.CeilToInt(waveIndex / 15);
            enemiesAlive = wave.count;
            wave.spawnRate = waveIndex / 100.0f;
            Debug.Log(wave.spawnRate);
            for(int i = 0; i < wave.count; i++) {
                SpawnEnemy(boss);
                yield return new WaitForSeconds(1 / wave.spawnRate);
            }
            IncreaseDifficulty();
        }  else if(waveIndex % 7 == 0) {
            wave.count = Mathf.CeilToInt(waveIndex * 2.5f);
            enemiesAlive = wave.count;
            wave.spawnRate = waveIndex / 5.0f;
            Debug.Log(wave.spawnRate);
            for(int i = 0; i < wave.count; i++) {
                SpawnEnemy(fast);
                yield return new WaitForSeconds(1 / wave.spawnRate);
            }
        } else if(waveIndex % 3 == 0) {
            wave.count = Mathf.CeilToInt(waveIndex * 1.5f);
            enemiesAlive = wave.count;
            wave.spawnRate = waveIndex / 3.0f;
            Debug.Log(wave.spawnRate);
            for(int i = 0; i < wave.count; i++) {
                SpawnEnemy(heavy);
                yield return new WaitForSeconds(1 / wave.spawnRate);
            }
        }else {
            wave.count = Mathf.CeilToInt((waveIndex) * 4);
            enemiesAlive = wave.count;
            wave.spawnRate = waveIndex;
            Debug.Log(wave.spawnRate);
            for(int i = 0; i < wave.count; i++) {
                SpawnEnemy(normal);
                yield return new WaitForSeconds(1 / wave.spawnRate);
            }
        }
    }

    void IncreaseDifficulty() {
        normal.GetComponent<Enemy>().startHp *= 2;
        heavy.GetComponent<Enemy>().startHp *= 2;
        fast.GetComponent<Enemy>().startHp *= 2;
        boss.GetComponent<Enemy>().startHp *= 2;
    }
}
