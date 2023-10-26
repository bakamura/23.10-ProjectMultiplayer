using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour, IPlayerLeft {

    private bool _canAct = true;

    [Header("Singleton")]

    private Player _instanceHeavy;
    private Player _instanceSpringer;
    private Player _instanceNavi;
    [SerializeField] private NetworkManager.PlayerType _type;

    [Header("Movement")]

    [SerializeField] private float _movementSpeed;

    [Header("Action")]

    [SerializeField] private PlayerAction _actionJump;
    [SerializeField] private PlayerAction _action1;
    [SerializeField] private PlayerAction _action2;
    [SerializeField] private PlayerAction _action3;

    [Header("Cache")]

    private InputPlayer _input = new InputPlayer();
    private Rigidbody _rigidbody;
    private Camera _camera;
    private Vector3 _screenSize;
    private Size _size;
    private Shield _shieldAbility;

    private Vector2 _inputV2Cache;
    private Vector3 _inputV2ToV3 = Vector3.zero;
    private WaitForSeconds _damagedAnimationWait;

    // Access

    public InputPlayer Input { get { return _input; } }
    public Rigidbody Rigidbody { get { return _rigidbody; } }
    public Camera Camera { get { return _camera; } }
    public Vector3 ScreenSize { get { return _screenSize; } }
    public Size Size { get { return _size; } }

    public override void Spawned() {
        switch (_type) {
            case NetworkManager.PlayerType.Heavy:
                if (!_instanceHeavy) _instanceHeavy = this;
                else if (_instanceHeavy != this) Destroy(gameObject);
                break;

            case NetworkManager.PlayerType.Springer:
                if (!_instanceSpringer) _instanceSpringer = this;
                else if (_instanceSpringer != this) Destroy(gameObject);
                break;

            case NetworkManager.PlayerType.Navi:
                if (!_instanceNavi) _instanceNavi = this;
                else if (_instanceNavi != this) Destroy(gameObject);
                break;
        }

        _input.Enable();
        _input.Player.MovePlayer.performed += Movement;
        _input.Player.Action1.performed += _action1.DoAction;

        _rigidbody = GetComponent<Rigidbody>();
        _camera = Camera.main;
        _screenSize[0] = Screen.width;
        _screenSize[1] = Screen.height;
        _shieldAbility = GetComponent<Shield>();

        //_damagedAnimationWait = DURAÇÃO DA ANIMAÇÃO DE MORTE
    }

    private void Movement(InputAction.CallbackContext input) {
        _inputV2Cache = input.ReadValue<Vector2>();
        _inputV2ToV3[0] = _inputV2Cache.x;
        _inputV2ToV3[2] = _inputV2Cache.y;

        _rigidbody.AddForce(_inputV2ToV3 * _movementSpeed, ForceMode.Acceleration);
    }

    public void TryDamage() {
        if (!_shieldAbility || !_shieldAbility.IsShielded) StartCoroutine(DamagedRoutine());
        else _shieldAbility.onBlockBullet.Invoke();
    }

    private IEnumerator DamagedRoutine() {
        // Damaged Animation

        yield return _damagedAnimationWait;

        foreach (SpawnAnchor anchor in FindObjectsOfType<SpawnAnchor>()) {
            if (_type == anchor.PlayerType) {
                transform.position = anchor.transform.position;
                break;
            }
        }

        // Respawn Animation
    }

    public void PlayerLeft(PlayerRef player) {

    }

}
