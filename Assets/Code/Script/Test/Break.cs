using UnityEngine;

public class Break : MonoBehaviour {

    [Header("Cache")]

    private Size _size;

    private void Awake() {
        _size = GetComponent<Size>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.E)) {
            Collider[] hits = Physics.OverlapBox(transform.position + transform.forward, Vector3.one);
            foreach (Collider hit in hits) hit.GetComponent<Breakable>()?.Break(_size.Class);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position + transform.forward, Vector3.one);
    }
#endif

}
