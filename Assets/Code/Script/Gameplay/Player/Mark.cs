using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Recall;

namespace ProjectMultiplayer.Player.Actions {
    public class Mark : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private float _actionRange;

        public override void DoAction(Ray cameraRay) {
            if (Physics.Raycast(cameraRay, out RaycastHit hit) && 
                Vector3.Distance(transform.position, hit.point) < _actionRange) hit.transform.GetComponent<Recallable>()?.Mark();
        }

        public override void StopAction() { }

    }
}
