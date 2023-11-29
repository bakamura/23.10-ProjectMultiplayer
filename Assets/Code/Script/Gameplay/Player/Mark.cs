using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Recall;
using Fusion;

namespace ProjectMultiplayer.Player.Actions {
    public class Mark : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private float _actionRange;
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

        public override void DoAction(Ray cameraRay) {
            _handler.SetTrigger(_animationTrigger);
            if (Physics.Raycast(cameraRay, out RaycastHit hit, Mathf.Infinity, _actionLayer) && Vector3.Distance(transform.position, hit.point) < _actionRange) {
                Recallable temp = hit.transform.GetComponent<Recallable>();
                if (temp) {
                    temp.Mark();
                    Rpc_UpdateVisuals(true);
                }

#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} is trying to mark {hit.transform.name}");
#endif
                return;
            }
#if UNITY_EDITOR
            else if (_debugLogs) Debug.Log($"{gameObject.name} is trying to mark but didn't hit anything");
#endif
            Rpc_UpdateVisuals(false);
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
