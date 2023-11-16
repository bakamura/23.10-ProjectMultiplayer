using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

using ProjectMultiplayer.Player.Actions;
using ProjectMultiplayer.Connection;
using ProjectMultiplayer.ObjectCategory.Size;

namespace ProjectMultiplayer.Player {
    public class Player : NetworkBehaviour, IPlayerLeft {

        //private bool _canAct = true;

        [Header("Type")]

        [SerializeField] private NetworkManager.PlayerType _type;

        [Header("Movement")]

        [SerializeField] private float _movementSpeed;

        [Header("Action")]

        [SerializeField] private PlayerAction _actionJump;
        [SerializeField] private PlayerAction _action1;
        [SerializeField] private PlayerAction _action2;
        [SerializeField] private PlayerAction _action3;

        [Header("Cache")]

        private NetworkRigidbody _nRigidbody;
        private Camera _camera;
        private Vector3 _screenSize;
        private Size _size;
        private Shield _shieldAbility;

        private bool _alreadyJumped;
        private bool _alreadyAction1;
        private bool _alreadyAction2;
        private bool _alreadyAction3;

        private Vector3 _inputV2ToV3 = Vector3.zero;
        //private WaitForSeconds _damagedAnimationWait;
        [Networked] private TickTimer _respawnTimer { get; set; }

        // Access

        public NetworkManager.PlayerType Type { get { return _type; } }
        public NetworkRigidbody NRigidbody { get { return _nRigidbody; } }
        private Ray _rayCache;
        public Size Size { get { return _size; } }

        //[ContextMenu("Test")]
        //private void Test()
        //{
        //    Debug.Log(_action1.GetType() == typeof(Jump));
        //}

        public override void Spawned() {
            _nRigidbody = GetComponent<NetworkRigidbody>();
            _size = GetComponent<Size>();
            _shieldAbility = GetComponent<Shield>();

            _camera = Camera.main;
            _screenSize[0] = Screen.width;
            _screenSize[1] = Screen.height;
            Debug.Log($"Spawned {name}");
        }

        public override void FixedUpdateNetwork() {
            if (GetInput(out DataPackInput inputData)) {
                _rayCache = _camera.ScreenPointToRay(_screenSize / 2);
                Movement(inputData.Movement);
                if (inputData.Jump != _alreadyJumped && inputData.Jump) _actionJump.DoAction(_rayCache);
                if (inputData.Action1 != _alreadyAction1 && inputData.Action1) _action1.DoAction(_rayCache);
                if (inputData.Action2 != _alreadyAction2 && inputData.Action2) _action2.DoAction(_rayCache);
                if (inputData.Action3 != _alreadyAction3 && inputData.Action3) _action3.DoAction(_rayCache);
                _alreadyJumped = inputData.Jump;
                _alreadyAction1 = inputData.Action1;
                _alreadyAction2 = inputData.Action2;
                _alreadyAction3 = inputData.Action3;
            }

            if (_respawnTimer.Expired(Runner)) {
                _respawnTimer = TickTimer.None;

                transform.position = FindObjectOfType<SpawnAnchor>().GetSpawnPosition(_type);

                // Respawn Animation
            }
        }

        private void Movement(Vector2 direction) {
            _inputV2ToV3[0] = direction.x;
            _inputV2ToV3[2] = direction.y;

            _nRigidbody.Rigidbody.AddForce(_inputV2ToV3 * _movementSpeed, ForceMode.Acceleration);
        }

        public void TryDamage() {
            if (_shieldAbility != null || !_shieldAbility.IsShielded) Damaged();
            else _shieldAbility.onBlockBullet.Invoke();
        }

        private void Damaged() {
            // Damaged Animation

            _respawnTimer = TickTimer.CreateFromSeconds(Runner, 3f);
        }

        public void PlayerLeft(PlayerRef player) {

        }

    }
}
