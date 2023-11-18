using UnityEngine;

namespace ProjectMultiplayer.Player.Actions {
    public class JumpGlide : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private float _jumpHeight;
        private Vector3 _jumpForce;

        [Space(16)]

        [SerializeField] private float _glideSpeed;
        private Vector3 _glideForce;
        private bool _isGliding = false;

        [Space(16)]

        [SerializeField] private Vector3 _checkGroundOffset;
        [SerializeField] private Vector3 _checkGroundBox;
        [SerializeField] private LayerMask _checkGroundLayer;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        private void Awake() {
            _jumpForce = Vector3.up * Mathf.Sqrt(2 * -Physics.gravity.y * _jumpHeight);
            _glideForce = Vector3.up * _glideSpeed;
        }

        private void FixedUpdate() {
            if (_isGliding && _player.NRigidbody.Rigidbody.velocity.y < 0) {
                _player.NRigidbody.Rigidbody.AddForce(_glideForce, ForceMode.Acceleration);
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} is gliding");
#endif
            }
        }

        public override void DoAction(Ray cameraRay) {
            if (IsGrounded()) {
                _player.NRigidbody.Rigidbody.AddForce(_jumpForce, ForceMode.VelocityChange);
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} has jumped");
#endif
            }
            _isGliding = true;
#if UNITY_EDITOR
            if (_debugLogs) Debug.Log($"{gameObject.name} started gliding");
#endif
        }

        public override void StopAction() {
            _isGliding = false;
#if UNITY_EDITOR
            if (_debugLogs) Debug.Log($"{gameObject.name} STOPED gliding anymore");
#endif
        }

        private bool IsGrounded() {
            return Physics.OverlapBox(transform.position + _checkGroundOffset, _checkGroundBox / 2, Quaternion.identity, _checkGroundLayer) != null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Gizmos.color = IsGrounded() ? Color.green : Color.red;
            Gizmos.DrawWireCube(transform.position + _checkGroundOffset, _checkGroundBox);
        }
#endif

    }
}
