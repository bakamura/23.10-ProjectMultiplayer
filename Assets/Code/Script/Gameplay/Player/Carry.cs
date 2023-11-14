using System.Linq;
using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Size;

namespace ProjectMultiplayer.Player.Actions {
    public class Carry : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private Vector3 _liftOffset;
        [SerializeField] private Vector3 _liftBox;

        private Transform _carriedObject;

        public override void DoAction(Ray cameraRay) {
            if (!_carriedObject) {
                Size sizeCache;
                foreach (Collider col in Physics.OverlapBox(transform.position + _liftOffset, _liftBox).OrderBy(col => (transform.position + _liftOffset - col.transform.position).sqrMagnitude).ToArray()) {
                    if (col.transform != transform) {
                        sizeCache = col.GetComponent<Size>();
                        if (sizeCache) {
                            _carriedObject = sizeCache.transform;
                            _carriedObject.transform.parent = transform;
                            _carriedObject.transform.localPosition = _liftOffset; // Test Out, Maybe create empty object
                            break;
                        }
                    }
                }
            }
            else {
                _carriedObject.transform.parent = null;
                _carriedObject = null;
            }
        }

        public override void StopAction() { }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position + _liftOffset, _liftBox);
        }
#endif
    }
}
