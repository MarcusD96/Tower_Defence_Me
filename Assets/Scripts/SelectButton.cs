using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour {

    public Button button;
    public Image selectedBorder;
    public Color enabledBorder, disabledBorder;

    void Start() {
        button.onClick.AddListener(Hightlight);
    }

    void Hightlight() {
        selectedBorder.color = 
    }
}
