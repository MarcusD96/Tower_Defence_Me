using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {
    public TurretFactory standardTurret, missileLauncher, laserBeamer, teslaTurret, railgunTurret;
    public Button standardTurretBtn, missileLauncherBtn, laserBeamerBtn, teslaTurretBtn, railgunTurretBtn, shopBtn;
    public Animator anim;

    private BuildManager buildManager;

    void Start() {
        buildManager = BuildManager.instance;
        standardTurret.costText.text = "$" + standardTurret.GetCost();
        missileLauncher.costText.text = "$" + missileLauncher.GetCost();
        laserBeamer.costText.text = "$" + laserBeamer.GetCost();
        teslaTurret.costText.text = "$" + teslaTurret.GetCost();
        railgunTurret.costText.text = "$" + railgunTurret.GetCost();
    }

    void LateUpdate() {
        CheckAffordability();
    }

    void CheckAffordability() {
        var cash = PlayerStats.money;

        //standard turret
        if(cash < standardTurret.GetCost())
            standardTurretBtn.interactable = false;
        else
            standardTurretBtn.interactable = true;

        //missile turret
        if(cash < missileLauncher.GetCost())
            missileLauncherBtn.interactable = false;
        else
            missileLauncherBtn.interactable = true;

        //laser turret
        if(cash < laserBeamer.GetCost())
            laserBeamerBtn.interactable = false;
        else
            laserBeamerBtn.interactable = true;

        //tesla turret
        if(cash < teslaTurret.GetCost())
            teslaTurretBtn.interactable = false;
        else
            teslaTurretBtn.interactable = true;

        //railgun turret
        if(cash < railgunTurret.GetCost())
            railgunTurretBtn.interactable = false;
        else
            railgunTurretBtn.interactable = true;
    }

    public void OpenShop() {
        //bool is false so shop is closed
        if(anim.GetBool("Open") == false) {
            anim.SetBool("Open", true);
            shopBtn.gameObject.SetActive(false);
        }
    }

    public void CloseShop() {
        //bool is true so shop is open
        if(anim.GetBool("Open") == true) {
            anim.SetBool("Open", false);
            shopBtn.gameObject.SetActive(true);
        }
    }

    public void SelectStandardTurret() {
        buildManager.SelectTurretToBuild(standardTurret);
    }

    public void SelectMissileLauncher() {
        buildManager.SelectTurretToBuild(missileLauncher);
    }

    public void SelectLaserBeamer() {
        buildManager.SelectTurretToBuild(laserBeamer);
    }

    public void SelectTeslaTurret() {
        buildManager.SelectTurretToBuild(teslaTurret);
    }

    public void SelectRailgunTurret() {
        buildManager.SelectTurretToBuild(railgunTurret);
    }
}
