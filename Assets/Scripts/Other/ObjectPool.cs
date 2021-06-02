
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

    public static ObjectPool instance;
    public PooledObject[] enemies, projectiles, effects;
    public Transform enemiesParent, projectilesParent, effectsParent;

    private List<GameObject>[] enemyPool, projectilesPool, effectsPool;
    private Vector3 spawn;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        enemyPool = new List<GameObject>[enemies.Length];
        projectilesPool = new List<GameObject>[projectiles.Length];
        effectsPool = new List<GameObject>[effects.Length];
        spawn = Path.waypoints[0].position;
        StartCoroutine(SpawnObjectsInPool(enemies, enemyPool, enemiesParent));
        StartCoroutine(SpawnObjectsInPool(projectiles, projectilesPool, projectilesParent));
        StartCoroutine(SpawnObjectsInPool(effects, effectsPool, effectsParent));
    }

    [HideInInspector]
    bool loadingEnemies = true, loadingProjectiles = true, loadingEffects = true;
    IEnumerator SpawnObjectsInPool(PooledObject[] objects, List<GameObject>[] pool, Transform parent) {        
        IEnumerator[] coroutines = new IEnumerator[objects.Length];
        for(int count = 0; count < objects.Length; count++) {
            GameObject p = null;
            coroutines[count] = SpawnObject(objects, pool, parent, count, p, coroutines);
            StartCoroutine(coroutines[count]);
        }
        if(objects == enemies) {
            bool notDone = true;
            while(notDone) {
                foreach(var c in coroutines) {
                    if(c != null) {
                        notDone = true;
                        break;
                    }
                    notDone = false;
                }
                yield return null;
            }
            loadingEnemies = false;
        } else if(objects == projectiles) {
            bool notDone = true;
            while(notDone) {
                foreach(var c in coroutines) {
                    if(c != null) {
                        notDone = true;
                        break;
                    }
                    notDone = false;
                }
                yield return null;
            }
            loadingProjectiles = false;
        } else {
            bool notDone = true;
            while(notDone) {
                foreach(var c in coroutines) {
                    if(c != null) {
                        notDone = true;
                        break;
                    }
                    notDone = false;
                }
                yield return null;
            }
            loadingEffects = false;
        }
    }

    IEnumerator SpawnObject(PooledObject[] objects, List<GameObject>[] pool, Transform parent, int count, GameObject parent_, IEnumerator[] corutines) {
        GameObject tmp;
        parent_ = new GameObject(objects[count].go.name);
        parent_.transform.SetParent(parent, true);
        pool[count] = new List<GameObject>();
        for(int num = 0; num < objects[count].amount; num += 5) {
            for(int i = 0; i < 5; i++) {
                tmp = Instantiate(objects[count].go, parent_.transform);
                tmp.SetActive(false);
                pool[count].Add(tmp);
            }
            yield return null;
        }
        corutines[count] = null;
    }

    public bool CheckLoading() {
        return !loadingEnemies && !loadingProjectiles && !loadingEffects;
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

    public GameObject ActivateEffect(EffectType e, Vector3 pos, Quaternion rot, float expireTime) {
        int t = (int) e;
        for(int i = 0; i < effectsPool[t].Count; i++) {
            if(!effectsPool[t][i].activeSelf) {
                GameObject currEff = effectsPool[t][i];
                Transform currTrans = currEff.transform;

                currEff.SetActive(true);
                currTrans.position = pos;
                currTrans.rotation = rot;
                if(!currEff.GetComponent<Effect>())
                    print(e);
                else
                    currEff.GetComponent<Effect>().expireTime = expireTime;
                return currEff;
            }
        }
        Debug.LogError("Not enough effects of type: " + e);
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

public enum EffectType {
    BulletImpact,
    RodImpact,
    TankShotImpact,
    MissileExplosion,
    SimpleDeath,
    QuickDeath,
    TankDeath,
    BossSimpleDeath,
    BossQuickDeath,
    BossTankDeath,
    SpecialActivated
}