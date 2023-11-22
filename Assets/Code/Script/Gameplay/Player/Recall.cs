using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Recall;

namespace ProjectMultiplayer.Player.Actions {
    public class Recall : PlayerAction {

        [SerializeField] private AudioClip _actionSuccess;
        [SerializeField] private AudioClip _actionFailed;
#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        public override void DoAction(Ray cameraRay) {
            if (RecallMark.Instance.markCurrent) {
                Recallable.Recall();
                PlayAudio(_actionSuccess);
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{RecallMark.Instance.markCurrent} was Asked to recall");
#endif
                return;
            }
            PlayAudio(_actionFailed);
        }

        public override void StopAction() { }

    }
}
