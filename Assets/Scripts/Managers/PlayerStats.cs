using UnityEngine;

public class PlayerStats {

    public static int startMoney = 650, lives = 100, maxLives = 9999, rounds = 0, maxRounds = 20;
    public static float money;
    public static string levelToLoad;
    public static Difficulty difficulty;

    public static void ResetToDifficulty() {
        switch(difficulty) {
            case Difficulty.Easy:     //easy
                GameMode.survival = false;
                Enemy.difficultyMultiplier = Upgrade.costDifficultyMultiplier = TurretFactory.costDifficultyMultiplier = 0.9f;
                lives = 100;
                maxRounds = 20;
                break;
            case Difficulty.Medium:     //medium
                GameMode.survival = false;
                Enemy.difficultyMultiplier = Upgrade.costDifficultyMultiplier = TurretFactory.costDifficultyMultiplier = 1.0f;
                lives = 50;
                maxRounds = 30;
                break;
            case Difficulty.Hard:     //hard
                GameMode.survival = false;
                Enemy.difficultyMultiplier = Upgrade.costDifficultyMultiplier = TurretFactory.costDifficultyMultiplier = 1.1f;
                lives = 10;
                maxRounds = 40;
                break;
            case Difficulty.Survival:     //survival
                GameMode.survival = true;
                Enemy.difficultyMultiplier = Upgrade.costDifficultyMultiplier = TurretFactory.costDifficultyMultiplier = 1.0f;
                lives = 100;
                maxRounds = int.MaxValue;
                break;
            default:
                Debug.LogError("lol how tf you do dis?");
                break;
        }

        money = startMoney;
        rounds = 0;
    }
}

public enum Difficulty {
    Easy = 0,
    Medium,
    Hard,
    Survival
}
