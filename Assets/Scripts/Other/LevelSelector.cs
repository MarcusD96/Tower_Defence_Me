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

        for(int i = 0; i < levelButtons.Length; i++) {
#if UNITY_EDITOR
            if(i + 1 > levelReached) {
                levelButtons[i].interactable = false;
            } else
                levelButtons[i].interactable = true;
#else
            if(i + 2 > levelReached) {
                levelButtons[i].interactable = false;
            } else
                levelButtons[i].interactable = true;
#endif
        }
    }

    public void SelectLevel(string levelName_) {
        levelName = levelName_;
        difficulty.SetActive(true);
    }

    public void SelectDifficulty(int difficultyLevel) {
        switch(difficultyLevel) {
            case 0:     //easy
                Enemy.speedDifficultyMultiplier = Enemy.hpDifficultyMultiplier = Upgrade.costDifficultyMultipler = 0.85f;
                PlayerStats.lives = 100;
                break;
            case 1:     //medium
                Enemy.speedDifficultyMultiplier = Enemy.hpDifficultyMultiplier = Upgrade.costDifficultyMultipler = 0.95f;
                PlayerStats.lives = 50;
                break;
            case 2:     //hard
                Enemy.speedDifficultyMultiplier = Enemy.hpDifficultyMultiplier = Upgrade.costDifficultyMultipler = 1.1f;
                PlayerStats.lives = 1;
                break;
            case 3:     //survival
                GameMode.survival = true;
                Enemy.speedDifficultyMultiplier = Enemy.hpDifficultyMultiplier = Upgrade.costDifficultyMultipler = 1.0f;
                PlayerStats.lives = 100;
                break;
            default:
                break;
        }
        sceneFader.FadeTo(levelName);
    }

    public void Back() {
        sceneFader.FadeTo("Main Menu");
    }

    public void CloseDifficulty() {
        difficulty.SetActive(false);
    }
}
