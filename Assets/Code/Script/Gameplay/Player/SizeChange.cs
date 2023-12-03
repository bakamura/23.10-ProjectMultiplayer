using UnityEngine;
using Fusion;
using ProjectMultiplayer.ObjectCategory.Size;

namespace ProjectMultiplayer.Player.Actions {
    public class SizeChange : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private bool _isGrowing;
        [SerializeField] private Vector3 _sizeChangeOffset;
        [SerializeField] private Vector3 _sizeChangeBox;
        [SerializeField] private LayerMask _sizedObjectLayer;

        [Space(16)]

        [SerializeField] private AudioClip _actionSuccess;
        [SerializeField] private AudioClip _actionFailed;

        private PlayerAnimationHandler _handler;
        [SerializeField] private string _animationTrigger;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        private void Awake() {
            _handler = GetComponentInChildren<PlayerAnimationHandler>();
        }

        public override void DoAction() {
            _handler.SetTrigger(_animationTrigger);

            Size hitSize;
            foreach (Collider col in Physics.OverlapBox(transform.position + Quaternion.Euler(0, transform.rotation.y, 0) * _sizeChangeOffset, _sizeChangeBox, transform.rotation)) {
                if (col.transform != transform) {
                    hitSize = col.transform.GetComponent<Size>();
                    if (hitSize && hitSize.TriPhase) {
                        if (Runner.IsServer) Rpc_UpdateVisuals(true);
                        hitSize.ChangeSize(_isGrowing);
                    }
#if UNITY_EDITOR
                    else if (_debugLogs) {
                        if (!hitSize) Debug.Log("Object without size been hit (Normal occurrence)");
                        else Debug.Log("Object without triphase been hit (Normal occurrence)");
                    }
#endif
                }
            }


#if UNITY_EDITOR
            if (_debugLogs && Physics.OverlapBox(transform.position + Quaternion.Euler(0, transform.rotation.y, 0) * _sizeChangeOffset, _sizeChangeBox, transform.rotation).Length < 1) Debug.Log($"{gameObject.name}'s SizeChange did not hit any relevant colliders");
#endif
            if (Runner.IsServer) Rpc_UpdateVisuals(false);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void Rpc_UpdateVisuals(bool actionSuccess) {
            UpdateVisuals(actionSuccess);
        }

        private void UpdateVisuals(bool actionSuccess) {
            PlayAudio(actionSuccess ? _actionSuccess : _actionFailed);
        }

        public override void StopAction() { }

    }
}
