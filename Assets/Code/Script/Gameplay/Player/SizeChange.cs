using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Actions {
    public class SizeChange : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private bool _isGrowing;
        [SerializeField] private float _actionRange;

        public override void DoAction(InputAction.CallbackContext input) {
            if (Physics.Raycast(_player.Camera.ScreenPointToRay(_player.ScreenSize), out RaycastHit hit) && Vector3.Distance(transform.position, hit.point) < _actionRange) {
                Size hitSize = hit.transform.GetComponent<Size>();
                if (hitSize && hitSize.TriPhase) hitSize.ChangeSize(_isGrowing);
            }
        }

        public override void StopAction(InputAction.CallbackContext input) { }

    }
}
