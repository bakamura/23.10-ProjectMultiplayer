using UnityEngine;

public class Breakable : MonoBehaviour {

    [Header("Cache")]

    private Size _size;

    private void Awake() {
        _size = GetComponent<Size>();
    }

    public void Break(Size.SizeClass size) {
        if (size >= _size.Class) gameObject.SetActive(false);
    }

}
