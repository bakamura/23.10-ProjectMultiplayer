using Fusion;
using UnityEngine;

namespace ObjectCategory.Break {
    public class Breakable : NetworkBehaviour {

        [Header("Parameter")]

        [SerializeField] private GameObject[] _objectToSpawn;

        [Header("Cache")]

        private Size.Size _size;

        public override void Spawned() {
            for (int i = 0; i < _objectToSpawn.Length; i++) _objectToSpawn[i] = Instantiate(_objectToSpawn[i], transform.position, transform.rotation);
        }

        public void TryBreak(Size.Size.SizeType breakerSize) {
            if (breakerSize >= _size.Type) {
                foreach (GameObject obj in _objectToSpawn) obj.SetActive(true);
            }
            gameObject.SetActive(false);
        }

    }
}