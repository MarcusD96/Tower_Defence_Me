
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleShot : MonoBehaviour {
    [Header("Particle Shot")]
    public LayerMask enemyLayer;
    public WeaponType weapon;

    protected float range;
    protected int level;
    protected bool special;
    protected ParticleSystem ps;

    void LateUpdate() {
        if(ps.isStopped) {
            ObjectPool.instance.Deactivate(gameObject);
        }
    }
}
