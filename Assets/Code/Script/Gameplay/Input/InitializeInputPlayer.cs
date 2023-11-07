using System.Collections;
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
                    InitializeInputPlayer[] results = GameObject.FindObjectsOfType<InitializeInputPlayer>();
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
                if (_instance == null)
                    _instance = new GameObject($"Instance of Type: {typeof(InitializeInputPlayer)}").AddComponent<InitializeInputPlayer>();
                return _instance;
            }
        }
        private InputPlayer _inputActions;
        public InputPlayer PlayerActions
        {
            get
            {
                if (_inputActions == null) return _inputActions = new InputPlayer();
                return _inputActions;
            }
        }

        private void Awake()
        {
            NetworkManagerReference.Instance.NetworkRunner.AddCalbacks(this);
            if (_instance != this)
            {
                Destroy(this);
                return;
            }
            PlayerActions.Player.MovePlayer.performed += OnMoveCharacter;
            PlayerActions.Player.Action1.performed += OnAction1;
            PlayerActions.Player.Action2.performed += OnAction2;
            PlayerActions.Player.Action3.performed += OnAction3;
            PlayerActions.Enable();
        }
        private DataPackInput _dataPackInputCached = new DataPackInput();

        #region Input System Callbacks
        private void OnMoveCharacter(InputAction.CallbackContext context)
        {
            _dataPackInputCached.Movement = context.ReadValue<Vector2>();
        }
        private void OnAction1(InputAction.CallbackContext context)
        {
            _dataPackInputCached.Action1 = context.ReadValue<float>() >= 1f;
        }
        private void OnAction2(InputAction.CallbackContext context)
        {
            _dataPackInputCached.Action2 = context.ReadValue<float>() >= 1f;
        }
        private void OnAction3(InputAction.CallbackContext context)
        {
            _dataPackInputCached.Action3 = context.ReadValue<float>() >= 1f;
        }
        #endregion

        #region Photon Callbacks
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {

        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {

        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
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