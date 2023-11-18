using UnityEngine;

namespace ProjectMultiplayer.ObjectCategory.Recall {
    public class RecallMark : MonoBehaviour {

        private static RecallMark _instance;

        [HideInInspector] public Recallable markCurrent;
        [HideInInspector] public Vector3 markPosition;
        [HideInInspector] public Quaternion markRotation;
        [HideInInspector] public Size.Size.SizeType markSize;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        // Access

        public static RecallMark Instance { get { return _instance; } }

        private void Awake() {
            if (!_instance) {
                _instance = this;
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} Instance created");
#endif
            }
            else if (_instance != this) {
                Destroy(gameObject);
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} duplicate Instance Destroyed");
#endif
            }
        }

    }
}
