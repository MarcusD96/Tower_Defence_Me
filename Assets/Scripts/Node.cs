using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour {

    public Color hoverColor, errorColor, selectColor;
    public Vector3 offset;

    [HideInInspector]
    public GameObject turret = null;
    [HideInInspector]
    public TurretFactory currentFactory;
    [HideInInspector]
    public bool isUpgraded = false;

    private Renderer rend;
    private Color startColor;
    private BuildManager buildManager;
    private Camera mainCam;

    void Start() {
        mainCam = Camera.main;
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        buildManager = BuildManager.instance;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Escape) || WaveSpawner.enemiesAlive <= 0 || PlayerStats.lives <= 0) {
            if(turret) {
                RevertTurret();
            }
        }
    }

    void OnMouseDown() {
        if(EventSystem.current.IsPointerOverGameObject())
            return;

        if(turret != null) {
            buildManager.SelectNode(this);
            return;
        }

        if(!buildManager.CanBuild)
            return;

        BuildTurret(buildManager.GetTurretToBuild());
    }

    void BuildTurret(TurretFactory turret_) {
        if(PlayerStats.money < turret_.cost) {
            return;
        }

        PlayerStats.money -= turret_.cost;
        GameObject turretGO = Instantiate(turret_.turretPrefab, GetBuildPosition(), Quaternion.identity);
        turret = turretGO;
        currentFactory = turret_;

        GameObject effect = Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5.0f);
    }

    public void UpgradeTurret() {
        if(PlayerStats.money < currentFactory.upgradeCost) {
            return;
        }

        PlayerStats.money -= currentFactory.upgradeCost;

        Destroy(turret);

        GameObject turretGO = Instantiate(currentFactory.upgradedPrefab, GetBuildPosition(), Quaternion.identity);
        turret = turretGO;

        GameObject effect = Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5.0f);

        isUpgraded = true;
    }

    public void SellTurret() {
        PlayerStats.money += currentFactory.GetSellPrice(isUpgraded);

        GameObject effect = Instantiate(buildManager.sellEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5.0f);

        Destroy(turret);
        currentFactory = null;
        isUpgraded = false;
    }

    public void ControlTurret() {
        turret.GetComponent<Turret>().AssumeControl();
        mainCam.enabled = false;
        CameraController.isEnabled = false;
    }

    void RevertTurret() {
        turret.GetComponent<Turret>().RevertControl();
        mainCam.enabled = true;
        CameraController.isEnabled = true;
    }

    void OnMouseEnter() {
        if(EventSystem.current.IsPointerOverGameObject())
            return;

        if(turret) {
            rend.material.color = selectColor;
            return;
        }

        if(!buildManager.CanBuild)
            return;

        if(buildManager.HasMoney) {
            rend.material.color = hoverColor;
        } else {
            rend.material.color = errorColor;
        }
    }

    void OnMouseExit() {
        rend.material.color = startColor;
    }

    public Vector3 GetBuildPosition() {
        return transform.position + offset;
    }

}
