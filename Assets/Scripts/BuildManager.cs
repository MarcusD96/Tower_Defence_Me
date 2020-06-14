using UnityEngine;

public class BuildManager : MonoBehaviour {

    public static BuildManager instance;
    public GameObject standardTurretPrefab;
    public GameObject missileTurretPrefab;

    private GameObject turretToBuild;

    void Awake() {
        if(instance) {
            Debug.Log("More than 1 build manager in scene!");
            return;
        }
        instance = this;
    }

    public GameObject GetTurretToBuild() {
        return turretToBuild;
    }

    public void SetTurretToBuild(GameObject turret) {
        turretToBuild = turret;
    }
}
