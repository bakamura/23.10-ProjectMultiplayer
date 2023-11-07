using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace ProjectMultiplayer.Connection
{
    public class SpawnAnchor : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private PlayersPrefabList _playersPrefabList;
        [SerializeField] private SpawnPointData[] _spawnPoints;
        private Vector3 _initialPosition;
        [Serializable]
        private struct SpawnPointData
        {
            public NetworkManager.PlayerType PlayerType;
            public Vector3 SpawnPoint;
        }

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private float _debugGizmoSize;
        [SerializeField] private Vector3 _textOffset = Vector3.up;
        [SerializeField] private Color _debugGizmoColor;
#endif

        private void Awake()
        {
            _initialPosition = transform.position;
            NetworkManagerReference.Instance.AddCallbackToNetworkRunner(this);
        }

        public Vector3 GetSpawnPosition(NetworkManager.PlayerType playerType)
        {
            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                if (_spawnPoints[i].PlayerType == playerType)
                    return _spawnPoints[i].SpawnPoint + _initialPosition;
            }
            Debug.LogError($"the player type {playerType} does not have a spawn point defined");
            return Vector3.zero;
        }

        private void OnDestroy()
        {
            NetworkManagerReference.Instance.RemoveCallbackToNetworkRunner(this);
        }
        #region Photon Callbacks
        public void OnConnectedToServer(NetworkRunner runner)
        {

        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {

        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {

        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {

        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {

        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {

        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {

        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {

        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {

        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {

        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {

        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            if (runner.IsServer)
            {
                foreach (var player in NetworkManagerReference.Instance.PlayersDictionary)
                {
                    runner.Spawn(_playersPrefabList.GetPlayerPrefab(player.Value.PlayerType), GetSpawnPosition(player.Value.PlayerType), Quaternion.identity, player.Key);
                }
            }
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {

        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {

        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {

        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {

        }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_spawnPoints != null)
            {
                Gizmos.color = _debugGizmoColor;
                for (int i = 0; i < _spawnPoints.Length; i++)
                {
                    Gizmos.DrawSphere(_spawnPoints[i].SpawnPoint + transform.position, _debugGizmoSize);
                    Handles.Label(_spawnPoints[i].SpawnPoint + transform.position + _textOffset, _spawnPoints[i].PlayerType.ToString());
                }
            }
        }
#endif
    }
}