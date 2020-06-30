using UnityEngine;

public class BuildManager : MonoBehaviour {

    public static BuildManager instance;
    public GameObject buildEffect, sellEffect;
    public NodeUI nodeUI;

    private TurretFactory turretToBuild;
    private Node selectedNode;

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
        if(turretToBuild != null)
            turretToBuild.border.gameObject.SetActive(false); //disable previous selection border
        turretToBuild = turret;
        turretToBuild.border.gameObject.SetActive(true); //enable new selection border
        DeselectNode();
    }

    public void SelectNode(Node node) {
        if(selectedNode == node) {
            DeselectNode();
            return;
        }
        selectedNode = node;
        if(turretToBuild != null) {
            turretToBuild.border.gameObject.SetActive(false); //disable current selection border if you go into the nodeUI menu
        }
        turretToBuild = null;
        nodeUI.SetTarget(node);
    }

    public void DeselectNode() {
        selectedNode = null;
        nodeUI.Hide();
    }

    public TurretFactory GetTurretToBuild() {
        return turretToBuild;
    }
}
