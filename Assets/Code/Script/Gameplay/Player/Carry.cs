using System.Linq;
using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Size;
using Fusion;

namespace ProjectMultiplayer.Player.Actions {
    public class Carry : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private Vector3 _liftOffset;
        [SerializeField] private Vector3 _liftBox;
        [SerializeField] private AudioClip _actionSuccess;
        [SerializeField] private AudioClip _actionFailed;

        private Transform _carriedObject;

        private PlayerAnimationHandler _handler;
        [SerializeField] private string _animationBool;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        private void Awake() {
            _handler = GetComponentInChildren<PlayerAnimationHandler>();
        }

        public override void DoAction() {
            if (!_carriedObject) {
                Size sizeCache;
                foreach (Collider col in Physics.OverlapBox(transform.position + Quaternion.Euler(0, transform.rotation.y, 0) * _liftOffset, _liftBox, transform.rotation).OrderBy(col => (transform.position + _liftOffset - col.transform.position).sqrMagnitude).ToArray()) {
                    if (col.transform != transform) {
                        sizeCache = col.GetComponent<Size>();
                        if (sizeCache) {
                            _handler.SetBool(_animationBool, true);
                            _carriedObject = sizeCache.transform;
                            _carriedObject.transform.parent = transform;
                            _carriedObject.transform.localPosition = _liftOffset; // Test Out, Maybe create empty object
                            if (Runner.IsServer) Rpc_UpdateVisuals(true);
#if UNITY_EDITOR
                            if (_debugLogs) Debug.Log($"{_carriedObject.name} is now being carried by {gameObject.name}");
#endif
                            return;
                        }
                    }
                }
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log("Carry did not hit any relevant colliders");
#endif
                if (Runner.IsServer) Rpc_UpdateVisuals(false);
            }
            else {
                _handler.SetBool(_animationBool, false);
                _carriedObject.transform.parent = null;
                _carriedObject = null;
                if (Runner.IsServer) Rpc_UpdateVisuals(true);
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{_carriedObject.name} STOPED being carried by {gameObject.name}");
#endif
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void Rpc_UpdateVisuals(bool actionSuccess)
        {
            UpdateVisuals(actionSuccess);
        }

        private void UpdateVisuals(bool actionSuccess)
        {
            PlayAudio(actionSuccess ? _actionSuccess : _actionFailed);
        }

        public override void StopAction() { }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Matrix4x4 prevMatrix = Gizmos.matrix;
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(_liftOffset, _liftBox);
            Gizmos.matrix = prevMatrix;
        }
#endif
    }
}
