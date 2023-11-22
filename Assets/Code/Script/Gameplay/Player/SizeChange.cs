using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Size;

namespace ProjectMultiplayer.Player.Actions {
    public class SizeChange : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private bool _isGrowing;
        [SerializeField] private float _actionRange;
        [SerializeField] private LayerMask _sizedObjectLayer;
        [SerializeField] private AudioClip _actionSuccess;
        [SerializeField] private AudioClip _actionFailed;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        public override void DoAction(Ray cameraRay) {
            if (Physics.Raycast(cameraRay, out RaycastHit hit, Mathf.Infinity, _sizedObjectLayer) && Vector3.Distance(transform.position, hit.point) < _actionRange) {
                Size hitSize = hit.transform.GetComponent<Size>();
#if UNITY_EDITOR
            if (_debugLogs) Debug.Log($"Size change Cast, hitting { hit.transform.name }, that { (hitSize ? (hitSize.TriPhase ? "Is TriPhase" : "Is NOT TriPhase") : "Does NOT have Size") } ");
#endif
                if (hitSize && hitSize.TriPhase)
                {
                    PlayAudio(_actionSuccess);
                    hitSize.ChangeSize(_isGrowing);
                    return;
                }
            }
            PlayAudio(_actionFailed);
        }

        public override void StopAction() { }

    }
}
