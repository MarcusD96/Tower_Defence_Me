
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour {

    private Color hoverColor = Color.grey, errorColor = Color.red, selectColor = Color.blue;

    [HideInInspector]
    public GameObject turret = null;

    [HideInInspector]
    public TurretFactory currentFactory;

    private Renderer rend;
    private Color startColor;
    private BuildManager buildManager;
    private Camera mainCam;
    private bool controlled = false;

    public GameObject range;

    void Start() {
        mainCam = Camera.main;
        rend = GetComponent<Renderer>();
        startColor = rend.sharedMaterial.color;
        buildManager = BuildManager.instance;
        range.SetActive(false);
    }

    void Update() {
        if(mainCam == null)
            mainCam = Camera.main;

        //wave ends, check if turrret was controlled
        if(WaveSpawner.enemiesAlive <= 0) {
            if(turret) {
                if(controlled) {
                    RevertTurret(true);
                }
            }
            return;
        }

        //manual key pressed/player dies, revert turret control
        if(Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.Escape) || PlayerStats.lives <= 0) {
            if(turret) {
                RevertTurret(false);
            }
        }

        //wave starts and turret was controlled before previous round ended
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
        t.AttachNode(this);

        UpdateRange(t);
        range.SetActive(false);

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
        t.ugA.IncreaseUpgrade(false);
        t.ApplyUpgradeA();
        UpdateRange(t);
    }

    public void UpgradeB() {
        var t = turret.GetComponent<Turret>();

        if(PlayerStats.money < t.ugB.GetUpgradeCost()) {
            return;
        }

        BuildEffect(buildManager.upgradeEffect);

        PlayerStats.money -= t.ugB.GetUpgradeCost();
        t.sellPrice += t.ugB.GetUpgradeCost();

        t.ugB.IncreaseUpgrade(true);
        t.ApplyUpgradeB();
    }

    public void UpgradeSpecial() {
        var t = turret.GetComponent<Turret>();

        if(PlayerStats.money < t.ugSpec.GetUpgradeCost()) {
            return;
        }

        BuildEffect(buildManager.upgradeEffect);

        PlayerStats.money -= t.ugSpec.GetUpgradeCost();
        t.sellPrice += t.ugSpec.GetUpgradeCost();
        t.ugSpec.IncreaseUpgrade(false);

        t.ApplySpecial();

        CheckTurretTypeAndMakeSpecial();
    }

    void CheckTurretTypeAndMakeSpecial() {
        //bullet
        if(turret.GetComponent<BulletTurret>())
            SpecialActivator.MakeBurst(turret.GetComponent<BulletTurret>());

        //missile
        else if(turret.GetComponent<MissileTurret>())
            SpecialActivator.MakeBarrage(turret.GetComponent<MissileTurret>());

        //railgun
        else if(turret.GetComponent<RailgunTurret>())
            SpecialActivator.MakeCharges(turret.GetComponent<RailgunTurret>());

        //laser
        else if(turret.GetComponent<LaserTurret>())
            SpecialActivator.MakeEMP(turret.GetComponent<LaserTurret>());

        //tesla
        else if(turret.GetComponent<TeslaTurret>())
            SpecialActivator.MakeSuperCharge(turret.GetComponent<TeslaTurret>());

        //fire
        else if(turret.GetComponent<FireTurret>())
            SpecialActivator.MakeInferno(turret.GetComponent<FireTurret>());

        //tank
        else if(turret.GetComponent<TankTurret>())
            SpecialActivator.MakeRundown(turret.GetComponent<TankTurret>());

        //farm
        else if(turret.GetComponent<FarmTower>()) {
            SpecialActivator.MakeDoubleCash(turret.GetComponent<FarmTower>());
        }
    }

    public void SellTurret() {
        if(GameManager.lastControlled = this) {
            GameManager.lastControlled = null;
        }

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
            MegaShot.RemoveTurret(turret.GetComponent<RailgunTurret>());
        }
        //laser
        else if(turret.GetComponent<LaserTurret>()) {
            AudioManager.StaticStop(turret.GetComponent<LaserTurret>().shootSound);
            EMP.RemoveTurret(turret.GetComponent<LaserTurret>());
        }
        //tesla
        else if(turret.GetComponent<TeslaTurret>()) {
            Supercharge.RemoveTurret(turret.GetComponent<TeslaTurret>());
        }
    }

    public void ControlTurret() {
        GameManager.lastControlled = this;
        controlled = true;
        turret.GetComponent<Turret>().AssumeControl();
        mainCam.enabled = false;
        CameraController.isEnabled = false;
    }

    public void RevertTurret(bool roundEnd) {
        controlled = false;
        turret.GetComponent<Turret>().RevertControl(roundEnd);
        CameraController.isEnabled = true;
    }

    public void UpdateRange(Turret t) {
        range.transform.localScale = new Vector3(t.range / 2.5f, 0.01f, t.range / 2.5f);
    }

    public Vector3 GetBuildPosition() {
        return transform.position + (Vector3.up * 0.5f);
    }

    void OnMouseDown() {
        if(GameManager.lastControlled != null) {
            if(GameManager.lastControlled.controlled)   //cant select nodes when a turret is being controlled
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
            if(GameManager.lastControlled.controlled)   //cant select nodes when a turret is being controlled
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
            range.SetActive(true);
        } else {
            rend.material.color = errorColor;
        }
    }

    void OnMouseExit() {
        rend.material.color = startColor;
        if(!turret) {
            range.SetActive(false);
        }
    }

    private void OnDrawGizmos() {
        //Gizmos.color = Color.red;
        //if(over) {
        //    if(buildManager.GetTurretToBuild() != null) {
        //        Gizmos.DrawWireSphere(transform.position, buildManager.GetTurretToBuild().GetTurret().range);
        //    }
        //}

        //if(turret != null) {
        //    Gizmos.DrawWireSphere(transform.position, turret.GetComponent<Turret>().range);
        //}
    }
}
