using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Recall;
using Fusion;

namespace ProjectMultiplayer.Player.Actions {
    public class Recall : PlayerAction {

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

        public override void DoAction(Ray cameraRay) {
            _handler.SetTrigger(_animationTrigger);
            if (RecallMark.Instance.markCurrent) {
                Recallable.Recall();
                Rpc_UpdateVisuals(true);
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{RecallMark.Instance.markCurrent} was Asked to recall");
#endif
                return;
            }
#if UNITY_EDITOR
            else if(_debugLogs) Debug.Log("Recall did not hit any relevant colliders");
#endif
            Rpc_UpdateVisuals(true);
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

    }
}
