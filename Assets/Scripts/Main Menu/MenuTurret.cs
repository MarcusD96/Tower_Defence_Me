
using System.Collections;
using UnityEngine;

public class MenuTurret : MonoBehaviour {

    public GameObject weapon;
    public string weaponSound;
    public Transform fireSpawn;
    public int shootInterval, maxShootNum;
    int shootNum;

    float nextFire;

    private void Awake() {
        nextFire = shootInterval;
    }

    private void Update() {
        if(Time.time >= nextFire) {
            StartCoroutine(Shoot());
            nextFire = Time.time + shootInterval;
        }
    }

    IEnumerator Shoot() {
        yield return new WaitForSeconds(1.0f);
        shootNum = Random.Range(1, maxShootNum + 1);
        for(int i = 0; i < shootNum; i++) {
            var g = Instantiate(weapon, fireSpawn.position, Quaternion.identity);
            AudioManager.StaticPlayEffect(AudioManager.instance.sounds, weaponSound, fireSpawn.position);
            Destroy(g, 4.0f);
            yield return new WaitForSeconds(1.0f / shootNum);
        }
    }
}
