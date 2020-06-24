using System.Collections;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour {
    public static int enemiesAlive = 0;

    public Wave[] waves;
    public Transform spawnPoint;
    public float timeBetweenWaves = 5.0f;
    public GameManager gameManager;
    public TextMeshProUGUI countdownText;

    private float countDown = 0.0f;
    private int waveIndex = 0;

    void Start() {
        enemiesAlive = 0;
    }

    void Update() {
        if(enemiesAlive > 0)
            return;
        
        if (waveIndex == waves.Length) {
            gameManager.WinLevel();
            this.enabled = false; //disables 'this' script
        }

        if(countDown <= 0.0f) {
            StartCoroutine(SpawnWave());
            countDown = timeBetweenWaves;
            return;
        }

        countDown -= Time.deltaTime;
        countDown = Mathf.Clamp(countDown, 0, float.MaxValue);

        countdownText.text = string.Format("{0:00.00}", countDown);        
    }

    IEnumerator SpawnWave() {        
        PlayerStats.rounds++;

        Wave wave = waves[waveIndex];

        for(int i = 0; i < wave.count; i++) {
            SpawnEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(1 / wave.spawnRate);
        }

        waveIndex++;
    }

    void SpawnEnemy(GameObject enemy) {
        Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
        enemiesAlive++;
    }
}
