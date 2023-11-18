using UnityEngine;

namespace ProjectMultiplayer.Player.Actions {
    public abstract class PlayerAction : MonoBehaviour {

        protected Player _player;

        private void Start() {
            _player = GetComponent<Player>();
        }

        public abstract void DoAction(Ray cameraRay);

        public abstract void StopAction();

    }
}
