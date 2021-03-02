
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickEnvironmentCancelTurret : MonoBehaviour {

    private void OnMouseDown() {
        if(EventSystem.current.IsPointerOverGameObject())
            return;

        //check if node is pressed, check if node is occupied
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo)) {
            var n = hitInfo.collider.gameObject.GetComponent<Node>();
            if(n != null)
                if(n.turret)
                    return;
        }

        BuildManager.instance.ToggleBuildSelection(false);
        BuildManager.instance.SelectTurretToBuild(null);
    }
}
