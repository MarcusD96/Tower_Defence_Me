﻿using UnityEngine;

public class GameManager : MonoBehaviour {   

    public static bool gameEnd;
    
    public GameObject gameOverUI, winLevelUI;

    void Start() {
        gameEnd = false;
    }

    // Update is called once per frame
    void Update() {
        if(gameEnd)
            return;

#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.E)) {
            EndGame();
        }
#endif
        if (PlayerStats.lives <= 0)
            EndGame();
    }

    void EndGame() {
        gameEnd = true;
        gameOverUI.SetActive(true);
    }

    public void WinLevel() {
        gameEnd = true;
        winLevelUI.SetActive(true);
    }
}
