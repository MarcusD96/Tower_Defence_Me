
using UnityEngine;

public class SpecialActivator : MonoBehaviour {

    private static SpecialActivator instance;

    public GameObject bulletBtn, missileBtn, railgunBtn, laserBtn, teslaBtn, fireBtn, tankBtn, farmBtn, windBtn, specBtn;

    public Animator anim;

    void Awake() {
        instance = this;
        bulletBtn.SetActive(false);
        missileBtn.SetActive(false);
        railgunBtn.SetActive(false);
        laserBtn.SetActive(false);
        teslaBtn.SetActive(false);
        fireBtn.SetActive(false);
        tankBtn.SetActive(false);
        farmBtn.SetActive(false);
        windBtn.SetActive(false);
    }

    public void OpenSpecials() {
        //bool is false so shop is closed
        if(anim.GetBool("Open") == false) {
            anim.SetBool("Open", true);
            specBtn.gameObject.SetActive(false);
        }
    }

    public void CloseSpecials() {
        //bool is true so shop is open
        if(anim.GetBool("Open") == true) {
            anim.SetBool("Open", false);
            specBtn.gameObject.SetActive(true);
        }
    }

    public static void MakeBulletSpecial(BulletTurret bt) {
        instance.bulletBtn.SetActive(true);
        BulletSpecial.AddNewBulletTurret(bt);
    }

    public static void MakeMissileSpecial(MissileTurret mt) {
        instance.missileBtn.SetActive(true);
        MissileSpecial.AddNewMissileTurret(mt);
    }

    public static void MakeRailgunSpecial(RailgunTurret rt) {
        instance.railgunBtn.SetActive(true);
        RailgunSpecial.AddNewRailgunTurret(rt);
    }

    public static void MakeLaserSpecial(LaserTurret lt) {
        instance.laserBtn.SetActive(true);
        LaserSpecial.AddNewLaserTurret(lt);
    }

    public static void MakeTeslaSpecial(TeslaTurret tt) {
        instance.teslaBtn.SetActive(true);
        TeslaSpecial.AddNewTeslaTurret(tt);
    }

    public static void MakeFireSpecial(FireTurret ft) {
        instance.fireBtn.SetActive(true);
        FireSpecial.AddNewFireTurret(ft);
    }

    public static void MakeTankSpecial(TankTurret tank) {
        instance.tankBtn.SetActive(true);
        TankSpecial.AddNewTankTurret(tank);
    }

    public static void MakeFarmSpecial(FarmTower farm) {
        instance.farmBtn.SetActive(true);
        FarmSpecial.AddNewFarmTower(farm);
    }

    public static void MakeWindSpecial(WindTurret wind) {
        instance.windBtn.SetActive(true);
        WindSpecial.AddNewWindTurret(wind);
    }
}
