
using UnityEngine;

public class LineBlur : MonoBehaviour {

    public LineRenderer parentLR;
    public bool canFade = false;
    LineRenderer lr;
    float startAlpha;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
        startAlpha = lr.startColor.a;
    }

    private void LateUpdate() {
        lr.enabled = parentLR.enabled;
        lr.positionCount = parentLR.positionCount;
        for(int i = 0; i < parentLR.positionCount; i++) {
            lr.SetPosition(i, parentLR.GetPosition(i));
        }
        lr.startWidth = lr.endWidth = parentLR.endWidth + 0.2f;
        if(canFade) {
            Color s = lr.startColor;
            Color e = lr.endColor;

            s.a = parentLR.startColor.a * startAlpha;
            e.a = parentLR.endColor.a * startAlpha;

            lr.startColor = s;
            lr.endColor = e;
        }
    }
}
