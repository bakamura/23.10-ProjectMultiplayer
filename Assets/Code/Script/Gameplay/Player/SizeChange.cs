using UnityEngine;

using ProjectMultiplayer.ObjectCategory.Size;

namespace ProjectMultiplayer.Player.Actions {
    public class SizeChange : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private bool _isGrowing;
        [SerializeField] private float _actionRange;

        public override void DoAction(Ray cameraRay) {
            if (Physics.Raycast(cameraRay, out RaycastHit hit) && Vector3.Distance(transform.position, hit.point) < _actionRange) {
                Size hitSize = hit.transform.GetComponent<Size>();
                if (hitSize && hitSize.TriPhase) hitSize.ChangeSize(_isGrowing);
            }
        }

        public override void StopAction() { }

    }
}
