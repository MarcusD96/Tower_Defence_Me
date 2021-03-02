
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    public Slider sounds, music, sensitivity;
    public TextMeshProUGUI soundsNum, musicNum, sensNum;
    public Toggle useKeys;
    public Button apply;

    private void LateUpdate() {
        soundsNum.text = Mathf.RoundToInt(sounds.value * 100).ToString();
        musicNum.text = Mathf.RoundToInt(music.value * 100).ToString();
        sensNum.text = Mathf.RoundToInt(sensitivity.value * 100).ToString();
    }

    public void ApplyChanges() {
        Settings.Sounds = sounds.value;
        Settings.Music = music.value;
        AudioManager.instance.currentSong.volume = music.value;
        Settings.Sensitivity = sensitivity.value;
        Settings.UseKeys = useKeys.isOn;
        gameObject.SetActive(false);
    }

    public void InitialUpdate() {
        sounds.value = Settings.Sounds;
        music.value = Settings.Music;
        sensitivity.value = Settings.Sensitivity;
        useKeys.isOn = Settings.UseKeys;
    }
}
