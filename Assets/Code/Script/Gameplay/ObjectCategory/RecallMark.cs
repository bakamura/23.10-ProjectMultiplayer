using Fusion;
using UnityEngine;

namespace ObjectCategory.Recall {
    public class RecallMark : NetworkBehaviour {

        private static RecallMark _instance;

        [HideInInspector] public Recallable markCurrent;
        [HideInInspector] public Vector3 markPosition;
        [HideInInspector] public Quaternion markRotation;
        [HideInInspector] public Size.Size.SizeType markSize;

        // Access

        public static RecallMark Instance { get { return _instance; } }

        public override void Spawned() {
            if (!_instance) _instance = this;
            else if (_instance != this) Destroy(gameObject);
        }

    }
}
