using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace ProjectMultiplayer.Connection
{
    public class SpawnAnchor : MonoBehaviour
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

        private bool _alreadySpawnedAllPlayers;
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private float _debugGizmoSize;
        [SerializeField] private Vector3 _textOffset = Vector3.up;
        [SerializeField] private Color _debugGizmoColor;
#endif

        private void Awake()
        {
            _initialPosition = transform.position;
            NetworkManagerReference.Instance.OnFixedNetworkUpdate += SpawnPlayers;
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

        private void SpawnPlayers()
        {
            if (!_alreadySpawnedAllPlayers)
            {
                if (NetworkManagerReference.Instance.NetworkRunner.IsServer)
                {
                    foreach (var player in NetworkManagerReference.Instance.PlayersDictionary)
                    {
                        NetworkManagerReference.Instance.NetworkRunner.Spawn(_playersPrefabList.GetPlayerPrefab(player.Value.PlayerType), GetSpawnPosition(player.Value.PlayerType), Quaternion.identity, player.Key);
                    }
                }
                _alreadySpawnedAllPlayers = true;
            }
        }        

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