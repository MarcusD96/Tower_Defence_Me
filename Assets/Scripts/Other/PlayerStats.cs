using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public static int money, lives = 1, maxLives, rounds;

    public int startMoney, numLives, numMaxLives;

    void Start() {
        money = startMoney;
        maxLives = numMaxLives;
        rounds = 0;
    }
}
