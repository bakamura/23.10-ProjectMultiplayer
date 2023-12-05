using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Recall;
using Fusion;
using System.Linq;

namespace ProjectMultiplayer.Player.Actions {
    public class Mark : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private Vector3 _MarkOffset;
        [SerializeField] private Vector3 _MarkBox;
        [SerializeField] private LayerMask _actionLayer;

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

            Recallable recallable;
            foreach (Collider col in Physics.OverlapBox(transform.position + Quaternion.Euler(0, transform.rotation.y, 0) * _MarkOffset, _MarkBox, transform.rotation).OrderBy(col => (transform.position + _MarkOffset - col.transform.position).sqrMagnitude).ToArray()) {
                if (col.transform != transform) {
                    recallable = col.GetComponent<Recallable>();
                    if (recallable) {
                        recallable.Mark();
                        if (Runner.IsServer) Rpc_UpdateVisuals(true);
#if UNITY_EDITOR
                        if (_debugLogs) Debug.Log($"{transform.name} is marking {recallable.name}");
#endif                      
                        return;
                    }
                }
            }

#if UNITY_EDITOR
            if (_debugLogs) Debug.Log($"{transform.name} is trying to mark but didn't hit anything");
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
