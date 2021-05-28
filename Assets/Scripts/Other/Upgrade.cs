
using UnityEngine;

[System.Serializable]
public class Upgrade {

    public static float costDifficultyMultiplier = 1.0f;
    public string upgradeName, description;    
    public int baseCost;
    public float upgradeFactorX, upgradeFactorY;

    private int currentCost;
    private int upgradeLevel = 0;

    public int GetUpgradeCost() {
        if(upgradeLevel == 0)
            currentCost = Mathf.RoundToInt(baseCost * costDifficultyMultiplier / 5) * 5;        
        return currentCost;
    }

    public void SetUpgradeCost(int cost) {
        currentCost = cost;
    }

    public void IncreaseUpgrade(bool multiply) {
        if(multiply)
            currentCost = Mathf.RoundToInt(currentCost * 1.5f / 5) * 5;
        upgradeLevel++;
    }

    public int GetLevel() {
        return upgradeLevel;
    }
}
