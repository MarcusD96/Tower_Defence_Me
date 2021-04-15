
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FireShot : MonoBehaviour {

    ParticleSystem fire;
    float radius, damage;
    int penetration;

    private void Start() {
        fire = GetComponent<ParticleSystem>();
        fire.Play();

        ParticleSystem.MainModule main = fire.main;
        main.startSpeed = (radius * 2) + 4;

        CheckHits();
    }

    void LateUpdate() {
        if(fire.isStopped) {
            Destroy(gameObject);
        }
    }

    public List<Collider> hits;
    void CheckHits() {
        hits = new List<Collider>(Physics.OverlapSphere(transform.position, radius));

        if(hits.Count > 0) {
            //trim out non enemies
            for(int i = hits.Count - 1; i >= 0; i--) {
                if(!hits[i].CompareTag("Enemy")) {
                    hits.RemoveAt(i);
                }
            }

            //sort enemies by first spawned
            hits.Sort((a, b) => {
                return b.gameObject.GetComponent<Enemy>().distanceTravelled.CompareTo(a.gameObject.GetComponent<Enemy>().distanceTravelled);
            });

            //damage remaining enemies
            foreach(var h in hits) {
                if(h == null)
                    continue;

                if(penetration == 0)
                    break;

                h.gameObject.GetComponent<Enemy>().TakeDamage(damage, true);
                penetration--;
            }
        }
    }

    public void SetStats(float damage_, float radius_, int penetration_) {
        damage = damage_;
        radius = radius_;
        penetration = penetration_;
    }
}
