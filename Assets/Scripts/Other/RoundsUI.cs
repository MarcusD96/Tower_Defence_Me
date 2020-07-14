using TMPro;
using UnityEngine;

public class RoundsUI : MonoBehaviour {
    public TextMeshProUGUI rounds;

    void Update() {
        if(!GameMode.survival) {
            rounds.text = (WaveSpawner.currentWave + 1) + "/" + WaveSpawner.maxWaves;
            return;
        }
        rounds.text = (WaveSpawner.currentWave + 1).ToString();
    }
}
