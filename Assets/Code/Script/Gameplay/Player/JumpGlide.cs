using UnityEngine;

namespace ProjectMultiplayer.Player.Actions {
    public class JumpGlide : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private float _jumpHeight;
        [SerializeField] private AudioClip _jumpSound;
        [SerializeField] private AudioClip _glideSound;
        private Vector3 _jumpForce;

        [Space(16)]

        [SerializeField] private float _glideSpeed;
        private Vector3 _glideForce;
        private bool _isGliding = false;

#if UNITY_EDITOR
        [Space(16)]

        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        private void Awake() {
            _jumpForce = Vector3.up * Mathf.Sqrt(2 * -Physics.gravity.y * _jumpHeight);
            _glideForce = Vector3.up * _glideSpeed;
        }

        private void FixedUpdate() {
            if (_isGliding && _player.NRigidbody.Rigidbody.velocity.y < 0) {
                _player.NRigidbody.Rigidbody.AddForce(_glideForce, ForceMode.VelocityChange);
                PlayAudio(_glideSound, false);
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} is gliding");
#endif
            }
        }

        public override void DoAction(Ray cameraRay) {
            if (_player.IsGrounded) {
                _player.NRigidbody.Rigidbody.AddForce(_jumpForce, ForceMode.VelocityChange);
                PlayAudio(_jumpSound);
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

    }
}
