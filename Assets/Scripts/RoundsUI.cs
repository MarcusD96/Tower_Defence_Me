using TMPro;
using UnityEngine;

public class RoundsUI : MonoBehaviour {
    public TextMeshProUGUI rounds;

    void Update() {
        rounds.text = (WaveSpawner.currentWave + 1) + "/" + WaveSpawner.maxWaves;
    }
}
