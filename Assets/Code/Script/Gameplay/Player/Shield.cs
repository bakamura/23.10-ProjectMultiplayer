using UnityEngine;
using UnityEngine.Events;

namespace ProjectMultiplayer.Player.Actions {
    public class Shield : PlayerAction {

        private bool _isShielded = false;
        public bool IsShielded { get { return _isShielded; } }
        public UnityEvent onBlockBullet = new UnityEvent();

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        public override void DoAction(Ray cameraRay) {
            _isShielded = true;
#if UNITY_EDITOR
            if (_debugLogs) Debug.Log($"{ gameObject.name } is now shielded");
#endif
        }

        public override void StopAction() {
            _isShielded = false;
#if UNITY_EDITOR
            if (_debugLogs) Debug.Log($"{gameObject.name} is now NOT shielded");
#endif
        }

    }
}
