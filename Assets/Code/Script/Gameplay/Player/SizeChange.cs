using UnityEngine;
using Fusion;
using ProjectMultiplayer.ObjectCategory.Size;

namespace ProjectMultiplayer.Player.Actions {
    public class SizeChange : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private bool _isGrowing;
        [SerializeField] private float _actionRange;
        [SerializeField] private LayerMask _sizedObjectLayer;
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
            if (Physics.Raycast(cameraRay, out RaycastHit hit, Mathf.Infinity, _sizedObjectLayer) && Vector3.Distance(transform.position, hit.point) < _actionRange) {
                Size hitSize = hit.transform.GetComponent<Size>();
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"Size change Cast, hitting {hit.transform.name}, that {(hitSize ? (hitSize.TriPhase ? "Is TriPhase" : "Is NOT TriPhase") : "Does NOT have Size")} ");
#endif 
                if (hitSize && hitSize.TriPhase) {
                    if (Runner.IsServer) Rpc_UpdateVisuals(true);
                    hitSize.ChangeSize(_isGrowing);
                    return;
                }
            }
#if UNITY_EDITOR
            else if (_debugLogs) Debug.Log($"{gameObject.name}'s SizeChange did not hit any relevant colliders");
#endif
            if (Runner.IsServer) Rpc_UpdateVisuals(false);
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
