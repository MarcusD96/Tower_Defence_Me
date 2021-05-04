
using UnityEngine;

[System.Serializable]
public class Upgrade {

    public static float costDifficultyMultiplier = 1.0f;

    public string upgradeName = "", description = "";
    public int upgradeCost;
    public float upgradeFactorX, upgradeFactorY;
    private int upgradeLevel = 0;


    public int GetUpgradeCost() {
        return Mathf.RoundToInt((costDifficultyMultiplier * upgradeCost) / 5) * 5;
    }

    public void IncreaseUpgrade(bool multiply) {
        if(multiply)
            upgradeCost = Mathf.RoundToInt(upgradeCost * 1.5f / 5) * 5;
        upgradeLevel++;
    }

    public int GetLevel() {
        return upgradeLevel;
    }
}
