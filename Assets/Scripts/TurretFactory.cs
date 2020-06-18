using UnityEngine;
using TMPro;

[System.Serializable]
public class TurretFactory {
    public GameObject turretPrefab, upgradedPrefab;
    public int cost, upgradeCost;
    public TextMeshProUGUI costText;

    public int GetSellPrice(bool upgraded) {
        if(!upgraded)
            return cost / 2;
        else
            return (cost + upgradeCost) / 2;
    }

    public int GetCost() {
        return cost;
    }
}
