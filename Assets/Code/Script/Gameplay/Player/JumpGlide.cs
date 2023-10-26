using UnityEngine;
using UnityEngine.InputSystem;

public class JumpGlide : PlayerAction {

    [Header("Parameters")]

    [SerializeField] private float _jumpHeight;
    private Vector3 _jumpForce;

    [Space(16)]

    [SerializeField] private float _glideSpeed;
    private Vector3 _glideForce;
    private bool _isGliding = false;

    [Space(16)]

    [SerializeField] private Vector3 _checkGroundOffset;
    [SerializeField] private Vector3 _checkGroundBox;
    [SerializeField] private LayerMask _checkGroundLayer;

    public override void Spawned() {
        _jumpForce = Vector3.up * Mathf.Sqrt(2 * -Physics.gravity.y * _jumpHeight);
        _glideForce = Vector3.up * _glideSpeed;
    }

    public override void FixedUpdateNetwork() {
        if (_isGliding && _player.Rigidbody.velocity.y < 0) _player.Rigidbody.AddForce(_glideForce, ForceMode.Acceleration);
    }

    public override void DoAction(InputAction.CallbackContext input) {
        if (IsGrounded()) {
            _player.Rigidbody.AddForce(_jumpForce, ForceMode.VelocityChange);
        }
        _isGliding = true;
    }

    public override void StopAction(InputAction.CallbackContext input) {
        _isGliding = false;
    }

    private bool IsGrounded() {
        return Physics.OverlapBox(transform.position + _checkGroundOffset, _checkGroundBox, Quaternion.identity, _checkGroundLayer) != null;
    }

}
