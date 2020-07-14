using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour {

    public static List<Transform> waypoints;

    void Awake() {
        waypoints = new List<Transform>();

        waypoints.Capacity = transform.childCount;
        for(int i = 0; i < waypoints.Capacity; i++) {
            waypoints.Add(transform.GetChild(i));
        }
    }

}
