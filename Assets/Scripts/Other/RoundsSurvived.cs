using System.Collections;
using TMPro;
using UnityEngine;

public class RoundsSurvived : MonoBehaviour {
    public TextMeshProUGUI roundsText;

    void OnEnable() {
        StartCoroutine(AnimateText());
    }

    IEnumerator AnimateText() {
        roundsText.text = "0";
        int round = 0;

        yield return new WaitForSeconds(1.0f);

        while(round < PlayerStats.rounds) {
            round++;
            roundsText.text = round.ToString();
            yield return new WaitForSeconds(0.05f);
        }
    }
}
