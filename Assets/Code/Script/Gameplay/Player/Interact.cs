using UnityEngine;

using ProjectMultiplayer.ObjectCategory;

namespace ProjectMultiplayer.Player.Actions {
    public class Interact : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private float _actionRange;
        [SerializeField] private LayerMask _actionLayer;
        [SerializeField] private AudioClip _actionSuccess;
        [SerializeField] private AudioClip _actionFailed;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        public override void DoAction(Ray cameraRay) {
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
            PlayAudio(_actionFailed);
        }

        public override void StopAction() { }

    }
}
