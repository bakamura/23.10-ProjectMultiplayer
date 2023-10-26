using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Shield : PlayerAction {

    private bool _isShielded = false;
    public bool IsShielded { get { return _isShielded; } }
    public UnityEvent onBlockBullet = new UnityEvent();

    public override void DoAction(InputAction.CallbackContext input) {
        _isShielded = true;
    }

    public override void StopAction(InputAction.CallbackContext input) {
        _isShielded = false;
    }

}
