
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    public Slider sounds, sensitivity;
    public Toggle useKeys;
    public Button apply;
    public TextMeshProUGUI soundsNum, sensNum;

    private void LateUpdate() {
        soundsNum.text = Mathf.RoundToInt(sounds.value * 100).ToString();
        sensNum.text = Mathf.RoundToInt(sensitivity.value* 100).ToString();
    }

    public void ApplyChanges() {
        Settings.Sounds = sounds.value;
        Settings.Sensitivity = sensitivity.value;
        Settings.UseKeys = useKeys.isOn;
        gameObject.SetActive(false);
    }

    public void InitialUpdate() {
        sounds.value = Settings.Sounds;
        sensitivity.value = Settings.Sensitivity;
        useKeys.isOn = Settings.UseKeys;
    }
}
