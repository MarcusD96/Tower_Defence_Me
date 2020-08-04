using UnityEngine;

public class Shop : MonoBehaviour {
    public TurretFactory standardTurret, missileLauncher, laserBeamer, teslaTurret;

    private BuildManager buildManager;

    void Start() {
        buildManager = BuildManager.instance;
        standardTurret.costText.text = "$" + standardTurret.cost;
        missileLauncher.costText.text = "$" + missileLauncher.cost;
        laserBeamer.costText.text = "$" + laserBeamer.cost;
        teslaTurret.costText.text = "$" + teslaTurret.cost;
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
}
