
using System.Collections;
using UnityEngine;

public class FarmTower : Turret {

    [Header("Farm Stats")]
    public int value, numSpawns;
    public GameObject valueIndicator;

    float interval, startDelay;
    IEnumerator giveMoney;

    protected new void Start() {
        farmTower = this;
        ResetForWave();
    }

    bool reset = false;
    private new void Update() {
        if(WaveSpawner.enemiesAlive <= 0) {
            ResetForWave();
        } else if(reset){
            reset = false;
            if(giveMoney == null) {
                giveMoney = GiveMoney();
                StartCoroutine(giveMoney);
            }
        }
    }

    void ResetForWave() {
        if(reset == true) //has already been reset, don't do it again
            return;

        reset = true;
        float waveTime = WaveSpawner.instance.GetCurrentWave().GetWaveTime();
        startDelay = waveTime / numSpawns;
        interval = (waveTime - (startDelay * 2)) / numSpawns;
    }

    public new void ApplyUpgradeA() { //more spawns
        numSpawns += Mathf.RoundToInt(ugA.GetLevel() * ugA.upgradeFactorX);
        ugA.IncreaseUpgrade(true);
    }

    public override void ApplyUpgradeB() { //increase value
        value = Mathf.RoundToInt(value * (1 + ugB.upgradeFactorX) / 5) * 5; //stay rounded to 5
    }

    public new void ApplySpecial() {
        //every money drop has a chance of giving back 1 life
    }

    public override bool ActivateSpecial() {
        //killing enemies are worth double
        return true;
    }

    IEnumerator GiveMoney() {
        yield return new WaitForSeconds(startDelay);
        for(int i = 0; i < numSpawns; i++) {
            PlayerStats.money += value;
            GameObject indicatorInstance = Instantiate(valueIndicator, fireSpawn.position, Quaternion.identity);
            indicatorInstance.transform.SetParent(fireSpawn, true);
            var indicator = indicatorInstance.GetComponent<DamageIndicator>();
            indicator.color = Color.green;
            indicator.damage = value;

            yield return new WaitForSeconds(interval);
        }
        giveMoney = null;
    }

}
