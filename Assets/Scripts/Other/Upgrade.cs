
using UnityEngine;

[System.Serializable]
public class Upgrade {

    public static float costDifficultyMultiplier = 1.0f;

    public string upgradeName = "Range++", description = "";
    public int upgradeCost;
    public float upgradeFactorX, upgradeFactorY;
    private int upgradeLevel = 0;


    public int GetUpgradeCost() {
        return Mathf.RoundToInt((costDifficultyMultiplier * upgradeCost) / 5) * 5;
    }

    public void IncreaseUpgrade() {
        upgradeLevel++;
    }

    public int GetLevel() {
        return upgradeLevel;
    }
}
