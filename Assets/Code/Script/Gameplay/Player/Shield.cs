using UnityEngine;
using UnityEngine.Events;

namespace ProjectMultiplayer.Player.Actions {
    public class Shield : PlayerAction {

        private bool _isShielded = false;
        public bool IsShielded { get { return _isShielded; } }
        public UnityEvent onBlockBullet = new UnityEvent();

        public override void DoAction(Ray cameraRay) {
            _isShielded = true;
        }

        public override void StopAction() {
            _isShielded = false;
        }

    }
}
