
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    public Slider sounds, music, sensitivity;
    public TextMeshProUGUI soundsNum, musicNum, songName, sensNum;
    public Toggle useKeys, returnTurret, autoStart;
    public Button apply, nextSong;

    private void LateUpdate() {
        soundsNum.text = Mathf.RoundToInt(sounds.value).ToString();
        musicNum.text = Mathf.RoundToInt(music.value).ToString();
        sensNum.text = Mathf.RoundToInt(sensitivity.value).ToString();
        songName.text = AudioManager.instance.currentSong.name;
    }

    public void ApplyChanges() {
        Settings.Sounds = sounds.value;
        Settings.Music = music.value;
        AudioManager.instance.currentSong.source.volume = music.value;
        AudioManager.instance.UpdateVolume();
        Settings.Sensitivity = sensitivity.value;
        Settings.UseKeys = useKeys.isOn;
        Settings.ReturnTurret = returnTurret.isOn;
        Settings.AutoStart = autoStart.isOn;
        gameObject.SetActive(false);
    }

    public void InitialUpdate() {
        sounds.value = Settings.Sounds;
        music.value = Settings.Music;
        sensitivity.value = Settings.Sensitivity;
        useKeys.isOn = Settings.UseKeys;
        returnTurret.isOn = Settings.ReturnTurret;
        autoStart.isOn = Settings.AutoStart;
    }

    public void NextSong() {
        AudioManager.StaticNextSong();
    }
}
