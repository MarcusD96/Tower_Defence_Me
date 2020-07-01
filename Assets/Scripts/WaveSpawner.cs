using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {
    public static int enemiesAlive = 0, maxWaves, currentWave;

    public Wave[] waves;
    public Transform spawnPoint;
    public GameManager gameManager;
    public Button startButton;
    public GameObject remainingText;

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

        if(waveIndex == waves.Length) {
            gameManager.WinLevel();
            this.enabled = false; //disables 'this' script
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

        if(Input.GetKeyDown(KeyCode.Space)) {
            StartWave();
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

    void SpawnEnemy(GameObject enemy) {
        Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
    }

    public void StartWave() {
        startButton.gameObject.SetActive(false);
        remainingText.SetActive(true);
        StartCoroutine(SpawnWave());
    }
}
