
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class STAYBARSTAY : MonoBehaviour {
    Scrollbar scrollbar;

    private void LateUpdate() {
        scrollbar = GetComponent<Scrollbar>();
            scrollbar.value = 0;
    }
}
