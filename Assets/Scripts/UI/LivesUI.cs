using UnityEngine;
using TMPro;

public class LivesUI : MonoBehaviour {

    public TextMeshProUGUI lives;

    void Update() {
        if(PlayerStats.lives == 1) {
            lives.text = PlayerStats.lives.ToString() + " Life";
        } else {
            lives.text = PlayerStats.lives.ToString() + " Lives";
        }
    }

}