using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour {

    public TextMeshProUGUI moneyText;

    // Update is called once per frame
    void Update() {
        moneyText.text = "$" + (Mathf.Round(PlayerStats.money / 5) * 5).ToString(); //round to the nearest 5
    }
}
