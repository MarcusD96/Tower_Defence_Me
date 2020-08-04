using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class TurretFactory {
    public GameObject turretPrefab; // upgradedPrefab;
    public TextMeshProUGUI costText;
    public Image border;
    public int cost;

    public int GetCost() {
        return cost;
    }

    public Turret GetTurret() {
        return turretPrefab.GetComponent<Turret>();
    }
}
