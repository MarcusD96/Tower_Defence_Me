
using UnityEngine;

[System.Serializable]
public class Upgrade {

    public static float costDifficultyMultipler = 1.0f;

    public string upgradeName = "Range++", description = "";
    public int upgradeCost;
    public float upgradeFactorX, upgradeFactorY;
    private int upgradeLevel = 0;

    public int GetUpgradeCost() {
        return Mathf.RoundToInt((upgradeCost * costDifficultyMultipler) / 5) * 5;
    }

    public void IncreaseUpgrade() {
        upgradeLevel++;
        upgradeCost = Mathf.RoundToInt((GetUpgradeCost() * 1.5f) / 5) * 5;
    }

    public int GetLevel() {
        return upgradeLevel;
    }
}
