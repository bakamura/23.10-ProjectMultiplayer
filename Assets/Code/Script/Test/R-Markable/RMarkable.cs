using UnityEngine;

public class RMarkable : MonoBehaviour {

    private static RMarkable _markCurrent;
    private static Vector3 _markPos;

    public void Mark() {
        if(_markCurrent != null) {

        }
        _markCurrent = this;
        _markPos = transform.position;
    }

    public static void Recall() {
        if (_markCurrent) {
            _markCurrent.transform.position = _markPos;
            _markCurrent = null;
        }
        else Debug.Log("No Marked Object");
    }

}
