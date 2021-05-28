using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TurretFactory {
    public GameObject turretPrefab; // upgradedPrefab;
    public TextMeshProUGUI costText;
    public Image border;
    [SerializeField]
    private int cost;
    public static float costDifficultyMultiplier = 1.0f;

    public int GetCost() {
        return Mathf.RoundToInt(cost * costDifficultyMultiplier / 5) * 5;
    }

    public Turret GetTurret() {
        return turretPrefab.GetComponent<Turret>();
    }
}
