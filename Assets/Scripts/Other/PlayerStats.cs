using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public static int money, lives = 1, maxLives, rounds, difficulty;

    public int startMoney, numLives, numMaxLives;

    void Start() {
        money = startMoney;
        maxLives = numMaxLives;
        rounds = 0;
    }

    public static void ResetToDifficulty() {
        switch(difficulty) {
            case 0:     //easy
                Enemy.speedDifficultyMultiplier = Enemy.hpDifficultyMultiplier = Upgrade.costDifficultyMultiplier = 0.9f;
                lives = 100;
                break;
            case 1:     //medium
                Enemy.speedDifficultyMultiplier = Enemy.hpDifficultyMultiplier = Upgrade.costDifficultyMultiplier = 1.0f;
                lives = 50;
                break;
            case 2:     //hard
                Enemy.speedDifficultyMultiplier = Enemy.hpDifficultyMultiplier = Upgrade.costDifficultyMultiplier = 1.1f;
                lives = 1;
                break;
            case 3:     //survival
                GameMode.survival = true;
                Enemy.speedDifficultyMultiplier = Enemy.hpDifficultyMultiplier = Upgrade.costDifficultyMultiplier = 1.0f;
                lives = 100;
                break;
            default:
                break;
        }
    }
}
