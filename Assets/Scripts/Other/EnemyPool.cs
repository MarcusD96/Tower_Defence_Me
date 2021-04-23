﻿
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour {

    public static EnemyPool instance;
    public PooledEnemy[] enemies;

    private List<GameObject>[] pool;
    private Vector3 spawn;

    private void Awake() {
        instance = this;
        foreach(var s in FindObjectsOfType<Transform>()) {
            if(s.CompareTag("Respawn")) {
                spawn = s.position;
                break;
            }
        }
    }

    private void Start() {
        GameObject tmp;
        pool = new List<GameObject>[enemies.Length];

        for(int count = 0; count < enemies.Length; count++) {
            pool[count] = new List<GameObject>();
            for(int num = 0; num < enemies[count].amount; num++) {
                tmp = Instantiate(enemies[count].enemy, new Vector3(0.0f, 1000.0f, 0.0f), Quaternion.identity);
                tmp.SetActive(false);
                tmp.transform.parent = transform;
                pool[count].Add(tmp);
            }
        }
    }

    public GameObject Activate(EnemyType e, Vector3 pos, Quaternion rot) {
        for(int count = 0; count < pool[(int) e].Count; count++) {
            if(!pool[(int) e][count].activeSelf) {
                GameObject currObj = pool[(int) e][count];
                Transform currTrans = currObj.transform;

                currObj.SetActive(true);
                currTrans.position = pos;
                currTrans.rotation = rot;
                return currObj;
            }
        }
        GameObject newObj = Instantiate(enemies[(int) e].enemy);
        Transform newTrans = newObj.transform;
        newTrans.position = pos;
        newTrans.rotation = rot;
        newTrans.parent = transform;
        pool[(int) e].Add(newObj);
        return newObj;
    }

    public void Deactivate(GameObject obj) {
        obj.SetActive(false);
        obj.transform.position = spawn;
    }
}

[System.Serializable]
public class PooledEnemy {
    public GameObject enemy;
    public int amount;
}

public enum EnemyType {
    Simple = 0,
    Quick,
    Tank,
    Simple_Boss,
    Quick_Boss,
    Tank_Boss
}
