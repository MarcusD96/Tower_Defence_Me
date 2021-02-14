
using UnityEngine;

public class BeamTurret : Turret {

    protected LaserTurret laserTurret;
    protected TeslaTurret teslaTurret;

    [Header("Beam Turret")]
    public LineRenderer lineRenderer;

    public void Awake() {
        lineRenderer.enabled = false;
    }

    new void Update() {
        base.Update();
    }

    public virtual void AutoShoot() {
        Debug.Log("Beam Shoot!");
    }

    public virtual void ManualShoot() {
        Debug.Log("Beam Manual Shoot!");
    }

    public void LaserOff() {
        if(laserTurret) {
            AudioManager.StopSound(shootSound);
            lineRenderer.enabled = false;
            laserTurret.impactEffect.Stop();
            laserTurret.impactLight.enabled = false;
        }
    }
}
