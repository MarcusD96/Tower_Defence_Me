using UnityEngine;

[System.Serializable]
public class TurretFactory {
    public GameObject turretPrefab, upgradedPrefab;
    public int cost, upgradeCost;

    public int GetSellPrice(bool upgraded) {
        if(!upgraded)
            return cost / 2;
        else
            return (cost + upgradeCost) / 2;
    }
}
