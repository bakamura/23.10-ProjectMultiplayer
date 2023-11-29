using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Break;

namespace ProjectMultiplayer.Player.Actions {
    public class Break : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private Vector3 _actionOffset;
        [SerializeField] private Vector3 _actionBox;
        [SerializeField] private AudioClip _breakSuccess;
        [SerializeField] private AudioClip _breakFailed;
        [SerializeField] private AudioClip _pushPlayer;

        [Space(16)]

        [SerializeField] private float _friendPushForce;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        public override void DoAction(Ray cameraRay) {
            foreach (Collider collider in Physics.OverlapBox(transform.position + _actionOffset, _actionBox / 2)) {
                Breakable breakScript = collider.GetComponent<Breakable>();
                if (breakScript && breakScript.TryBreak(_player.Size.Type)) PlayAudio(_breakSuccess);
                Player playerScript = collider.GetComponent<Player>();
                if (playerScript)
                {
                    playerScript.NRigidbody.Rigidbody.AddForce((collider.transform.position - transform.position).normalized * _friendPushForce, ForceMode.VelocityChange); ;
                    PlayAudio(_pushPlayer);
                }
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{collider.name} was Asked to break");
#endif
                return;
            }
#if UNITY_EDITOR
            if (_debugLogs) Debug.Log("Break did not hit any relevant colliders");
#endif
            PlayAudio(_breakFailed);
        }

        public override void StopAction() { }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + _actionOffset, _actionBox / 2);
        }
#endif
    }
}