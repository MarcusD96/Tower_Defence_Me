using TMPro;
using UnityEngine;

public class LivesUI : MonoBehaviour {

    public TextMeshProUGUI lives;

    void Update() {
        lives.text = "Lives: " + PlayerStats.lives.ToString();
    }
}