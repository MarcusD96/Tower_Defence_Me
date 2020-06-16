using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public static int money, lives, maxLives, rounds;


    public int startMoney, numLives, numMaxLives;

    void Start() {
        money = startMoney;
        maxLives = numMaxLives;
        lives = numLives;
        rounds = 0;
    }

}
