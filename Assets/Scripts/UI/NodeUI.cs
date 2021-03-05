using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour {
    [Header("UI Elements")]
    public GameObject ui;
    public GameObject upgradeUI, tooltips, info;
    public Button controlButton;
    public TextMeshProUGUI sellPrice;

    [Header("Upgrades")]
    public Button upgradeButtonA;
    public Button upgradeButtonB, upgradeButtonSpecial;
    public TextMeshProUGUI upgradeTextA, upgradeTextB, upgradeTextSpecial;
    public TextMeshProUGUI upgradeCostA, upgradeCostB, upgradeCostSpecial;
    public TextMeshProUGUI upgradeLevelA, upgradeLevelB;
    public TextMeshProUGUI toolTipA, toolTipB, toolTipSpec;

    [Header("Targetting")]
    public TextMeshProUGUI targetting;

    private Node target;

    void Update() {
        if(!target)
            return;
        if(PauseMenu.paused == true)
            return;

        NodeUIKeyboardShortcuts();
    }

    private void FixedUpdate() {
        CheckInteractable();
    }

    void NodeUIKeyboardShortcuts() {
        if(Input.GetKeyDown(KeyCode.X)) { //open upgrade menu
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
        if(WaveSpawner.enemiesAlive > 0) {
            if(Input.GetKeyDown(KeyCode.Tab)) { //control
                Control();
            }
        }
    }

    void CheckInteractable() {
        if(target == null) {
            return;
        }

        if(target.turret == null)
            return;

        var t = target.turret.GetComponent<Turret>();

        var cash = PlayerStats.money;

        //control button
        if(WaveSpawner.enemiesAlive <= 0)
            controlButton.interactable = false;
        else
            controlButton.interactable = true;

        //upg A button
        if(cash < t.ugA.GetUpgradeCost() || t.ugA.GetLevel() > 4)
            upgradeButtonA.interactable = false;
        else
            upgradeButtonA.interactable = true;

        //upg B button
        if(cash < t.ugB.GetUpgradeCost() || t.ugB.GetLevel() > 2)
            upgradeButtonB.interactable = false;
        else
            upgradeButtonB.interactable = true;

        //special button
        if(t.ugA.GetLevel() >= 3 && t.ugB.GetLevel() >= 3 && cash >= t.ugSpec.GetUpgradeCost()) {
            upgradeButtonSpecial.interactable = true;
        } else
            upgradeButtonSpecial.interactable = false;
        if(t.ugSpec.GetLevel() > 0)
            upgradeButtonSpecial.interactable = false;
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

        SetUIInfo();
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
        SetUIInfo();
        CheckMax();
    }

    public void UpgradeB() {
        target.UpgradeB();
        SetUIInfo();
        CheckMax();
    }

    public void UpgradeSpecial() {
        target.UpgradeSpecial();
        SetUIInfo();
        CheckMax();
    }

    public void Sell() {
        target.RevertTurret(false);
        target.SellTurret();
        upgradeUI.SetActive(false);
        BuildManager.instance.DeselectNode();
    }

    public void Control() {
        target.ControlTurret();
        upgradeUI.SetActive(false);
        BuildManager.instance.DeselectNode();
    }

    public void ChangeTargetting(bool next) {
        targetting.text = target.turret.GetComponent<Turret>().ChangeTargetting(next);
    }

    public void ToggleTooltip() {
        tooltips.SetActive(!tooltips.activeSelf);
    }

    void SetUIInfo() {
        var t = target.turret.GetComponent<Turret>();

        targetting.text = target.turret.GetComponent<Turret>().GetTargetting();

        sellPrice.text = "$" + t.GetSellPrice();

        upgradeTextA.text = t.ugA.upgradeName;
        upgradeCostA.text = t.ugA.GetUpgradeCost().ToString();
        upgradeLevelA.text = t.ugA.GetLevel().ToString() + "/5";
        toolTipA.text = t.ugA.description;

        upgradeTextB.text = t.ugB.upgradeName;
        upgradeCostB.text = t.ugB.GetUpgradeCost().ToString();
        upgradeLevelB.text = t.ugB.GetLevel().ToString() + "/3";
        toolTipB.text = t.ugB.description;

        upgradeTextSpecial.text = t.ugSpec.upgradeName;
        upgradeCostSpecial.text = t.ugSpec.GetUpgradeCost().ToString();
        toolTipSpec.text = t.ugSpec.description;
    }

    void CheckMax() {

        var t = target.turret.GetComponent<Turret>();

        if(t.ugA.GetLevel() > 4) {
            upgradeTextA.text = "MAX";
            upgradeCostA.text = "";
            upgradeButtonA.interactable = false;
            toolTipA.text = "max";
        }
        if(t.ugB.GetLevel() > 2) {
            upgradeTextB.text = "MAX";
            upgradeCostB.text = "";
            upgradeButtonB.interactable = false;
            toolTipB.text = "max";
        }
        if(t.ugSpec.GetLevel() > 0) {
            upgradeTextSpecial.text = "MAX";
            upgradeCostSpecial.text = "";
            upgradeButtonSpecial.interactable = false;
            toolTipSpec.text = "max";
        }
    }
}
