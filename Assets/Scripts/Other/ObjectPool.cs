
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

    public static ObjectPool instance;
    public PooledObject[] enemies, projectiles;
    public Transform enemiesParent, projectilesParent;

    private List<GameObject>[] enemyPool, projectilesPool;
    private Vector3 spawn;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        enemyPool = new List<GameObject>[enemies.Length];
        projectilesPool = new List<GameObject>[projectiles.Length];
        spawn = Path.waypoints[0].position;
        StartCoroutine(SpawnObjects(enemies, enemyPool));
        StartCoroutine(SpawnObjects(projectiles, projectilesPool));
    }

    [HideInInspector]
    bool loadingEnemies = true, loadingProjectiles = true;
    IEnumerator SpawnObjects(PooledObject[] objects, List<GameObject>[] pool) {
        GameObject tmp;
        for(int count = 0; count < objects.Length; count++) {
            var p = new GameObject(objects[count].go.name);
            if(objects == enemies) {
                p.transform.SetParent(enemiesParent, true);
            } else
                p.transform.SetParent(projectilesParent, true);
            pool[count] = new List<GameObject>();
            for(int num = 0; num < objects[count].amount; num += 5) {
                for(int i = 0; i < 5; i++) {
                    tmp = Instantiate(objects[count].go, p.transform);
                    tmp.SetActive(false);
                    pool[count].Add(tmp);
                }
                yield return null;
            }
        }
        if(objects == enemies)
            loadingEnemies = false;
        else
            loadingProjectiles = false;
    }

    public bool CheckLoading() {
        return !loadingEnemies && !loadingProjectiles;
    }

    public List<GameObject>[] GetPooledEnemies() {
        return enemyPool;
    }

    public List<GameObject>[] GetPooledProjectiles() {
        return projectilesPool;
    }

    public GameObject ActivateEnemy(EnemyType e, Vector3 pos, Quaternion rot) {
        int t = (int) e;
        for(int i = 0; i < enemyPool[t].Count; i++) {
            if(!enemyPool[t][i].activeSelf) {
                GameObject currEnemy = enemyPool[t][i];
                Transform currTrans = currEnemy.transform;

                currEnemy.SetActive(true);
                currTrans.position = pos;
                currTrans.rotation = rot;
                Enemy enemyComp = currEnemy.GetComponent<Enemy>();
                enemyComp.isDead = false;
                enemyComp.ResetEnemy();
                return currEnemy;
            }
        }

        Debug.LogError("Not enough enemies of type: " + e);
        return null;
    }

    public GameObject ActivateProjectile(ProjectileType p, Vector3 pos, Quaternion rot) {
        int t = (int) p;
        for(int i = 0; i < projectilesPool[t].Count; i++) {
            if(!projectilesPool[t][i].activeSelf) {
                GameObject currProj = projectilesPool[t][i];
                Transform currTrans = currProj.transform;

                currProj.SetActive(true);
                currTrans.position = pos;
                currTrans.rotation = rot;
                return currProj;
            }
        }
        Debug.LogError("Not enough projectiles of type: " + p);
        return null;
    }

    public void Deactivate(GameObject e) {
        e.SetActive(false);
        e.transform.position = spawn;
    }
}

[System.Serializable]
public class PooledObject {
    public string name;
    public GameObject go;
    public int amount;
}

public enum ProjectileType {
    Bullet,
    FireShot,
    Missile,
    Rod,
    TankShot,
    Bullet_Special,
    Missile_Special,
    Rod_Special
}

public enum EnemyType {
    Simple,
    Quick,
    Tank,
    Simple_Boss,
    Quick_Boss,
    Tank_Boss
}
