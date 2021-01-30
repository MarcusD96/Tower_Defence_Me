using UnityEngine;

public class Shop : MonoBehaviour {
    public TurretFactory standardTurret, missileLauncher, laserBeamer, teslaTurret, railgunTurret;

    private BuildManager buildManager;

    void Start() {
        buildManager = BuildManager.instance;
        standardTurret.costText.text = "$" + standardTurret.GetCost();
        missileLauncher.costText.text = "$" + missileLauncher.GetCost();
        laserBeamer.costText.text = "$" + laserBeamer.GetCost();
        teslaTurret.costText.text = "$" + teslaTurret.GetCost();
        railgunTurret.costText.text = "$" + railgunTurret.GetCost();
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
