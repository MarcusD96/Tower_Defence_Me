
using UnityEngine;

public class WindShot : ParticleShot {
    [Header("WindShot")]
    int penetration;
    float damage, chanceToBlowBack, blowBackDuration;

    public void Initialize(int rangeLevel_, int level_B, float range_, float damage_, int penetration_, float chanceToBlowback_, float blowBackDuration_) {
        level = level_B;
        range = range_;
        damage = damage_;
        penetration = penetration_;
        chanceToBlowBack = chanceToBlowback_;
        blowBackDuration = blowBackDuration_;

        ps = GetComponent<ParticleSystem>();
        var vel = ps.velocityOverLifetime;

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 75 + (25 * rangeLevel_));
        curve.AddKey(1.0f, 0.0f);
        vel.z = new ParticleSystem.MinMaxCurve(1.0f, curve);

        ps.Play();
        CheckHits();
    }

    Enemy e;
    RaycastHit[] hits;
    void CheckHits() {
        Ray ray = new Ray(transform.position, transform.forward);

        hits = Physics.SphereCastAll(ray, 3, range, enemyLayer);

        foreach(var hit in hits) {
            if(penetration <= 0)
                break;
            e = hit.collider.GetComponent<Enemy>();
            e.TakeDamage(damage, Color.black);
            float r = Random.Range(0.0f, 1.0f);
            if(e.isBoss) {
                e.BlowBack(blowBackDuration, level);
                penetration--;
                return;
            }
            if(r <= chanceToBlowBack)
                e.BlowBack(blowBackDuration, level);

            penetration--;
        }
    }
}
