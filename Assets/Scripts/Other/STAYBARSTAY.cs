
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class STAYBARSTAY : MonoBehaviour {
    Scrollbar scrollbar;

    private void Awake() {
        scrollbar = GetComponent<Scrollbar>();
            scrollbar.value = 0;
    }
}
