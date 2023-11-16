using UnityEngine;

namespace ProjectMultiplayer.Player.Actions {
    public class Jump : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private float _jumpHeight;
        private Vector3 _jumpForce;

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
        }

        public override void DoAction(Ray cameraRay) {
            if (IsGrounded()) {
                _player.NRigidbody.Rigidbody.AddForce(_jumpForce, ForceMode.VelocityChange);
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} has jumped");
#endif
            }
#if UNITY_EDITOR
            else if (_debugLogs) Debug.Log($"{gameObject.name} tried jumping but is NOT in ground");
#endif
        }

        public override void StopAction() { }

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
