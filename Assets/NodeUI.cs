using UnityEngine;

public class NodeUI : MonoBehaviour {
    public GameObject ui;

    private Node target;

    public void SetTarget(Node target_) {
        ui.SetActive(true);
        target = target_;
        transform.position = target.GetBuildPosition();
    }

    public void Hide() {
        ui.SetActive(false);
    }
}
