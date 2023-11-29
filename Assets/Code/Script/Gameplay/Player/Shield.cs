using UnityEngine;
using UnityEngine.Events;

namespace ProjectMultiplayer.Player.Actions {
    public class Shield : PlayerAction {

        private bool _isShielded = false;
        public bool IsShielded { get { return _isShielded; } }
        public UnityEvent onBlockBullet = new UnityEvent();

        private PlayerAnimationHandler _handler;
        [SerializeField] private string _animationBool;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        private void Awake() {
            _handler = GetComponentInChildren<PlayerAnimationHandler>();
        }

        public override void DoAction(Ray cameraRay) {
            _handler.SetBool(_animationBool, true);
            _isShielded = true;
#if UNITY_EDITOR
            if (_debugLogs) Debug.Log($"{ gameObject.name } is now shielded");
#endif
        }

        public override void StopAction() {
            _handler.SetBool(_animationBool, false);
            _isShielded = false;
#if UNITY_EDITOR
            if (_debugLogs) Debug.Log($"{gameObject.name} is now NOT shielded");
#endif
        }

    }
}
