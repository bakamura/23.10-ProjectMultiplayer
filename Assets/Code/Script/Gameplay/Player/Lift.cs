using System.Linq;
using UnityEngine;

namespace ProjectMultiplayer.Player.Actions {
    public class Lift : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private Vector3 _liftOffset;
        [SerializeField] private Vector3 _liftBox;

        [Space(16)]

        [SerializeField] private float _friendThrowupVelocity;
        private Vector3 _friendThrowupForce;

        private Transform _liftedObject;

        private void Awake() {
            _friendThrowupForce = Vector3.up * _friendThrowupVelocity;
        }

        public override void DoAction(Ray cameraRay) {
            if (!_liftedObject) {
                Size sizeCache;
                foreach (Collider col in Physics.OverlapBox(transform.position + _liftOffset, _liftBox).OrderBy(col => (transform.position + _liftOffset - col.transform.position).sqrMagnitude).ToArray()) {
                    if (col.transform != transform) {
                        sizeCache = col.GetComponent<Size>();
                        if (sizeCache) {
                            _liftedObject = sizeCache.transform;
                            break;
                        }
                    }
                }
                _liftedObject.transform.parent = transform;
                _liftedObject.transform.localPosition = _liftOffset; // Test Out, Maybe create empty object
                _liftedObject.transform.localRotation = Quaternion.identity; // Test Out
            }
            else {
                if (_liftedObject.GetComponent<Player>()) _liftedObject.GetComponent<Rigidbody>().AddForce(_friendThrowupForce, ForceMode.VelocityChange);
                _liftedObject.transform.parent = null;
                _liftedObject = null;
            }
        }


        public override void StopAction() { }

    }
}
