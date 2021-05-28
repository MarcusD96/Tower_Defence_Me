
using System.Collections;
using UnityEngine;

public class FarmTower : Turret {

    public int specialCashNumSpawns;
    public float specialCashValue;

    [Header("Farm Stats")]
    public int cashValue;
    public int numSpawns, lifeValue, lifeChance;
    public GameObject valueIndicator;

    float interval, startDelay;
    IEnumerator cashDrop;

    protected new void Start() {
        farmTower = this;
        ResetForWave();
    }

    bool reset = false;

    private new void Update() {
        if(WaveSpawner.enemiesAlive <= 0) {
            ResetForWave();
        } else if(reset) {
            reset = false;
            if(cashDrop == null) {
                cashDrop = CashDrop();
                StartCoroutine(cashDrop);
            }
        }
        if(specialAmount <= 0)
            specialActivated = false;
    }

    void ResetForWave() {
        if(reset == true) //has already been reset, don't do it again
            return;

        if(cashDrop != null) {
            StopCoroutine(cashDrop);
            cashDrop = null;
        }
        reset = true;
        float waveTime = WaveSpawner.instance.GetCurrentWave().GetWaveTime();
        startDelay = waveTime / numSpawns;
        interval = (waveTime - (startDelay * 2)) / numSpawns;
    }

    public new void ApplyUpgradeA() { //more spawns
        numSpawns += Mathf.RoundToInt(ugA.GetLevel() * ugA.upgradeFactorX);
        ugA.SetUpgradeCost(ugA.GetUpgradeCost() * 2);
    }

    public override void ApplyUpgradeB() { //increase value
        cashValue = Mathf.CeilToInt(cashValue * (1 + ugB.upgradeFactorX) / 5) * 5; //stay rounded to 5
    }

    public new void ApplySpecial() {
        hasSpecial = true;
    }

    public override bool ActivateSpecial() {
        //killing enemies are worth double
        if(!specialActivated && WaveSpawner.enemiesAlive > 0) {
            specialActivated = true;
            StartCoroutine(CashInjection());
            return true;
        }
        return false;
    }

    DamageIndicator ShowDrop(Color color, float num, float distance) {
        GameObject indicatorInstance = Instantiate(valueIndicator, fireSpawn.position, Quaternion.identity);
        indicatorInstance.transform.SetParent(fireSpawn, true);
        DamageIndicator indicator = indicatorInstance.GetComponent<DamageIndicator>();
        indicator.color = color;
        indicator.damage = num;
        if(distance > 0) {
            indicator.distance = distance;
        }
        return indicator;
    }

    IEnumerator CashDrop() {
        yield return new WaitForSeconds(startDelay);
        for(int i = 0; i < numSpawns; i++) {
            PlayerStats.money += cashValue;
            ShowDrop(Color.green, cashValue, 0);

            if(hasSpecial) {
                int r = Random.Range(0, 100 / lifeChance);
                if(r == 0) {
                    yield return new WaitForEndOfFrame();
                    PlayerStats.lives += lifeValue;
                    ShowDrop(Color.red, lifeValue, 3);
                }
            }

            yield return new WaitForSeconds(interval);
        }
        cashDrop = null;
    }

    IEnumerator CashInjection() {
        StartCoroutine(SpecialTime());

        float startTime = Time.time;
        for(int i = 0; i < specialCashNumSpawns; i++) {
            PlayerStats.money += specialCashValue;
            ShowDrop(Color.cyan, specialCashValue, 10);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
