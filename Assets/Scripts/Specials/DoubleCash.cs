
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoubleCash : MonoBehaviour {

    private static DoubleCash instance = null;

    public List<FarmTower> turrets;
    public Image recharge;
    public TextMeshProUGUI num;

    void Awake() {
        instance = this;
        turrets = new List<FarmTower>();
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
                recharge.fillAmount = turrets[0].specialAmount / turrets[0].specialRate;
                break;
            }

            if(i == 0) { //skip the first element so the next statement doesnt error out
                continue;
            }

            //always show the LEAST fill of all abilities
            if(t.specialAmount > turrets[i - 1].specialAmount) { //previous is more full
                recharge.fillAmount = turrets[i - 1].specialAmount / turrets[i - 1].specialRate;
            } else {
                recharge.fillAmount = t.specialAmount / t.specialRate;
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

    public static void AddNewFarmTower(FarmTower farm) {
        instance.turrets.Add(farm);
    }

    public static void RemoveTower(FarmTower farm) {
        if(!instance)
            return;

        if(instance.turrets.Count > 0)
            instance.turrets.Remove(farm);

        if(instance.turrets.Count < 1) {
            instance.gameObject.SetActive(false);
        }
    }

    public void DoSpecial() {
        foreach(var t in turrets) {
            if(t.specialAmount <= 0) {
                if(t.ActivateSpecial())
                    break;
            }
        }
    }
}
