
using UnityEngine;

[System.Serializable]
public class Upgrade {

    public static float costDifficultyMultiplier = 1.0f;

    public string upgradeName = "Range++", description = "";
    public int upgradeCost;
    //private int startCost;
    public float upgradeFactorX, upgradeFactorY;
    private int upgradeLevel = 0;

    public int GetUpgradeCost() {
        //if(upgradeLevel == 0) {
        //    startCost = Mathf.RoundToInt((upgradeCost * costDifficultyMultiplier) / 5) * 5;
        //    return startCost;
        //} else {
        //    return Mathf.RoundToInt(startCost / 5) * 5;
        //}
        return Mathf.RoundToInt((costDifficultyMultiplier * upgradeCost) / 5) * 5;
    }

    public void IncreaseUpgrade() {
        upgradeLevel++;
        //startCost = Mathf.RoundToInt((GetUpgradeCost() * 1.5f) / 5) * 5;
    }

    public int GetLevel() {
        return upgradeLevel;
    }
}
