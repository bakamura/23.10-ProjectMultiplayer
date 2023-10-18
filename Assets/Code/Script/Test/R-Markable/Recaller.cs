using UnityEngine;

public class Recaller : MonoBehaviour {

    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse1)) RMarkable.Recall();
    }
}
