using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Actions {
    public class Carry : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private Vector3 _liftOffset;
        [SerializeField] private Vector3 _liftBox;

        private Transform _carriedObject;

        public override void DoAction(InputAction.CallbackContext input) {
            if (!_carriedObject) {
                Size sizeCache;
                foreach (Collider col in Physics.OverlapBox(transform.position + _liftOffset, _liftBox).OrderBy(col => (transform.position + _liftOffset - col.transform.position).sqrMagnitude).ToArray()) {
                    if (col.transform != transform) {
                        sizeCache = col.GetComponent<Size>();
                        if (sizeCache) {
                            _carriedObject = sizeCache.transform;
                            break;
                        }
                    }
                }
                _carriedObject.transform.parent = transform;
                _carriedObject.transform.localPosition = _liftOffset; // Test Out, Maybe create empty object
            }
            else {
                _carriedObject.transform.parent = null;
                _carriedObject = null;
            }
        }

        public override void StopAction(InputAction.CallbackContext input) {
            throw new System.NotImplementedException();
        }

    }
}
