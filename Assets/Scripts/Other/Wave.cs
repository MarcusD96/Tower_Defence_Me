
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

    public float GetWaveTime() {
        float highestTime = 0;
        foreach(var c in chunks) {
            float r = c.spawnRate;
            if(r <= 0) //DIVIDE BY 0 IS A NO NO :)
                r = Time.deltaTime;

            float t = ((1 / r) * c.count) + c.startDelay;

            if(t > highestTime)
                highestTime = t;
        }
        return highestTime;
    }
}

[System.Serializable]
public class WaveChunk {
    public EnemyType e;
    public int count;
    public float spawnRate;
    public float startDelay;
}