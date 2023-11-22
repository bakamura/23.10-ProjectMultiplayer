using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Recall;

namespace ProjectMultiplayer.Player.Actions
{
    public class Mark : PlayerAction
    {

        [Header("Parameters")]

        [SerializeField] private float _actionRange;
        [SerializeField] private LayerMask _actionLayer;
        [SerializeField] private AudioClip _actionSuccess;
        [SerializeField] private AudioClip _actionFailed;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        public override void DoAction(Ray cameraRay)
        {
            if (Physics.Raycast(cameraRay, out RaycastHit hit, Mathf.Infinity, _actionLayer) && Vector3.Distance(transform.position, hit.point) < _actionRange)
            {
                Recallable temp = hit.transform.GetComponent<Recallable>();
                if (temp)
                {
                    temp.Mark();
                    PlayAudio(_actionSuccess);
                }

#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} is trying to mark {hit.transform.name}");
#endif
                return;
            }
#if UNITY_EDITOR
            else if (_debugLogs) Debug.Log($"{gameObject.name} is trying to mark but didn't hit anything");
#endif
            PlayAudio(_actionFailed);
        }

        public override void StopAction() { }

    }
}
