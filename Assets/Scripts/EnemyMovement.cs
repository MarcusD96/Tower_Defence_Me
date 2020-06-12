using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    public float speed = 10.0f;

    private Transform target;
    private int wayPointIndex = 0;

    // Start is called before the first frame update
    void Start() {
        target = Path.waypoints[0];
    }

    // Update is called once per frame
    void Update() {
        Vector3 direction = target.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        if(Vector3.Distance(transform.position, target.position) <= 0.4f) {
            GetNextWayPoint();
        }
    }

    void GetNextWayPoint() {
        if(wayPointIndex >= Path.waypoints.Capacity - 1) {
            Destroy(gameObject);
            return;
        }
        wayPointIndex++;
        target = Path.waypoints[wayPointIndex];
    }
}
