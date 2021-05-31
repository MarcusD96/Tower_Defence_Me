
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurretViewUpgrades : MonoBehaviour {

    public Button buttonA, buttonB, buttonSpec;
    public TextMeshProUGUI nameA, nameB, nameSpec;
    public TextMeshProUGUI costA, costB, costSpec;
    public TextMeshProUGUI levelA, levelB;

    private Turret turret;

    private void Awake() {
        turret = transform.root.GetComponent<Turret>();
    }

    public void Initialize() {
        nameA.text = turret.ugA.upgradeName;
        costA.text = turret.ugA.GetUpgradeCost().ToString();
        nameB.text = turret.ugB.upgradeName;
        costB.text = turret.ugB.GetUpgradeCost().ToString();
        nameSpec.text = turret.ugSpec.upgradeName;
        costSpec.text = turret.ugSpec.GetUpgradeCost().ToString();
        levelA.text = turret.ugA.GetLevel().ToString() + "/5";
        levelB.text = turret.ugB.GetLevel().ToString() + "/3";
    }

    public void LateUpdate() {
        Initialize();
        CheckAffordability();
        CheckMax();
    }

    void CheckAffordability() {
        var m = PlayerStats.money;

        if(m < turret.ugA.GetUpgradeCost())
            buttonA.interactable = false;
        else
            buttonA.interactable = true;

        if(m < turret.ugB.GetUpgradeCost())
            buttonB.interactable = false;
        else
            buttonB.interactable = true;

        if(m < turret.ugSpec.GetUpgradeCost() || (turret.ugA.GetLevel() < 3 && turret.ugB.GetLevel() < 3))
            buttonSpec.interactable = false;
        else
            buttonSpec.interactable = true;
    }

    void CheckMax() {
        if(turret.ugA.GetLevel() > 4) {
            nameA.text = "MAX";
            costA.text = "";
            buttonA.interactable = false;
        }
        if(turret.ugB.GetLevel() > 2) {
            nameB.text = "MAX";
            costB.text = "";
            buttonB.interactable = false;
        }
        if(turret.ugSpec.GetLevel() > 0) {
            nameSpec.text = "MAX";
            costSpec.text = "";
            buttonSpec.interactable = false;
        }
    }
}
