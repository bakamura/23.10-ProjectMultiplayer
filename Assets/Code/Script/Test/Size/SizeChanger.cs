using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeChanger : MonoBehaviour {

    [Header("Parameters")]

    [SerializeField] private bool _isGrowing;

    [Header("Cache")]

    private Camera _cam;

    private void Awake() {
        _cam = Camera.main;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)){
            hit.transform.GetComponent<Size>()?.TryChangeSize(_isGrowing);
        }
    }

}
