using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour {
    [Header("UI Elements")]
    public GameObject ui;
    public GameObject upgradeUI, tooltips, info;
    public TextMeshProUGUI sellPrice;

    [Header("Upgrades")]
    public Button upgradeButtonA;
    public Button upgradeButtonB, upgradeButtonSpecial;
    public TextMeshProUGUI upgradeTextA, upgradeTextB, UpgradeTextSpecial;
    public TextMeshProUGUI upgradeCostA, upgradeCostB, UpgradeCostSpecial;
    public TextMeshProUGUI ttA, ttB, ttSpec;

    [Header("Targetting")]
    public TextMeshProUGUI targetting;


    private Node target;

    void Update() {
        if(!target)
            return;

        if(Input.GetKeyDown(KeyCode.X)) {
            Upgrade();
        }

        if(upgradeButtonA.interactable) {
            if(Input.GetKeyDown(KeyCode.Comma)) { //upgrade left
                UpgradeA();
            }
        }
        if(upgradeButtonB.interactable) {
            if(Input.GetKeyDown(KeyCode.Period)) { //upgrade right
                UpgradeB();
            }
        }
        if(upgradeButtonSpecial.interactable) {
            if(Input.GetKeyDown(KeyCode.Slash)) { //upgrade special
                UpgradeSpecial();
            }
        }
        if(Input.GetKeyDown(KeyCode.Backspace)) { //sell
            Sell();
        }
        if(Input.GetKeyDown(KeyCode.Tab)) { //control 
            Control();
        }
    }

    public void SetTarget(Node target_) {
        ui.SetActive(true);
        upgradeUI.SetActive(false);
        info.SetActive(false);
        tooltips.SetActive(false);

        target = target_;
        transform.position = target.GetBuildPosition();

        upgradeButtonA.interactable = true;
        upgradeButtonB.interactable = true;
        upgradeButtonSpecial.interactable = true;

        SetNamesPricesToolTip();
        CheckMax();
    }

    public void Hide() {
        upgradeUI.SetActive(false);
        ui.SetActive(false);
        info.SetActive(false);
        tooltips.SetActive(false);
    }

    public void Upgrade() {
        upgradeUI.SetActive(!upgradeUI.activeSelf);
        info.SetActive(!info.activeSelf);
    }

    public void UpgradeA() {
        target.UpgradeA();
        SetNamesPricesToolTip();
        CheckMax();
    }

    public void UpgradeB() {
        target.UpgradeB();
        SetNamesPricesToolTip();
        CheckMax();
    }

    public void UpgradeSpecial() {
        target.UpgradeSpecial();
        SetNamesPricesToolTip();
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

    public void ToggleTooltip() {
        tooltips.SetActive(!tooltips.activeSelf);
    }

    void SetNamesPricesToolTip() {
        var t = target.turret.GetComponent<Turret>();

        sellPrice.text = "$" + t.GetSellPrice();

        upgradeTextA.text = t.ugA.upgradeName;
        upgradeCostA.text = t.ugA.GetUpgradeCost().ToString();
        ttA.text = t.ugA.description;

        upgradeTextB.text = t.ugB.upgradeName;
        upgradeCostB.text = t.ugB.GetUpgradeCost().ToString();
        ttB.text = t.ugB.description;

        UpgradeTextSpecial.text = t.ugSpec.upgradeName;
        UpgradeCostSpecial.text = t.ugSpec.GetUpgradeCost().ToString();
        ttSpec.text = t.ugSpec.description;
    }

    void CheckMax() {

        var t = target.turret.GetComponent<Turret>();

        if(t.ugA.GetLevel() > 2) {
            upgradeTextA.text = "MAX";
            upgradeCostA.text = "";
            upgradeButtonA.interactable = false;
            ttA.text = "max";
        }
        if(t.ugB.GetLevel() > 2) {
            upgradeTextB.text = "MAX";
            upgradeCostB.text = "";
            upgradeButtonB.interactable = false;
            ttB.text = "max";
        }
        if(t.ugSpec.GetLevel() > 0) {
            UpgradeTextSpecial.text = "MAX";
            UpgradeCostSpecial.text = "";
            upgradeButtonSpecial.interactable = false;
            ttSpec.text = "max";
        }
    }
}
