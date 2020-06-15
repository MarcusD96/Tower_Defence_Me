using UnityEngine;

public class Shop : MonoBehaviour {
    public TurretFactory standardTurret, missileLauncher, laserBeamer;

    private BuildManager buildManager;

    void Start() {
        buildManager = BuildManager.instance;
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
