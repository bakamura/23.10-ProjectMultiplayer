using Fusion;
using System.Collections;
using UnityEngine;

namespace ProjectMultiplayer.ObjectCategory.Recall {
    [RequireComponent(typeof(NetworkRigidbody), typeof(Collider))]
    public class Recallable : NetworkBehaviour {

        [Header("Parameters")]

        [SerializeField] private float _recallDuration;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif
        private NetworkRigidbody _networkRb;
        private Collider _collider;

        private void Awake()
        {
            _networkRb = GetComponent<NetworkRigidbody>();
            _collider = GetComponent<Collider>();
        }

        public void Mark() {
            RecallMark.Instance.markCurrent = this;
            RecallMark.Instance.markPosition = transform.position;
            RecallMark.Instance.markRotation = transform.rotation;
            RecallMark.Instance.markSize = GetComponent<Size.Size>().Type;
#if UNITY_EDITOR
            if (_debugLogs) Debug.Log($"{gameObject.name} has been Marked");
#endif
        }

        public static void Recall() {
            RecallMark.Instance.markCurrent.StartCoroutine(RecallMark.Instance.markCurrent.RecallRoutine());
#if UNITY_EDITOR
            Debug.Log($"{RecallMark.Instance.markCurrent.gameObject.name} has been Recalled");
#endif
        }

        public IEnumerator RecallRoutine() {
            Vector3 initialPosition = transform.position;
            Quaternion initialRotation = transform.rotation;
            _networkRb.Rigidbody.isKinematic = true; //
            _collider.enabled = false;

            float progress = 0;
            while (progress < 1) {
                progress += Time.deltaTime / _recallDuration;
                _networkRb.Transform.transform.SetPositionAndRotation(Vector3.Lerp(initialPosition, RecallMark.Instance.markPosition, progress), Quaternion.Lerp(initialRotation, RecallMark.Instance.markRotation, progress));

                yield return null;
            }
            _networkRb.Transform.transform.SetPositionAndRotation(RecallMark.Instance.markPosition, RecallMark.Instance.markRotation);

            _networkRb.Rigidbody.isKinematic = false; //
            _collider.enabled = true;

            RecallMark.Instance.markCurrent = null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 2f);
        }
#endif
    }
}
