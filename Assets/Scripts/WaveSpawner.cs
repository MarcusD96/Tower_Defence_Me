using System.Collections;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour {

    public Transform enemyPrefab;
    public Transform spawnPoint;

    private float countDown = 0.0f, timeBetweenWaves = 3.5f;

    public TextMeshProUGUI countdownText;

    private int waveIndex = 0;

    void Update() {
        if(countDown <= 0.0f) {
            StartCoroutine(SpawnWave(0.5f));
            countDown = timeBetweenWaves;
            return;
        }

        countDown -= Time.deltaTime;

        countdownText.text = Mathf.Round(countDown).ToString();
    }

    IEnumerator SpawnWave(float spawnTime) {
        for(int i = 0; i < waveIndex; i++) {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnTime);
        }
        waveIndex++;
    }

    void SpawnEnemy() {
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
