
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Node : MonoBehaviour {

    public Color hoverColor, errorColor, selectColor;
    public Vector3 offset;
    public RawImage range;

    [HideInInspector]
    public GameObject turret = null;

    [HideInInspector]
    public TurretFactory currentFactory;

    private Renderer rend;
    private Color startColor;
    private BuildManager buildManager;
    private Camera mainCam;
    private bool controlled = false;

    void Start() {
        mainCam = Camera.main;
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        buildManager = BuildManager.instance;
    }

    void Update() {
        if(WaveSpawner.enemiesAlive <= 0) {
            if(turret)
                if(controlled) {
                    RevertTurret(true);
                }
            return;
        }

        if(Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.Escape) || PlayerStats.lives <= 0) {
            if(turret) {
                RevertTurret(false);
            }
        }
        if(WaveSpawner.enemiesAlive > 0 && GameManager.lastControlled) {
            buildManager.DeselectNode();
        }
    }

    void BuildEffect(GameObject effect) {
        var effect_ = Instantiate(effect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect_, 3.0f);
    }

    void BuildTurret(TurretFactory turret_) {
        if(PlayerStats.money < turret_.GetCost()) {
            return;
        }

        PlayerStats.money -= turret_.GetCost();
        GameObject turretGO = Instantiate(turret_.turretPrefab, GetBuildPosition(), Quaternion.identity);
        turret = turretGO;
        currentFactory = turret_;

        var t = turret.GetComponent<Turret>();
        t.sellPrice = turret_.GetCost();

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
        t.sellPrice += t.ugA.GetUpgradeCost();

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
        t.sellPrice += t.ugB.GetUpgradeCost();

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
        t.sellPrice += t.ugSpec.GetUpgradeCost();
        t.ugSpec.IncreaseUpgrade();

        t.EnableSpecial();

        CheckTurretTypeAndMakeSpecial();
    }

    void CheckTurretTypeAndMakeSpecial() {
        //bullet
        if(turret.GetComponent<BulletTurret>()) {
            SpecialActivator.MakeBurst(turret.GetComponent<BulletTurret>());
        }
        //missile
        else if(turret.GetComponent<MissileTurret>()) {
            SpecialActivator.MakeBarrage(turret.GetComponent<MissileTurret>());
        }
        //railgun
        else if(turret.GetComponent<RailgunTurret>()) {
            SpecialActivator.MakeCharges(turret.GetComponent<RailgunTurret>());
        }
        //laser
        else if(turret.GetComponent<LaserTurret>()) {
            SpecialActivator.MakeEMP(turret.GetComponent<LaserTurret>());
        }
        //tesla
        else if(turret.GetComponent<TeslaTurret>()) {
            SpecialActivator.MakeSuperChrge(turret.GetComponent<TeslaTurret>());
        }
    }

    public void SellTurret() {
        PlayerStats.money += turret.GetComponent<Turret>().GetSellPrice();

        BuildEffect(buildManager.sellEffect);

        CheckTurretTypeToSell();

        Destroy(turret);
        currentFactory = null;
    }

    void CheckTurretTypeToSell() {
        //bullet
        if(turret.GetComponent<BulletTurret>()) {
            Burst.RemoveTurret(turret.GetComponent<BulletTurret>());
        }
        //missile
        else if(turret.GetComponent<MissileTurret>()) {
            Barrage.RemoveTurret(turret.GetComponent<MissileTurret>());
        }
        //railgun
        else if(turret.GetComponent<RailgunTurret>()) {
            Charges.RemoveTurret(turret.GetComponent<RailgunTurret>());
        }
        //laser
        else if(turret.GetComponent<LaserTurret>()) {
            EMP.RemoveTurret(turret.GetComponent<LaserTurret>());
        }
        //tesla
        else if(turret.GetComponent<TeslaTurret>()) {
            Supercharge.RemoveTurret(turret.GetComponent<TeslaTurret>());
        }
    }

    public void ControlTurret() {
        controlled = true;
        turret.GetComponent<Turret>().AssumeControl();
        mainCam.enabled = false;
        CameraController.isEnabled = false;
    }

    void RevertTurret(bool roundEnd) {
        controlled = false;
        turret.GetComponent<Turret>().RevertControl(roundEnd);
        CameraController.isEnabled = true;
    }

    void UpdateRange(Turret t) {
        var scale = range.rectTransform.localScale;
        scale.x = t.range;
        scale.y = t.range;
        range.rectTransform.localScale = scale;
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
