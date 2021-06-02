using UnityEngine;

public class MainMenu : MonoBehaviour {
    public string levelToLoad = "Level Selection";
    public string survivalLevelToLoad = "Survival";
    public SceneFader sceneFader;
    public GameObject settingsUI;

    public Transform turretSpot;
    public GameObject[] menuTurrets;

    private void Awake() {
        Time.timeScale = 1;
        GameObject t = menuTurrets[Random.Range(0, menuTurrets.Length)];
        //t = menuTurrets[5];
        Instantiate(t, turretSpot);
    }

    public void Play() {
        GameMode.survival = false;
        sceneFader.FadeTo(levelToLoad);
    }

    public void Turrets() {
        sceneFader.FadeTo("Explore Turrets");
    }

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void OpenSettings() {
        settingsUI.SetActive(true);
        settingsUI.GetComponent<SettingsMenu>().InitialUpdate();
    }

    public void ResetStats() {
        PlayerPrefs.DeleteAll();
    }
}

public static class GameMode {
    public static bool survival = false;
}
