using UnityEngine;

public class GameManager : MonoBehaviour {

    public static bool gameEnd;

    private GameObject gameOverUI, winLevelUI;

    public float fastForward;

    public static Node lastControlled;

    void Awake() {
        winLevelUI = Resources.FindObjectsOfTypeAll<LevelWin>()[0].gameObject;
        gameOverUI = Resources.FindObjectsOfTypeAll<GameOver>()[0].gameObject;
    }

    void Start() {
        gameEnd = false;
        lastControlled = null;
        PlayerStats.ResetToDifficulty();
        if(Time.timeScale != 1) {
            Time.timeScale = 1;
        }
    }

    // Update is called once per frame
    void Update() {
        if(gameEnd)
            return;

        if(PlayerStats.lives <= 0)
            EndGame();

        if(!PauseMenu.paused) {

            //fast forward
            if(Input.GetKeyDown(KeyCode.LeftShift)) {
                if(Time.timeScale == fastForward) {
                    Time.timeScale = 1;
                } else {
                    Time.timeScale = fastForward;
                }
            }

            //cheaty :P
            if(Input.GetKey(KeyCode.P)) {
                if(Input.GetKey(KeyCode.M)) {
                    if(Input.GetKeyDown(KeyCode.B)) {
                        Cheaty.play = true;
                        PlayerStats.money = 99995;
                        PlayerStats.lives = 9995;
                    }
                }
            }

            if(Input.GetKeyDown(KeyCode.F)) {
                WinLevel();
            }
        }
    }

    void EndGame() {
        gameEnd = true;
        gameOverUI.SetActive(true);
        AudioManager.StopAllSounds();
    }

    public void WinLevel() {
        gameEnd = true;
        winLevelUI.SetActive(true);
        AudioManager.StopAllSounds();
    }

    public void LastControlled() {
        if(lastControlled) {
            lastControlled.ControlTurret();
        }
    }
}
