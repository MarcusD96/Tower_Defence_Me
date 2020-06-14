using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour {

    public Color hoverColor;
    public Vector3 offset;

    private GameObject turret = null;
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

        if(!buildManager.GetTurretToBuild())
            return;

        if(turret) {
            Debug.Log("Turret already here - TODO: Display on screen");
        }

        GameObject turretToBuild = buildManager.GetTurretToBuild();
        turret = Instantiate(turretToBuild, transform.position + offset, transform.rotation);
    }

    void OnMouseEnter() {
        if(EventSystem.current.IsPointerOverGameObject())
            return;

        if(!buildManager.GetTurretToBuild())
            return;

        rend.material.color = hoverColor;
    }

    void OnMouseExit() {
        rend.material.color = startColor;
    }

}
