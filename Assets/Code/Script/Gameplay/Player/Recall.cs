using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Recall;

namespace ProjectMultiplayer.Player.Actions {
    public class Recall : PlayerAction {

        public override void DoAction(Ray cameraRay) {
            if(RecallMark.Instance.markCurrent) Recallable.Recall();
        }

        public override void StopAction() { }

    }
}
