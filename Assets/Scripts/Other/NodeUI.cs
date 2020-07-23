using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour {
    public GameObject ui, upgradeUI;
    public TextMeshProUGUI sellPrice;

    public Button upgradeButtonA, upgradeButtonB, upgradeButtonSpecial;
    public TextMeshProUGUI upgradeTextA, upgradeTextB, UpgradeTextSpecial;
    public TextMeshProUGUI upgradeCostA, upgradeCostB, UpgradeCostSpecial;
    public TextMeshProUGUI targetting;

    private Node target;

    public void SetTarget(Node target_) {
        ui.SetActive(true);
        upgradeUI.SetActive(false);

        target = target_;
        transform.position = target.GetBuildPosition();

        upgradeButtonA.interactable = true;
        upgradeButtonB.interactable = true;
        upgradeButtonSpecial.interactable = true;

        SetNamesAndPrices();

        CheckMax();
    }

    public void Hide() {
        upgradeUI.SetActive(false);
        ui.SetActive(false);
    }

    public void Upgrade() {
        upgradeUI.SetActive(!upgradeUI.activeSelf);
    }

    public void UpgradeA() {
        target.UpgradeA();
        SetNamesAndPrices();
        CheckMax();
    }
    
    public void UpgradeB() {
        target.UpgradeB();
        SetNamesAndPrices();
        CheckMax();
    }
    
    public void UpgradeSpecial() {
        target.UpgradeSpecial();
        SetNamesAndPrices();
        CheckMax();
    }

    public void Sell() {
        target.SellTurret();
        upgradeUI.SetActive(false);
        BuildManager.instance.DeselectNode();
    }

    public void Control() {
        target.ControlTurret();
        upgradeUI.SetActive(false);
        BuildManager.instance.DeselectNode();
    }

    public void NextTargetting() {
        target.turret.GetComponent<Turret>().NextTargettingOption();
        targetting.text = target.turret.GetComponent<Turret>().targetting;
    }

    public void LastTargetting() {
        target.turret.GetComponent<Turret>().LastTargettingOption();
        targetting.text = target.turret.GetComponent<Turret>().targetting;
    }

    void SetNamesAndPrices() {
        var t = target.turret.GetComponent<Turret>();

        sellPrice.text = "$" + t.GetSellPrice();

        upgradeTextA.text = t.ugA.upgradeName;
        upgradeCostA.text = t.ugA.upgradeCost.ToString();

        upgradeTextB.text = t.ugB.upgradeName;
        upgradeCostB.text = t.ugB.upgradeCost.ToString();

        UpgradeTextSpecial.text = t.ugSpec.upgradeName;
        UpgradeCostSpecial.text = t.ugSpec.upgradeCost.ToString();
    }

    void CheckMax() {

        var t = target.turret.GetComponent<Turret>();

        if(t.ugA.GetLevel() > 2) {
            upgradeTextA.text = "MAX";
            upgradeCostA.text = "";
            upgradeButtonA.interactable = false;
        }
        if(t.ugB.GetLevel() > 2) {
            upgradeTextB.text = "MAX";
            upgradeCostB.text = "";
            upgradeButtonB.interactable = false;
        }
        if(t.ugSpec.GetLevel() > 0) {
            UpgradeTextSpecial.text = "MAX";
            UpgradeCostSpecial.text = "";
            upgradeButtonSpecial.interactable = false;
        }
    }
}
