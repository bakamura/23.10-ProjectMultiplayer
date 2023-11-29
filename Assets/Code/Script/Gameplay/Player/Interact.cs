using UnityEngine;

using ProjectMultiplayer.ObjectCategory;

namespace ProjectMultiplayer.Player.Actions {
    public class Interact : PlayerAction {

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
                IInteractable temp = hit.transform.GetComponent<IInteractable>();
                if(temp != null)
                {
                    PlayAudio(_actionSuccess);
                    temp.Interact();
                }

#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} tried interacting with {hit.transform.name}");
#endif
                return;
            }
#if UNITY_EDITOR
            if (_debugLogs) Debug.Log("Interact did not hit any relevant colliders");
#endif
            PlayAudio(_actionFailed);
        }

        public override void StopAction() { }

    }
}
