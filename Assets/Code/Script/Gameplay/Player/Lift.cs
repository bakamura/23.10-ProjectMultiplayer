using System.Linq;
using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Size;

namespace ProjectMultiplayer.Player.Actions {
    public class Lift : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private Vector3 _liftOffset;
        [SerializeField] private Vector3 _liftBox;
        [SerializeField] private AudioClip _liftObjectSuccess;
        [SerializeField] private AudioClip _liftObjectFailed;
        [SerializeField] private AudioClip _liftPlayer;

        [Space(16)]

        [SerializeField] private float _friendThrowupVelocity;
        private Vector3 _friendThrowupForce;

        private Transform _liftedObject;

        private PlayerAnimationHandler _handler;
        [SerializeField] private string _animationBool;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        private void Awake() {
            _handler = GetComponentInChildren<PlayerAnimationHandler>();
            _friendThrowupForce = Vector3.up * _friendThrowupVelocity;
        }

        public override void DoAction(Ray cameraRay) {
            _handler.SetBool(_animationBool, true);
            if (!_liftedObject) {
                Size sizeCache;
                foreach (Collider col in Physics.OverlapBox(transform.position + Quaternion.Euler(0, transform.rotation.y, 0) * _liftOffset, _liftBox).OrderBy(col => (transform.position + _liftOffset - col.transform.position).sqrMagnitude).ToArray()) {
                    if (col.transform != transform) {
                        sizeCache = col.GetComponent<Size>();
                        if (sizeCache) {
                            _handler.SetBool(_animationBool, true);
                            _liftedObject = sizeCache.transform;
                            _liftedObject.transform.parent = transform;
                            _liftedObject.transform.localPosition = _liftOffset; // Test Out, Maybe create empty object
                            _liftedObject.transform.localRotation = Quaternion.identity; // Test Out
                            PlayAudio(_liftObjectSuccess);
#if UNITY_EDITOR
                            if (_debugLogs) Debug.Log($"{_liftedObject.name} is now being lifted by {gameObject.name}");
#endif
                            return;
                        }
                    }
                }
                PlayAudio(_liftObjectFailed);
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} failed to lift anything");
#endif
            }
            else {
                _handler.SetBool(_animationBool, false);
                _liftedObject.transform.parent = null;
                if (_liftedObject.GetComponent<Player>())
                {
                    _liftedObject.GetComponent<Rigidbody>().AddForce(_friendThrowupForce, ForceMode.VelocityChange);
                    PlayAudio(_liftPlayer);
#if UNITY_EDITOR
                    if (_debugLogs) Debug.Log($"{_liftedObject.name} was thrown up by {gameObject.name}");
#endif
                }
                else PlayAudio(_liftObjectFailed);
                _liftedObject = null;
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{_liftedObject.name} STOPED being lifted by {gameObject.name}");
#endif
            }
        }


        public override void StopAction() { }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Matrix4x4 prevMatrix = Gizmos.matrix;
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(_liftOffset, _liftBox);
            Gizmos.matrix = prevMatrix;
        }
#endif

    }
}
