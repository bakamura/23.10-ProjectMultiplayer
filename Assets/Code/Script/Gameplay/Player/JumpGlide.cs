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

        private void Awake() {
            _jumpForce = Vector3.up * Mathf.Sqrt(2 * -Physics.gravity.y * _jumpHeight);
            _glideForce = Vector3.up * _glideSpeed;
        }

        private void FixedUpdate() {
            if (_isGliding && _player.NRigidbody.Rigidbody.velocity.y < 0) _player.NRigidbody.Rigidbody.AddForce(_glideForce, ForceMode.Acceleration);
        }

        public override void DoAction(Ray cameraRay) {
            if (IsGrounded()) {
                _player.NRigidbody.Rigidbody.AddForce(_jumpForce, ForceMode.VelocityChange);
            }
            _isGliding = true;
        }

        public override void StopAction() {
            _isGliding = false;
        }

        private bool IsGrounded() {
            return Physics.OverlapBox(transform.position + _checkGroundOffset, _checkGroundBox, Quaternion.identity, _checkGroundLayer) != null;
        }

    }
}
