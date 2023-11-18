using UnityEngine;

using ProjectMultiplayer.ObjectCategory;

namespace ProjectMultiplayer.Player.Actions {
    public class Interact : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private float _actionRange;
        [SerializeField] private LayerMask _actionLayer;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        public override void DoAction(Ray cameraRay) {
            if (Physics.Raycast(cameraRay, out RaycastHit hit, Mathf.Infinity, _actionLayer) && Vector3.Distance(transform.position, hit.point) < _actionRange) {
                hit.transform.GetComponent<IInteractable>()?.Interact();
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} tried interacting with {hit.transform.name}");
#endif
            }
        }

        public override void StopAction() { }

    }
}
