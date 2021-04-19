
using UnityEngine;
using UnityEngine.UI;

public class AimIndicator : MonoBehaviour {

    Turret turret;
    public Image image;

    private void LateUpdate() {
        if(!turret)
            return;

        if(turret.manual)
            image.gameObject.SetActive(true);
        else
            image.gameObject.SetActive(false);
    }

    public void SetTurret(Turret t) {
        turret = t;
        SetPositionAtRange();
    }

    public void SetPositionAtRange() { //z and x
        var pos = transform.position;
        var posY = transform.position.y;
        pos = turret.pivot.position + (turret.pivot.forward.normalized * turret.range * turret.manualRangeMultiplier);
        pos.y = posY;

        transform.position = pos;
    }

}
