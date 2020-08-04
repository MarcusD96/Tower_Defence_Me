﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Node : MonoBehaviour {

    public Color hoverColor, errorColor, selectColor;
    public Vector3 offset;
    public Image range;

    [HideInInspector]
    public GameObject turret = null, controlledTurret = null;
    [HideInInspector]
    public TurretFactory currentFactory;

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
        if(Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape) || PlayerStats.lives <= 0) {
            if(turret) {
                RevertTurret(false);
            }
        }
        if(WaveSpawner.enemiesAlive <= 0) {
            if(turret)
                RevertTurret(true);
        }
        if(WaveSpawner.enemiesAlive > 0 && GameManager.lastControlled) {
            buildManager.DeselectNode();
        }
    }

    void BuildEffect(GameObject effect) {
        var effect_ = Instantiate(effect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect_, 5.0f);
    }

    void BuildTurret(TurretFactory turret_) {
        if(PlayerStats.money < turret_.cost) {
            return;
        }

        PlayerStats.money -= turret_.cost;
        GameObject turretGO = Instantiate(turret_.turretPrefab, GetBuildPosition(), Quaternion.identity);
        turret = turretGO;
        currentFactory = turret_;

        var t = turret.GetComponent<Turret>();
        t.cost = turret_.cost;

        UpdateRange(t);
        range.gameObject.SetActive(false);

        BuildEffect(buildManager.buildEffect);
    }

    public void UpgradeA() { //range always
        var t = turret.GetComponent<Turret>();

        if(PlayerStats.money < t.ugA.GetUpgradeCost()) {
            return;
        }

        PlayerStats.money -= t.ugA.GetUpgradeCost();
        t.cost += t.ugA.GetUpgradeCost();

        BuildEffect(buildManager.upgradeEffect);
        t.ApplyUpgradeA();
        UpdateRange(t);
        t.ugA.IncreaseUpgrade();
    }

    public void UpgradeB() {
        var t = turret.GetComponent<Turret>();

        if(PlayerStats.money < t.ugB.GetUpgradeCost()) {
            return;
        }

        BuildEffect(buildManager.upgradeEffect);

        PlayerStats.money -= t.ugB.GetUpgradeCost();
        t.cost += t.ugB.GetUpgradeCost();

        t.ApplyUpgradeB();
        t.ugB.IncreaseUpgrade();
    }

    public void UpgradeSpecial() {
        var t = turret.GetComponent<Turret>();

        if(PlayerStats.money < t.ugSpec.GetUpgradeCost()) {
            return;
        }

        BuildEffect(buildManager.upgradeEffect);

        PlayerStats.money -= t.ugSpec.GetUpgradeCost();
        t.cost += t.ugSpec.GetUpgradeCost();

        t.EnableSpecial();
        t.ugSpec.IncreaseUpgrade();
    }

    public void SellTurret() {
        PlayerStats.money += turret.GetComponent<Turret>().GetSellPrice();

        BuildEffect(buildManager.sellEffect);

        Destroy(turret);
        currentFactory = null;
    }

    public void ControlTurret() {
        turret.GetComponent<Turret>().AssumeControl();
        mainCam.enabled = false;
        CameraController.isEnabled = false;
    }

    void RevertTurret(bool roundEnd) {
        turret.GetComponent<Turret>().RevertControl(roundEnd);
        mainCam.enabled = true;
        CameraController.isEnabled = true;
    }

    void UpdateRange(Turret t) {
        RectTransform rt = range.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(t.range / 2, t.range / 2);
    }

    void OnMouseDown() {
        if(GameManager.lastControlled != null) {
            if(GameManager.lastControlled.manual)   //cant select nodes when a turret is being controlled
                return;
        }

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

    void OnMouseEnter() {
        if(GameManager.lastControlled != null) { 
            if(GameManager.lastControlled.manual)   //cant select nodes when a turret is being controlled
                return;
        }

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
            var t = buildManager.GetTurretToBuild().GetTurret();
            UpdateRange(t);
            range.gameObject.SetActive(true);
        } else {
            rend.material.color = errorColor;
        }
    }

    void OnMouseExit() {
        rend.material.color = startColor;
        if(!turret) {
            range.gameObject.SetActive(false); 
        }
    }

    public Vector3 GetBuildPosition() {
        return transform.position + offset;
    }
}