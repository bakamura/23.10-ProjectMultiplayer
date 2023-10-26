using UnityEngine;
using UnityEngine.InputSystem;

public class Mark : PlayerAction {

    [Header("Parameters")]

    [SerializeField] private float _actionRange;

    public override void DoAction(InputAction.CallbackContext input) {
        if (Physics.Raycast(_player.Camera.ScreenPointToRay(_player.ScreenSize), out RaycastHit hit) && Vector3.Distance(transform.position, hit.point) < _actionRange) hit.transform.GetComponent<Recallable>()?.Mark();
    }

    public override void StopAction(InputAction.CallbackContext input) { }

}
