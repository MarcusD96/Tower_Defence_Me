using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour {
    public GameObject ui;
    public TextMeshProUGUI upgradeCost, sellPrice;
    public Button upgradeButton;

    private Node target;

    public void SetTarget(Node target_) {
        ui.SetActive(true);
        target = target_;
        transform.position = target.GetBuildPosition();

        if(target.isUpgraded == false) {
            upgradeCost.text = "$" + target.currentFactory.upgradeCost;
            upgradeButton.interactable = true;
        } else {
            upgradeCost.text = "MAX";
            upgradeButton.interactable = false;
        }

        sellPrice.text = "$" + target.currentFactory.GetSellPrice(target.isUpgraded);
    }

    public void Hide() {
        ui.SetActive(false);
    }

    public void Upgrade() {
        target.UpgradeTurret();
        BuildManager.instance.DeselectNode();
    }

    public void Sell() {
        target.SellTurret();
        BuildManager.instance.DeselectNode();
    }

    public void Control() {
        target.ControlTurret();
        BuildManager.instance.DeselectNode();
    }
}
