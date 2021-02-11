
using UnityEngine;

public class SpecialActivator : MonoBehaviour {

    private static SpecialActivator instance;

    public GameObject barrageButton, burstButton, chargesButton, empButton, superchargeButton, specBtn;


    public Animator anim;

    void Awake() {
        instance = this;
        burstButton.SetActive(false);
        barrageButton.SetActive(false);
        chargesButton.SetActive(false);
        empButton.SetActive(false);
        superchargeButton.SetActive(false);
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
        Burst.AddNewBulletTurret(bt);
    }

    public static void MakeBarrage(MissileTurret mt) {
        instance.barrageButton.SetActive(true);
        Barrage.AddNewMissileTurret(mt);
    }

    public static void MakeCharges(RailgunTurret rt) {
        instance.chargesButton.SetActive(true);
        Charges.AddNewRailgunTurret(rt);
    }

    public static void MakeEMP(LaserTurret lt) {
        instance.empButton.SetActive(true);
        EMP.AddNewLaserTurret(lt);
    }

    public static void MakeSuperChrge(TeslaTurret tt) {
        instance.superchargeButton.SetActive(true);
        Supercharge.AddNewTeslaTurret(tt);
    }
}
