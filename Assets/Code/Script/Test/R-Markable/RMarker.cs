using UnityEngine;

public class RMarker : MonoBehaviour {

    [Header("Cache")]

    private Camera _cam;

    void Start() {
        _cam = Camera.main;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
                RMarkable rObj = hit.transform.GetComponent<RMarkable>();
                if (rObj) {
                    rObj.Mark();
                    return;
                }
            }
            Debug.Log("Can't be Marked");
        }
    }
}
