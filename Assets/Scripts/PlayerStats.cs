using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public static int money, lives, maxLives;


    public int startMoney, numLives, numMaxLives;

    void Start() {
        money = startMoney;
        maxLives = numMaxLives;
        lives = numLives;
    }

}
