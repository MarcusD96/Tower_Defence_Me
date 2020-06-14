using UnityEngine;

public class Shop : MonoBehaviour {

    BuildManager buildManager;

    void Start() {
        buildManager = BuildManager.instance;
    }

    public void PurchaseStandardTurret() {
        Debug.Log("Standard turret selected");
        buildManager.SetTurretToBuild(buildManager.standardTurretPrefab);
    }
    
    public void PurchaseMissileLauncher() {
        Debug.Log("Missile launcher selected");
        buildManager.SetTurretToBuild(buildManager.missileLauncherPrefab);

    }

    public void PurchaseLaserTurret() {
        //Debug.Log("Laser turret purchased");
        //buildManager.SetTurretToBuild(buildManager.standardTurretPrefab);

    }

}
