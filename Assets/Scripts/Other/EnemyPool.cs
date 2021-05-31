
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour {

    public static EnemyPool instance;
    public PooledEnemy[] enemies;

    private List<GameObject>[] pool;
    private Vector3 spawn;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        pool = new List<GameObject>[enemies.Length];
        spawn = Path.waypoints[0].position;
        StartCoroutine(SpawnEnemies());
    }

    [HideInInspector]
    public bool finishedLoading = false;
    IEnumerator SpawnEnemies() {
        GameObject tmp;
        for(int count = 0; count < enemies.Length; count++) {
            var p = new GameObject(enemies[count].enemy.name);
            p.transform.SetParent(gameObject.transform);
            pool[count] = new List<GameObject>();
            for(int num = 0; num < enemies[count].amount; num += 5) {
                for(int i = 0; i < 5; i++) {
                    tmp = Instantiate(enemies[count].enemy, p.transform);
                    tmp.SetActive(false);
                    pool[count].Add(tmp);
                }
                yield return null;
            }
        }
        finishedLoading = true;
    }

    public List<GameObject>[] GetPooledEnemies() {
        return pool;
    }

    public GameObject Activate(EnemyType e, Vector3 pos, Quaternion rot) {
        for(int count = 0; count < pool[(int) e].Count; count++) {
            if(!pool[(int) e][count].activeSelf) {
                GameObject currEnemy = pool[(int) e][count];
                Transform currTrans = currEnemy.transform;

                currEnemy.SetActive(true);
                currTrans.position = pos;
                currTrans.rotation = rot;
                currEnemy.GetComponent<Enemy>().isDead = false;
                currEnemy.GetComponent<Enemy>().ResetEnemy();
                return currEnemy;
            }
        }
        //not enough stored, bring in new enemy type, store it for future use
        GameObject newEnemy = Instantiate(enemies[(int) e].enemy);
        Transform newTrans = newEnemy.transform;
        newTrans.position = pos;
        newTrans.rotation = rot;
        newTrans.parent = transform;
        pool[(int) e].Add(newEnemy);
        newEnemy.GetComponent<Enemy>().isDead = false;
        newEnemy.GetComponent<Enemy>().ResetEnemy();
        return newEnemy;
    }

    public void Deactivate(GameObject e) {
        e.SetActive(false);
        e.transform.position = spawn;
    }
}

[System.Serializable]
public class PooledEnemy {
    public GameObject enemy;
    public int amount;
}

public enum EnemyType {
    Simple,
    Quick,
    Tank,
    Simple_Boss,
    Quick_Boss,
    Tank_Boss
}
