using Fusion;
using UnityEngine.InputSystem;

namespace Player.Actions {
    public abstract class PlayerAction : NetworkBehaviour {

        protected static Player _player;

        public abstract void DoAction(InputAction.CallbackContext input);

        public abstract void StopAction(InputAction.CallbackContext input);

    }
}
