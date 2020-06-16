using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour {

    public Color hoverColor, errorColor;
    public Vector3 offset;

    [Header("Optional")]
    public GameObject turret = null;

    private Renderer rend;
    private Color startColor;
    private BuildManager buildManager;

    void Start() {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        buildManager = BuildManager.instance;
    }

    void OnMouseDown() {
        if(EventSystem.current.IsPointerOverGameObject())
            return;

        if(!buildManager.CanBuild)
            return;

        if(turret != null) {
            Debug.Log("Turret already here - TODO: Display on screen");
            return;
        }

        buildManager.BuildTurretOn(this);
    }

    void OnMouseEnter() {
        if(EventSystem.current.IsPointerOverGameObject())
            return;

        if(!buildManager.CanBuild)
            return;

        if(turret) { 
            rend.material.color = errorColor;
            return;
        }


        if(buildManager.HasMoney) {
            rend.material.color = hoverColor;
        } else {
            rend.material.color = errorColor;
        }
    }

    void OnMouseExit() {
        rend.material.color = startColor;
    }

    public Vector3 GetBuildPosition() {
        return transform.position + offset;
    }

}
