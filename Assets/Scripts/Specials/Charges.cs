
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Charges : MonoBehaviour {

    private static Charges instance;

    public List<RailgunTurret> turrets;
    public Image recharge;
    public TextMeshProUGUI num;

    void Awake() {
        instance = this;
        turrets = new List<RailgunTurret>();
        num.gameObject.SetActive(false);
    }

    void Update() {
        if(turrets.Count <= 1) {
            num.gameObject.SetActive(false);
        } else {
            num.gameObject.SetActive(true);
            num.text = turrets.Count.ToString();
        }

        int i = -1;
        foreach(var t in turrets) {
            i++;

            if(turrets.Count == 1) {
                recharge.fillAmount = turrets[0].specialBar.fillBar.fillAmount;
                break;
            }

            if(i == 0) { //skip the first element so the next statement doesnt error out
                continue;
            }

            //always show the LEAST fill of all abilities
            if(t.specialBar.fillBar.fillAmount > turrets[i - 1].specialBar.fillBar.fillAmount) { //previous is more full
                recharge.fillAmount = turrets[i - 1].specialBar.fillBar.fillAmount;
            } else {
                recharge.fillAmount = t.specialBar.fillBar.fillAmount;
            }
        }

        if(recharge.fillAmount <= 0) {
            Color hide = recharge.color;
            hide.a = 0;
            recharge.color = hide;
        } else {
            Color show = recharge.color;
            show.a = 0.7f;
            recharge.color = show;
        }
    }

    public static void AddNewRailgunTurret(RailgunTurret rgt) {
        instance.turrets.Add(rgt);
    }

    public static void RemoveTurret(RailgunTurret rgt) {
        instance.turrets.Remove(rgt);
        if(instance.turrets.Count < 1) {
            instance.gameObject.SetActive(false);
        }
    }

    public void DoSpecial() {
        foreach(var t in turrets) {
            if(t.specialBar.fillBar.fillAmount <= 0) {
                t.ActivateSpecial();
                break;
            }
        }
    }
}
