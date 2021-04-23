
using UnityEngine;

[System.Serializable]
public class Wave {

    public WaveChunk[] chunks;

    public int GetTotalEnemies() {
        int total = 0;

        foreach(var e in chunks) {
            total += e.count;
        }

        return total;
    }
}

[System.Serializable]
public class WaveChunk {
    public GameObject enemyPrefab;
    public int count;
    public float spawnRate;
    public float startDelay;
}