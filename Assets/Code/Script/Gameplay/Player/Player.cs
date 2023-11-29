using Fusion;
using UnityEngine;

using ProjectMultiplayer.Player.Actions;
using ProjectMultiplayer.Connection;
using ProjectMultiplayer.ObjectCategory.Size;
using UnityEngine.InputSystem;

namespace ProjectMultiplayer.Player {
    public class Player : NetworkBehaviour, IPlayerLeft {

        private bool _canAct = true;

        [Header("Type")]

        [SerializeField] private NetworkManager.PlayerType _type;

        [Header("Movement")]

        [SerializeField] private float _movementSpeed;
        [SerializeField] private Vector3 _checkGroundOffset;
        [SerializeField] private Vector3 _checkGroundBox;
        [SerializeField] private LayerMask _checkGroundLayer;
        private float _currentTurnVelocity;
        [SerializeField] private float _turnDuration;

        [Header("Action")]

        [SerializeField] private PlayerActionData _actionJump;
        [SerializeField] private PlayerActionData _action1;
        [SerializeField] private PlayerActionData _action2;
        [SerializeField] private PlayerActionData _action3;

        [Header("Audio")]
        [SerializeField] private AudioSource _movmentAudioSource;
        [SerializeField] private bool _randomizePicth;
        [SerializeField] private Vector2 _randomizeRange;

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
        private bool _isGrounded;
        private bool _recentlyJumped;
        //private WaitForSeconds _damagedAnimationWait;
        [Networked] private TickTimer _respawnTimer { get; set; }

#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        // Access

        public NetworkManager.PlayerType Type { get { return _type; } }
        public NetworkRigidbody NRigidbody { get { return _nRigidbody; } }
        private Ray _rayCache;
        public Size Size { get { return _size; } }
        public bool IsGrounded => _isGrounded;
        private PlayerActionData[] _playerActions;

        [System.Serializable]
        private struct PlayerActionData {
            public PlayerAction Action;
            [SerializeField] private AnimationClip _animation;
            [HideInInspector] public float CurrentCooldownTime;
            public bool UseCheckGroundInstead;
            public AnimationClip AnimClip => _animation;

            public void ResetCooldown() {
                if (_animation) CurrentCooldownTime = _animation.length;
                else CurrentCooldownTime = 0;
            }
        }

        public override void Spawned() {
            _nRigidbody = GetComponent<NetworkRigidbody>();
            _size = GetComponent<Size>();
            _shieldAbility = GetComponent<Shield>();
            _playerActions = new PlayerActionData[] { /*_actionJump,*/ _action1, _action2, _action3 };

            _camera = Camera.main;
            _screenSize[0] = Screen.width;
            _screenSize[1] = Screen.height;
            Debug.Log($"Spawned {gameObject.name}");
        }

        public override void FixedUpdateNetwork() {
            _isGrounded = Physics.OverlapBox(transform.position + Quaternion.Euler(0, transform.rotation.y, 0) * _checkGroundOffset, _checkGroundBox / 2, Quaternion.identity, _checkGroundLayer) != null;

            if (GetInput(out DataPackInput inputData) && _canAct) {
                _rayCache = _camera.ScreenPointToRay(_screenSize / 2);
                Movement(inputData.Movement);
                if (inputData.Jump != _alreadyJumped) {
                    if (inputData.Jump) {
                        _actionJump.Action.DoAction(_rayCache);
                        LockPlayerAction(_actionJump);
                    }
                    else _actionJump.Action.StopAction();

                }
                if (inputData.Action1 != _alreadyAction1) {
                    if (inputData.Action1) {
                        _action1.Action.DoAction(_rayCache);
                        LockPlayerAction(_action1);
                    }
                    else _action1.Action.StopAction();
                }
                if (inputData.Action2 != _alreadyAction2) {
                    if (inputData.Action2) {
                        _action2.Action.DoAction(_rayCache);
                        LockPlayerAction(_action2);
                    }
                    else _action2.Action.StopAction();
                }
                if (inputData.Action3 != _alreadyAction3) {
                    if (inputData.Action3) {
                        _action3.Action.DoAction(_rayCache);
                        LockPlayerAction(_action3);
                    }
                    else _action3.Action.StopAction();
                }
                _alreadyJumped = inputData.Jump;
                _alreadyAction1 = inputData.Action1;
                _alreadyAction2 = inputData.Action2;
                _alreadyAction3 = inputData.Action3;
            }

            if (_respawnTimer.Expired(Runner)) {
                _respawnTimer = TickTimer.None;

                transform.position = FindObjectOfType<SpawnAnchor>().GetSpawnPosition(_type);
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} has respawned at {transform.position}");
#endif
                // Respawn Animation
            }

            // check to unlock player actions
            for (int i = 0; i < _playerActions.Length; i++) {
                if ((_playerActions[i].UseCheckGroundInstead || _playerActions[i].AnimClip) && _playerActions[i].CurrentCooldownTime > 0) {
                    if (_playerActions[i].UseCheckGroundInstead) {
                        if (!_recentlyJumped) _recentlyJumped = !_isGrounded;
                        if (_recentlyJumped && _isGrounded) {
                            _playerActions[i].CurrentCooldownTime = 0;
                            _recentlyJumped = false;
                            UpdateCanAct(true);
                        }
                    }
                    else {
                        _playerActions[i].CurrentCooldownTime -= Runner.DeltaTime;
                        if (_playerActions[i].CurrentCooldownTime <= 0) UpdateCanAct(true);
                    }
                }
                else {
                    UpdateCanAct(true);
                }
            }

        }

        private void Movement(Vector2 direction) {
            _inputV2ToV3[0] = direction.x;
            _inputV2ToV3[2] = direction.y;
            if (_movmentAudioSource.clip) {
                if (_movmentAudioSource.clip && _inputV2ToV3.sqrMagnitude > 0 && !_movmentAudioSource.isPlaying) {
                    if (_randomizePicth) _movmentAudioSource.pitch = Random.Range(_randomizeRange.x, _randomizeRange.y);
                    _movmentAudioSource.Play();
                }
            }

            if (_inputV2ToV3.sqrMagnitude > 0) {
                float targetAngle = Mathf.Atan2(_inputV2ToV3.x, _inputV2ToV3.z) * Mathf.Rad2Deg + _camera.transform.eulerAngles.y
                _nRigidbody.Rigidbody.AddForce(_movementSpeed * (Quaternion.Euler(0, targetAngle, 0) * Vector3.forward).normalized, ForceMode.Acceleration);
                transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentTurnVelocity, _turnDuration), 0);

            }
        }

        public void TryDamage() {
            if (_shieldAbility != null || !_shieldAbility.IsShielded) {
                Damaged();
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} took damage");
#endif
            }
            else {
                _shieldAbility.onBlockBullet.Invoke();
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} blocked damage with shield");
#endif
            }
        }

        private void Damaged() {
            // Damaged Animation

            _respawnTimer = TickTimer.CreateFromSeconds(Runner, 3f);
        }

        public void PlayerLeft(PlayerRef player) {

        }

        public void UpdateCanAct(bool canAct) {
            _canAct = canAct;
        }

        private void LockPlayerAction(PlayerActionData currentActionGoing) {
            UpdateCanAct(false);
            currentActionGoing.ResetCooldown();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Matrix4x4 prevMatrix = Gizmos.matrix;
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(transform.position + _checkGroundOffset, _checkGroundBox);
            Gizmos.matrix = prevMatrix;
        }
#endif
    }
}
