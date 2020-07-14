using UnityEngine;

public class Shop : MonoBehaviour {
    public TurretFactory standardTurret, missileLauncher, laserBeamer;

    private BuildManager buildManager;

    void Start() {
        buildManager = BuildManager.instance;
    }

    public void Update() {
        standardTurret.costText.text = "$" + standardTurret.cost;
        missileLauncher.costText.text = "$" + missileLauncher.cost;
        laserBeamer.costText.text = "$" + laserBeamer.cost;
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

}
