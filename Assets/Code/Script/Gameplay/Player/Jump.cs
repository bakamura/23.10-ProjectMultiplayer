using Fusion;
using UnityEngine;

namespace ProjectMultiplayer.Player.Actions {
    public class Jump : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private float _jumpHeight;
        private Vector3 _jumpForce;
        [SerializeField] private AudioClip _soundEffect;

#if UNITY_EDITOR
        [Space(16)]

        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        private void Awake() {
            _jumpForce = Vector3.up * Mathf.Sqrt(2 * -Physics.gravity.y * _jumpHeight);
        }

        public override void DoAction(Ray cameraRay) {
            if (_player.IsGrounded) {
                _player.NRigidbody.Rigidbody.AddForce(_jumpForce, ForceMode.VelocityChange);
                if (Runner.IsServer) Rpc_UpdateVisuals();
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} has jumped");
#endif
            }
#if UNITY_EDITOR
            else if (_debugLogs) Debug.Log($"{gameObject.name} tried jumping but is NOT in ground");
#endif
        }
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void Rpc_UpdateVisuals()
        {
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if(_soundEffect) PlayAudio(_soundEffect);
        }


        public override void StopAction() { }

    }
}
