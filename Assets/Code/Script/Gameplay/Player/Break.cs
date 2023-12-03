using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Break;
using Fusion;

namespace ProjectMultiplayer.Player.Actions {
    public class Break : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private Vector3 _actionOffset;
        [SerializeField] private Vector3 _actionBox;
        [SerializeField] private AudioClip _breakSuccess;// index 0
        [SerializeField] private AudioClip _breakFailed;// index 1
        [SerializeField] private AudioClip _pushPlayer;// index 2

        [Space(16)]

        [SerializeField] private float _friendPushForce;

        private PlayerAnimationHandler _handler;
        [SerializeField] private string _animationTrigger;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        private void Awake() {
            _handler = GetComponentInChildren<PlayerAnimationHandler>();
        }

        public override void DoAction(Ray cameraRay) {
            _handler.SetTrigger(_animationTrigger);
            foreach (Collider collider in Physics.OverlapBox(transform.position + Quaternion.Euler(0, transform.rotation.y, 0) * _actionOffset, _actionBox / 2)) {
                Breakable breakScript = collider.GetComponent<Breakable>();
                if (breakScript && breakScript.TryBreak(_player.Size.Type))
                {
                    if (Runner.IsServer) Rpc_UpdateVisuals(0);
                }
                Player playerScript = collider.GetComponent<Player>();
                if (playerScript)
                {
                    playerScript.NRigidbody.Rigidbody.AddForce((collider.transform.position - transform.position).normalized * _friendPushForce, ForceMode.VelocityChange); ;
                    if (Runner.IsServer) Rpc_UpdateVisuals(2);
                }
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{collider.name} was Asked to break");
#endif
                return;
            }
#if UNITY_EDITOR
            if (_debugLogs) Debug.Log("Break did not hit any relevant colliders");
#endif
            if (Runner.IsServer) Rpc_UpdateVisuals(1);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void Rpc_UpdateVisuals(byte audioType)
        {
            UpdateVisuals(audioType);
        }

        private void UpdateVisuals(byte audioType)
        {
            switch (audioType)
            {
                case 0:
                    PlayAudio(_breakSuccess);
                    break;
                case 1:
                    PlayAudio(_breakFailed);
                    break;
                case 2:
                    PlayAudio(_pushPlayer);
                    break;
                default:
                    break;
            }
        }

        public override void StopAction() { }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Matrix4x4 prevMatrix = Gizmos.matrix;
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(_actionOffset, _actionBox);
            Gizmos.matrix = prevMatrix;
        }
#endif
    }
}