
using UnityEngine;

public class ClickEnvironmentToCancel : MonoBehaviour {

    private void OnMouseDown() {
        //check if node is pressed, check if node is occupied
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo)) {

            var n = hitInfo.collider.gameObject.GetComponent<Node>();
            if(n != null)
                if(n.turret)
                    return;

            BuildManager.instance.DeselectNode();
        }
    }
}
