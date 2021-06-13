using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour {

    public List<Transform> waypoints;
    //[HideInInspector]
    public float pathLength;

    void Awake() {
        waypoints = new List<Transform>();

        waypoints.Capacity = transform.childCount;
        for(int i = 0; i < waypoints.Capacity; i++) {
            waypoints.Add(transform.GetChild(i));
        }
        CalculatePathLength();
    }

    void CalculatePathLength() {
        pathLength = 0;
        for(int i = 2; i < waypoints.Count; i++) { //start at third waypoint...(dont include hidden/first visible waypoint)
            pathLength += Vector3.Distance(waypoints[i].position, waypoints[i - 1].position);
        }
    }

}
