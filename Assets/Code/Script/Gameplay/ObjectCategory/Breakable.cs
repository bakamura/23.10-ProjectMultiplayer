using Fusion;
using UnityEngine;

namespace ProjectMultiplayer.ObjectCategory.Break {
    public class Breakable : NetworkBehaviour {

        [Header("Parameter")]

        [SerializeField] private GameObject[] _objectToSpawn;

        [Header("Cache")]

        private Size.Size _size;

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        public override void Spawned() {
            _size = GetComponent<Size.Size>();

            for (int i = 0; i < _objectToSpawn.Length; i++) _objectToSpawn[i] = Instantiate(_objectToSpawn[i], transform.position, transform.rotation);

#if UNITY_EDITOR
            if (_debugLogs) {
                bool b = false;
                if (!_size) {
                    Debug.LogWarning($"{gameObject.name}'s Size is empty");
                    b = true;
                }
                if (_objectToSpawn.Length <= 0) {
                    Debug.LogWarning($"{gameObject.name} has no Objects Spawned!");
                    b = true;
                }
                if (!b) Debug.Log($"{gameObject.name} has been properly initialized");
            }
#endif
        }

        public bool TryBreak(Size.Size.SizeType breakerSize) {
            if (breakerSize >= _size.Type) {
                foreach (GameObject obj in _objectToSpawn) obj.SetActive(true);
                gameObject.SetActive(false);
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} has been broken");
#endif
                return true;
            }
#if UNITY_EDITOR
            else if (_debugLogs) Debug.Log($"Couldn't break {gameObject.name} due to lacking Size form the breaker");
#endif
            return false;
        }

    }
}