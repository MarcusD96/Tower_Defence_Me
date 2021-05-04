﻿using TMPro;
using UnityEngine;

public class RemainingUI : MonoBehaviour {
    public TextMeshProUGUI remainingText;

    void Update() {
        remainingText.text = WaveSpawner.enemiesAlive.ToString();
    }
}