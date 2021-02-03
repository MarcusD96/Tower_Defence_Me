using UnityEngine;

public class GameManager : MonoBehaviour {

    public static bool gameEnd;

    public GameObject gameOverUI, winLevelUI;

    public static Node lastControlled;

    void Start() {
        gameEnd = false;
        lastControlled = null;
        PlayerStats.ResetToDifficulty();
    }

    // Update is called once per frame
    void Update() {
        if(gameEnd)
            return;

        if(PlayerStats.lives <= 0)
            EndGame();

        ///////
        if(Input.GetKeyDown(KeyCode.L)) {
            WinLevel();
        }

        ///////
        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            if(Time.timeScale == 20f) {
                Time.timeScale = 1;
            } else {
                Time.timeScale = 20f;
            }
        }

        //cheaty :P
        if(Input.GetKey(KeyCode.P)) {
            if(Input.GetKey(KeyCode.M)) {
                if(Input.GetKeyDown(KeyCode.B)) {
                    Cheaty.play = true;
                }
            }
        }
    }

    void EndGame() {
        gameEnd = true;
        gameOverUI.SetActive(true);
    }

    public void WinLevel() {
        gameEnd = true;
        winLevelUI.SetActive(true);
    }

    public void LastControlled() {
        if(lastControlled) {
            lastControlled.ControlTurret();
        }
    }
}
