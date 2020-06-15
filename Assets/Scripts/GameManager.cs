using UnityEngine;

public class GameManager : MonoBehaviour {

    private bool gameEnd = false;

    // Update is called once per frame
    void Update() {
        if(gameEnd)
            return;

        if (PlayerStats.lives <= 0)
            EndGame();
    }

    void EndGame() {
        gameEnd = true;
        Debug.Log("Game Over");
    }
}
