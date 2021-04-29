using UnityEngine;

public class GameManager : MonoBehaviour {

    public static bool gameEnd;

    private GameObject gameOverUI, winLevelUI;

    public float fastForward;

    public static Node lastControlled;

    void Awake() {
        winLevelUI = FindObjectOfType<LevelWin>(true).gameObject;
        winLevelUI.SetActive(false);
        gameOverUI = FindObjectOfType<GameOver>(true).gameObject;
        gameOverUI.SetActive(false);
        if(FindObjectOfType<FPSCounter>() == null)
            Instantiate(new GameObject("FPS", typeof(FPSCounter)));
        Application.targetFrameRate = 200;
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
    void LateUpdate() {
        if(gameEnd)
            return;

        if(PlayerStats.lives <= 0) {
            EndGame();
            return;
        }

        if(!PauseMenu.paused) {
            if(WaveSpawner.enemiesAlive <= 0) {
                //fast forward
                if(Input.GetKeyDown(KeyCode.LeftShift)) {
                    if(Time.timeScale == fastForward) {
                        Time.timeScale = 1;
                    } else {
                        Time.timeScale = fastForward;
                    }
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
        }
    }

    void EndGame() {
        gameEnd = true;
        gameOverUI.SetActive(true);
        AudioManager.StaticStopAllSounds();
    }

    public void WinLevel() {
        gameEnd = true;
        winLevelUI.SetActive(true);
        AudioManager.StaticStopAllSounds();
    }

    public void LastControlled() {
        if(!Settings.ReturnTurret) {
            if(lastControlled) {
                lastControlled.RevertTurret(false); 
            }
            return;
        }
            
        if(lastControlled) {
            lastControlled.ControlTurret();
        }
    }
}
