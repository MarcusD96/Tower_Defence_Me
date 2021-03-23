using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour {

    public SceneFader sceneFader;
    public Button[] levelButtons;
    public GameObject difficulty;

    private string levelName;

    void Start() {
        difficulty.SetActive(false);

        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        //**********CHANGE WHEN NOT IN DEVELOPMENT BACK TO 1*************//
        levelReached = 6;

        for(int i = 0; i < levelButtons.Length; i++) {
            if(i + 1 > levelReached) {
                levelButtons[i].interactable = false;
            } else
                levelButtons[i].interactable = true;
        }
    }

    public void SelectLevel(string levelName_) {
        levelName = levelName_;
        PlayerStats.nextLevel = levelName_;
        difficulty.SetActive(true);
    }

    public void SelectDifficulty(int difficultyLevel) {
        PlayerStats.difficulty = difficultyLevel;
        PlayerStats.ResetToDifficulty();
        sceneFader.FadeTo("Controls");
    }

    public void Back() {
        sceneFader.FadeTo("Main Menu");
    }

    public void CloseDifficulty() {
        difficulty.SetActive(false);
    }
}
