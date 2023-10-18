using UnityEngine;

public class GenericPlayer : MonoBehaviour {

    [Header("Movement")]

    [SerializeField] private float _movementSpeed;
    private Vector3 _movementInput;

    [SerializeField] private float _movementTurnDuration;
    private float _movementTurnVelocityCache;

    [Header("Cache")]

    private Rigidbody _rb;
    private Transform _camTransform;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        _camTransform = Camera.main.transform;
    }

    private void Update() {
        _movementInput[0] = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
        _movementInput[2] = (Input.GetKey(KeyCode.S) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);

        float targetAngle = Mathf.Atan2(_movementInput.x, _movementInput.z) * Mathf.Rad2Deg + _camTransform.eulerAngles.y;
        if (_movementInput != Vector3.zero) transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _movementTurnVelocityCache, _movementTurnDuration), 0);
        _rb.velocity = _movementSpeed * (_movementInput.sqrMagnitude > 0 ? (Quaternion.Euler(0, targetAngle, 0) * Vector3.forward).normalized : 
                                                                           new Vector3(0, _rb.velocity.y, 0));
    }

}
