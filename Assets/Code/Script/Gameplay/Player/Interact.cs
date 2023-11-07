using UnityEngine;

namespace ProjectMultiplayer.Player.Actions {
    public class Interact : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private float _actionRange;

        public override void DoAction(Ray cameraRay) {
            if (Physics.Raycast(cameraRay, out RaycastHit hit) && 
                Vector3.Distance(transform.position, hit.point) < _actionRange) hit.transform.GetComponent<IInteractable>()?.Interact();

        }

        public override void StopAction() { }

    }
}
