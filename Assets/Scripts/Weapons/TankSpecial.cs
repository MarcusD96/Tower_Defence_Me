
using System.Collections.Generic;
using UnityEngine;

public class TankSpecial : MonoBehaviour {

    public float speed;

    List<Transform> path;
    Transform target;
    GameObject owner;
    TankTurret ownerComp;
    private int wayPointIndex = 0;

    private void Awake() {
        path = new List<Transform>(Path.waypoints);
        path.Reverse();
        transform.position = path[0].position;
        target = path[0];
    }

    private void LateUpdate() {
        if(owner) {
            if(owner.activeSelf) {
                ownerComp.SetSpecial(true);
                ownerComp.StartSpecialTime();
                owner.SetActive(false);
            }
        }

        Vector3 direction = target.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        if(Vector3.Distance(transform.position, target.position) <= 0.5f) {
            GetNextWayPoint();
            transform.LookAt(target, Vector3.up);
        }
    }
    void GetNextWayPoint() {
        if(wayPointIndex >= path.Capacity - 1) {
            owner.SetActive(true);
            ownerComp.StartSpecialTime();
            Destroy(gameObject);
            return;
        }
        wayPointIndex++;
        target = path[wayPointIndex];
    }

    public void SetOwner(GameObject o, TankTurret t) {
        owner = o;
        ownerComp = t;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Enemy")) {
            Enemy e = other.gameObject.GetComponent<Enemy>();
            e.TakeDamage((e.startHp / 2.0f) + 1, Color.white, true);
        }
    }
}
