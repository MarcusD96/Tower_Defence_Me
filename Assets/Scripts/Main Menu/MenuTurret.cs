
using System.Collections;
using UnityEngine;

public class MenuTurret : MonoBehaviour {
    public bool hasNoAction;

    [Header("Menu Turret")]
    public GameObject weapon, shootEffect;
    public string weaponSound;
    public Transform fireSpawn;
    public int maxShootNum;
    public Animator shootAnim, secondaryShootAnim;

    [Header("Descriptors")]
    public string turretName;
    public string description, upgradeAName, upgradeA, upgradeBName, upgradeB, specName, upgradeSpec;

    int shootNum;
    float nextFire;

    private void Awake() {
        nextFire = Random.Range(4, 8);
    }

    private void Update() {
        if(hasNoAction)
            return;
        if(Time.time >= nextFire) {
            StartCoroutine(Shoot());
            nextFire = Time.time + Random.Range(4, 8);
        }
    }

    IEnumerator Shoot() {
        yield return new WaitForSeconds(1.0f);

        shootNum = Random.Range(1, maxShootNum + 1);

        for(int i = 0; i < shootNum; i++) {
            if(shootAnim) {
                shootAnim.SetTrigger("Shoot"); 
            }

            if(secondaryShootAnim)
                secondaryShootAnim.SetTrigger("Shoot");

            if(shootEffect) {
                var gg = Instantiate(shootEffect, fireSpawn);
                Destroy(gg, 0.5f);
            }

            var g = Instantiate(weapon, fireSpawn);
            Destroy(g, 4.0f);

            AudioManager.StaticPlayEffect(AudioManager.instance.sounds, weaponSound, fireSpawn.position);

            yield return new WaitForSeconds(1.0f / shootNum);
        }
    }
}
