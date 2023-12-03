using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.InputSystem;

using ProjectMultiplayer.Connection;

namespace ProjectMultiplayer.Player
{
    public class InitializeInputPlayer : MonoBehaviour, INetworkRunnerCallbacks
    {
        private static InitializeInputPlayer _instance;
        public static InitializeInputPlayer Instance
        {
            get
            {
                // se ainda n tiver uma referência da instancia, procura ela no GameObject
                if (_instance == null)
                {
                    InitializeInputPlayer[] results = FindObjectsOfType<InitializeInputPlayer>();
                    if (results.Length > 0)
                    {
                        if (results.Length > 1) Debug.Log($"Multiple Instances of {typeof(InitializeInputPlayer).Name} found, destroing extras");
                        for (int i = 1; i < results.Length; i++)
                        {
                            Destroy(results[i]);
                        }
                        _instance = results[0];
                    }
                }
                // se ainda n tiver uma referência da instancia, cria uma do tipo desejado
                //if (_instance == null)
                //    _instance = new GameObject($"Instance of Type: {typeof(InitializeInputPlayer)}").AddComponent<InitializeInputPlayer>();
                return _instance;
            }
        }
        private PlayerInput _inputActions;
        public PlayerInput PlayerActions
        {
            get
            {
                if (_inputActions == null) return _inputActions = GetComponent<PlayerInput>();
                return _inputActions;
            }
        }
        private DataPackInput _dataPackInputCached = new DataPackInput();
        private InputAction _playerMove;
        private InputAction _jump;
        private InputAction _action1;
        private InputAction _action2;
        private InputAction _action3;
        private Camera _camera;
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }
            _camera = Camera.main;
            NetworkManagerReference.Instance.NetworkRunner.AddCallbacks(this);
            _playerMove = PlayerActions.actions["MovePlayer"];
            _jump = PlayerActions.actions["Jump"];
            _action1 = PlayerActions.actions["Action1"];
            _action2 = PlayerActions.actions["Action2"];
            _action3 = PlayerActions.actions["Action3"];
        }

        #region Photon Callbacks
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {

        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {

        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            _dataPackInputCached.CameraYAngle = _camera.transform.eulerAngles.y;
            _dataPackInputCached.Movement = _playerMove.ReadValue<Vector2>();
            _dataPackInputCached.Jump = _jump.IsPressed();
            _dataPackInputCached.Action1 = _action1.IsPressed();
            _dataPackInputCached.Action2 = _action2.IsPressed();
            _dataPackInputCached.Action3 = _action3.IsPressed();
            input.Set(_dataPackInputCached);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {

        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {

        }

        public void OnConnectedToServer(NetworkRunner runner)
        {

        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {

        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {

        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {

        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {

        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {

        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {

        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {

        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {

        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {

        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {

        }
        #endregion
    }
}