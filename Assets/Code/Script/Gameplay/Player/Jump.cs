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

        private void Awake() {
            _jumpForce = Vector3.up * Mathf.Sqrt(2 * -Physics.gravity.y * _jumpHeight);
        }

        public override void DoAction(Ray cameraRay) {
            if (IsGrounded()) _player.Rigidbody.AddForce(_jumpForce, ForceMode.VelocityChange);
        }

        public override void StopAction() { }

        private bool IsGrounded() {
            return Physics.OverlapBox(transform.position + _checkGroundOffset, _checkGroundBox, Quaternion.identity, _checkGroundLayer) != null;
        }

    }
}
