using UnityEngine;

namespace ProjectMultiplayer.Player.Actions {
    public abstract class PlayerAction : MonoBehaviour {

        protected static Player _player;

        public abstract void DoAction(Ray cameraRay);

        public abstract void StopAction();

    }
}
