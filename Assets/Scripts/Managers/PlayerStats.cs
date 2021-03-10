using UnityEngine;

public class PlayerStats {

    public static int money, startMoney = 650, lives = 100, maxLives = 9999, rounds = 0, difficulty;
    public static string nextLevel;

    public static void ResetToDifficulty() {
        switch(difficulty) {
            case 0:     //easy
                GameMode.survival = false;
                Enemy.speedDifficultyMultiplier = Enemy.hpDifficultyMultiplier = Upgrade.costDifficultyMultiplier = TurretFactory.costDifficultyMultiplier = 0.8f;
                lives = 100;
                break;
            case 1:     //medium
                GameMode.survival = false;
                Enemy.speedDifficultyMultiplier = Enemy.hpDifficultyMultiplier = Upgrade.costDifficultyMultiplier = TurretFactory.costDifficultyMultiplier = 1.0f;
                lives = 50;
                break;
            case 2:     //hard
                GameMode.survival = false;
                Enemy.speedDifficultyMultiplier = Enemy.hpDifficultyMultiplier = Upgrade.costDifficultyMultiplier = TurretFactory.costDifficultyMultiplier = 1.25f;
                lives = 10;
                break;
            case 3:     //survival
                GameMode.survival = true;
                Enemy.speedDifficultyMultiplier = Enemy.hpDifficultyMultiplier = Upgrade.costDifficultyMultiplier = TurretFactory.costDifficultyMultiplier = 1.0f;
                lives = 100;
                break;
            default:
                Debug.LogError("lol how tf you do dis?");
                break;
        }

        money = startMoney;
        rounds = 0;
    }
}
