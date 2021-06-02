
using System.Collections;
using UnityEngine;

public class Effect : MonoBehaviour {

    public float expireTime;

    void OnEnable() {
        StartCoroutine(Deactivate(expireTime));
    }

    IEnumerator Deactivate(float time) {
        if(time == 0)
            time = 1.0f;
        yield return new WaitForSeconds(time);
        ObjectPool.instance.Deactivate(gameObject);
    }
}
