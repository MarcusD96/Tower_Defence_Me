
using UnityEngine;

public class Rod : Projectile {

    private int penetration;
    Vector3 direction;

    void Awake() {
        rod = this;
        lifeEnd = Time.time + 1;
        direction = gameObject.transform.forward;
    }

    new void Update() {
        if(Time.time > lifeEnd) {
            HitTarget(true);
            return;
        }

        if(penetration <= 0) {
            Destroy(gameObject);
        }

        distanceThisFrame = speed * Time.deltaTime;

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    public void SetPenetration(int penetration_) { //giggiddy
        penetration = penetration_;
    }

    protected override void HitTarget(bool endOfLife) {
        GameObject effectInstance = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 3.0f);

        if(endOfLife) {
            Destroy(gameObject);
            return;
        }

        Damage(target);
        GameObject indicatorInstance = Instantiate(indicator, transform.position, Quaternion.identity);
        indicatorInstance.GetComponent<DamageIndicator>().damage = damage;
        Destroy(indicatorInstance, 0.5f);
    }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Enemy")) {
            target = other.transform;
            HitTarget(false);
            penetration--;
        }
    }
}
