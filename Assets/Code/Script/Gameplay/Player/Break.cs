using UnityEngine;

namespace ProjectMultiplayer.Player.Actions {
    public class Break : PlayerAction {

        [Header("Parameters")]

        [SerializeField] private Vector3 _actionOffset;
        [SerializeField] private Vector3 _actionBox;

        [Space(16)]

        [SerializeField] private float _friendPushForce;

        public override void DoAction(Ray cameraRay) {
            foreach (Collider collider in Physics.OverlapBox(transform.position + _actionOffset, _actionBox / 2)) {
                collider.GetComponent<Breakable>()?.TryBreak(_player.Size.Type);
                collider.GetComponent<Player>()?.Rigidbody.AddForce((collider.transform.position - transform.position).normalized * _friendPushForce, ForceMode.VelocityChange);
            }
        }

        public override void StopAction() { }

    }
}