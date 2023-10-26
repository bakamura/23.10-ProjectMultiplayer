using UnityEngine.InputSystem;

public class Recall : PlayerAction {

    public override void DoAction(InputAction.CallbackContext input) {
        Recallable.Recall();
    }

    public override void StopAction(InputAction.CallbackContext input) { }

}
