
using System.Collections.Generic;
using UnityEngine;

public class IndicatorPool : MonoBehaviour {

    public static IndicatorPool instance;
    public PooledIndicator[] indicators;

    private List<GameObject>[] pool;
    private Vector3 spawn = Vector3.down * 2;

    private void Awake() {
        instance = this;
    }

    private void Start() {
    }

    public GameObject Activate(EnemyType e, Vector3 pos, Quaternion rot) {
        return new GameObject();
    }

    public void Deactivate(GameObject e) {

    }
}

[System.Serializable]
public class PooledIndicator {
    public GameObject indicator;
    public int amount;
}
