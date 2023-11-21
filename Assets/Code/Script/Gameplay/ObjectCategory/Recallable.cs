using Fusion;
using System.Collections;
using UnityEngine;

namespace ProjectMultiplayer.ObjectCategory.Recall {
    public class Recallable : NetworkBehaviour {

        [Header("Parameters")]

        [SerializeField] private float _recallDuration;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

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
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            Collider collider = GetComponent<Collider>();
            rigidbody.isKinematic = true; //
            collider.enabled = false;

            float progress = 0;
            while (progress < 1) {
                progress += Time.deltaTime / _recallDuration;
                transform.position = Vector3.Lerp(initialPosition, RecallMark.Instance.markPosition, progress);
                transform.rotation = Quaternion.Lerp(initialRotation, RecallMark.Instance.markRotation, progress);

                yield return null;
            }

            transform.position = RecallMark.Instance.markPosition;
            transform.rotation = RecallMark.Instance.markRotation;

            rigidbody.isKinematic = false; //
            collider.enabled = true;

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
