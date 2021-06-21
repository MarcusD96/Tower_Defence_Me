
using UnityEngine;

public class WindWave : MonoBehaviour {

    private float range, damage, blowBackDuration;
    private LayerMask enemyLayer;

    public void Initialize(float range_, float damage_, float blowBackDuration_, LayerMask enemyLayer_) {
        range = range_;
        transform.localScale = new Vector3(range_, range_, 1);
        damage = damage_;
        blowBackDuration = blowBackDuration_;
        enemyLayer = enemyLayer_;
        CheckHits();
        Destroy(gameObject, 1.0f);
    }

    Enemy enemyComp;
    void CheckHits() {
        Collider[] colliders;
        colliders = Physics.OverlapSphere(transform.position, range, enemyLayer);

        foreach(Collider collider in colliders) {
            enemyComp = collider.GetComponent<Enemy>();
            enemyComp.BlowBack(blowBackDuration, 4);
            enemyComp.TakeDamage(damage, Color.black);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
