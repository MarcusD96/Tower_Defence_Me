
using UnityEngine;

[System.Serializable]
public class Upgrade {

    public string upgradeName = "Range++";
    public int upgradeCost;
    public float upgradeFactorX, upgradeFactorY;
    private int upgradeLevel = 0;

    public void IncreaseUpgrade() {
        upgradeLevel++;
        upgradeCost = Mathf.RoundToInt((upgradeCost * 1.5f) / 5) * 5;
    }

    public int GetLevel() {
        return upgradeLevel;
    }
}
