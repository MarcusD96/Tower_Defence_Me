﻿using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class TurretFactory {
    public GameObject turretPrefab, upgradedPrefab;
    public TextMeshProUGUI costText;
    public Image border;
    public int cost, upgradeCost;

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
