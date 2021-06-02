
using TMPro;
using UnityEngine;

public class ExploreTurrets : MonoBehaviour {
    public TextMeshProUGUI turretName, description, upgrades;

    public Transform spawn;
    public float speed;
    public GameObject[] turrets;

    GameObject currentGO;
    MenuTurret currentComp;
    int currentIndex;

    private void Start() {
        currentIndex = 0;
        SpawnTurret();
        currentGO.GetComponentInChildren<Animator>().enabled = false;
    }

    private void Update() {
        spawn.Rotate(Vector3.up * Time.deltaTime * speed);
    }

    void SpawnTurret() {
        currentGO = Instantiate(turrets[currentIndex], spawn);
        currentComp = currentGO.GetComponent<MenuTurret>();
        UpdateStats();
        currentComp.enabled = false;
    }

    public void Next() {
        Destroy(currentGO);
        currentIndex++;
        if(currentIndex > turrets.Length - 1) {
            currentIndex = 0;
        }
        SpawnTurret();
        if(currentGO.GetComponentInChildren<Animator>())
            currentGO.GetComponentInChildren<Animator>().enabled = false;
        UpdateStats();
    }

    public void Last() {
        Destroy(currentGO);
        currentIndex--;
        if(currentIndex < 0) {
            currentIndex = turrets.Length - 1;
        }
        SpawnTurret();
        if(currentGO.GetComponentInChildren<Animator>())
            currentGO.GetComponentInChildren<Animator>().enabled = false;
    }

    void UpdateStats() {
        turretName.text = currentComp.turretName;
        description.text = currentComp.description;
        upgrades.text = "<color=red>" + currentComp.upgradeAName + ": </color>" + currentComp.upgradeA +
                        "\n<color=red>" + currentComp.upgradeBName + ": </color>" + currentComp.upgradeB +
                        "\n<color=blue>" + currentComp.specName + ": </color>" + currentComp.upgradeSpec;
    }
}
