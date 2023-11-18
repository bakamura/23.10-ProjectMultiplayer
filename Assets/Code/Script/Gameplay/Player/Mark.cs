using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Recall;

namespace ProjectMultiplayer.Player.Actions {
    public class Mark : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private float _actionRange;
        [SerializeField] private LayerMask _actionLayer;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        public override void DoAction(Ray cameraRay) {
            if (Physics.Raycast(cameraRay, out RaycastHit hit, Mathf.Infinity, _actionLayer) && Vector3.Distance(transform.position, hit.point) < _actionRange) {
                hit.transform.GetComponent<Recallable>()?.Mark();

#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} is trying to mark {hit.transform.name}");
#endif
            }
#if UNITY_EDITOR
            else if(_debugLogs) Debug.Log($"{gameObject.name} is trying to mark but didn't hit anything");
#endif
        }

        public override void StopAction() { }

    }
}
