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
    
    public void PurchaseMissileTurret() {
        Debug.Log("Missile turret selected");
        buildManager.SetTurretToBuild(buildManager.missileTurretPrefab);

    }

    public void PurchaseLaserTurret() {
        //Debug.Log("Laser turret purchased");
        //buildManager.SetTurretToBuild(buildManager.standardTurretPrefab);

    }

}
