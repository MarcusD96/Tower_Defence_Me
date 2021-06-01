﻿
using UnityEngine;

public class SpecialActivator : MonoBehaviour {

    private static SpecialActivator instance;

    public GameObject barrageButton, burstButton, chargesButton, empButton, superchargeButton, infernoBtn, rundownBtn, doubleCashBtn, specBtn;


    public Animator anim;

    void Awake() {
        instance = this;
        burstButton.SetActive(false);
        barrageButton.SetActive(false);
        chargesButton.SetActive(false);
        empButton.SetActive(false);
        superchargeButton.SetActive(false);
        infernoBtn.SetActive(false);
        rundownBtn.SetActive(false);
        doubleCashBtn.SetActive(false);
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

    public static void MakeBurst(BulletTurret bt) {
        instance.burstButton.SetActive(true);
        BulletSpecial.AddNewBulletTurret(bt);
    }

    public static void MakeBarrage(MissileTurret mt) {
        instance.barrageButton.SetActive(true);
        MissileSpecial.AddNewMissileTurret(mt);
    }

    public static void MakeCharges(RailgunTurret rt) {
        instance.chargesButton.SetActive(true);
        RailgunSpecial.AddNewRailgunTurret(rt);
    }

    public static void MakeEMP(LaserTurret lt) {
        instance.empButton.SetActive(true);
        LaserSpecial.AddNewLaserTurret(lt);
    }

    public static void MakeSuperCharge(TeslaTurret tt) {
        instance.superchargeButton.SetActive(true);
        TeslaSpecial.AddNewTeslaTurret(tt);
    }

    public static void MakeInferno(FireTurret ft) {
        instance.infernoBtn.SetActive(true);
        FireSpecial.AddNewFireTurret(ft);
    }

    public static void MakeRundown(TankTurret tank) {
        instance.rundownBtn.SetActive(true);
        TankSpecial.AddNewTankTurret(tank);
    }

    public static void MakeDoubleCash(FarmTower farm) {
        instance.doubleCashBtn.SetActive(true);
        FarmSpecial.AddNewFarmTower(farm);
    }
}
