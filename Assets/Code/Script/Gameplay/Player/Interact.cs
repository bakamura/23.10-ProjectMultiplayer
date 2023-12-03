using UnityEngine;

using ProjectMultiplayer.ObjectCategory;
using Fusion;

namespace ProjectMultiplayer.Player.Actions {
    public class Interact : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private Vector3 _InteractOffset;
        [SerializeField] private Vector3 _InteractBox;
        [SerializeField] private LayerMask _actionLayer;
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

            IInteractable hitInteractable;
            foreach (Collider col in Physics.OverlapBox(transform.position + Quaternion.Euler(0, transform.rotation.y, 0) * _InteractOffset, _InteractBox, transform.rotation)) {
                if (col.transform != transform) {
                    hitInteractable = col.transform.GetComponent<IInteractable>();
                    if (hitInteractable != null) {
                        if (Runner.IsServer) Rpc_UpdateVisuals(true);
                        hitInteractable.Interact();
#if UNITY_EDITOR
                        if(_debugLogs) Debug.Log($"{transform.name} is interacting with {col.name}");
#endif
                    }
#if UNITY_EDITOR
                    else if (_debugLogs && hitInteractable == null) Debug.Log("Object without interactable been hit (Normal occurrence)");
#endif
                }
            }

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
