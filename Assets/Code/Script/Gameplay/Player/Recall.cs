using UnityEngine;

namespace ProjectMultiplayer.Player.Actions {
    public class Recall : PlayerAction {

        public override void DoAction(Ray cameraRay) {
            Recallable.Recall();
        }

        public override void StopAction() { }

    }
}
