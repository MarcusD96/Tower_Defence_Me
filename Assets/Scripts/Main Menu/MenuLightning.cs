
using System.Collections;
using UnityEngine;

public class MenuLightning : MonoBehaviour {

    public LineRenderer lr;
    public int numArcs;

    private void Awake() {
        Destroy(gameObject, 2.0f);
        BuildLightning();
        StartCoroutine(Fade());
    }

    Vector3 target = new Vector3(5.57f, -4.17f, 1.09f);
    void BuildLightning() {
        var lastPoint = Vector3.zero;
        var lineVert = 1;

        lr.SetPosition(0, lastPoint);    //make the origin of the lineRenderer the same as the transform

        for(int i = 0; i < numArcs; i++) {    //was the last arc not close to the target
            lr.positionCount = lineVert + 1;     //new vertex
            var fwd = target - lastPoint;    //gives the direction to our target from the end of the last arc
            fwd.Normalize();
            fwd = RandomizeArc(fwd, 2);   //we don't want a straight line to the target though
            fwd *= Random.Range(0.5f, 2);     //nature is never too uniform
            fwd += lastPoint;   //point + distance * direction = new point. this is where our new arc ends
            if(fwd.y >= 6) { //point must be above the ground
                fwd.y = 5.8f;
            }
            lr.SetPosition(lineVert, fwd);
            lineVert++;
            lastPoint = fwd;    //so we know where we are starting from for the next arc
        }

        lr.SetPosition(lr.positionCount - 1, target); //last point is always the target
    }

    Vector3 RandomizeArc(Vector3 v, float inaccuracy_) {
        v += new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f) * inaccuracy_);
        v.Normalize();
        return v;
    }

    IEnumerator Fade() {
        while(lr.startColor.a > 0) {
            Color start = lr.startColor;
            Color end = lr.endColor;

            start.a = Mathf.Lerp(start.a, -1, Time.deltaTime);
            end.a = Mathf.Lerp(end.a, -1, Time.deltaTime);

            lr.startColor = start;
            lr.endColor = end;
            yield return null;
        }
    }

}
