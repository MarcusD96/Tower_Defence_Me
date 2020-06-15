using UnityEngine;

public class BuildManager : MonoBehaviour {

    public static BuildManager instance;
    public GameObject buildEffect;

    private TurretFactory turretToBuild;

    void Awake() {
        if(instance) {
            Debug.Log("More than 1 build manager in scene!");
            return;
        }
        instance = this;
    }

    public bool CanBuild {
        get {
            return turretToBuild != null;
        }
    }

    public bool HasMoney {
        get {
            return PlayerStats.money >= turretToBuild.cost;
        }
    }

    public void SelectTurretToBuild(TurretFactory turret) {
        turretToBuild = turret;
    }

    public void BuildTurretOn(Node node) {
        if(PlayerStats.money < turretToBuild.cost) {
            return;
        }

        PlayerStats.money -= turretToBuild.cost;
        GameObject turret = Instantiate(turretToBuild.turretPrefab, node.GetBuildPosition(), Quaternion.identity);
        node.turret = turret;

        GameObject effect = Instantiate(buildEffect, node.GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5.0f);
    }
}
