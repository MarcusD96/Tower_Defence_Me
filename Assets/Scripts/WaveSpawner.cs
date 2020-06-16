using System.Collections;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour {

    public Transform enemyPrefab;
    public Transform spawnPoint;

    private float countDown = 2.0f, timeBetweenWaves = 5.0f;

    public TextMeshProUGUI countdownText;

    private int waveIndex = 0;

    void Update() {
        if(countDown <= 0.0f) {
            StartCoroutine(SpawnWave(0.5f));
            countDown = timeBetweenWaves;
            return;
        }

        countDown -= Time.deltaTime;
        countDown = Mathf.Clamp(countDown, 0, float.MaxValue);

        countdownText.text = string.Format("{0:00.00}", countDown);
    }

    IEnumerator SpawnWave(float spawnTime) {
        for(int i = 0; i < waveIndex; i++) {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnTime);
        }
        waveIndex++;
        PlayerStats.rounds++;
    }

    void SpawnEnemy() {
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
