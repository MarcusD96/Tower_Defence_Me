using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour {

    public static List<Transform> waypoints = new List<Transform>();

    void Awake() {
        waypoints.Capacity = transform.childCount;
        for(int i = 0; i < waypoints.Capacity; i++) {
            waypoints.Add(transform.GetChild(i));
        }
    }

}
