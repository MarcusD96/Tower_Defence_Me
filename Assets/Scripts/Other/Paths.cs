
using System.Collections.Generic;
using UnityEngine;

public class Paths : MonoBehaviour {

    public static List<Path> paths;

    private void Awake() {
        paths = new List<Path>();

        paths.Capacity = transform.childCount;
        for(int i = 0; i < paths.Capacity; i++) {
            paths.Add(transform.GetChild(i).GetComponent<Path>());
        }
    }

    public static List<Transform> GetPathWaypoints(int index) {
        return paths[index].waypoints;
    }

    public static Vector3 GetFirstPathPosition() {
        return paths[0].waypoints[0].position;
    }

    public static float GetPathLength(int index) {
        return paths[index].pathLength;
    }
}
