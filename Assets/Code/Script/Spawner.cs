using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPlayer _playerPrefab;
    [SerializeField] private SpawnPoints _spawnPoints;
    private CharacterInputHandler _inputHandler;

    private void Start()
    {
        NetworkRunnerHandler.AddCallbackToNetworkRunner(this);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

        //if (runner.IsServer)
        //{
        //    Debug.Log("OnPlayerJoined this is server. Spawning player");
        //    runner.Spawn(_playerPrefab, _spawnPoints.GetRandomSpawnPoint(), Quaternion.identity, player);
        //}
        //else Debug.Log("OnPlayerJoined");

    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (_inputHandler == null && NetworkPlayer.LocalPlayer != null)
        {
            _inputHandler = NetworkPlayer.LocalPlayer.GetComponent<CharacterInputHandler>();
        }

        if (_inputHandler != null)
        {
            input.Set(_inputHandler.GetInputData());
        }
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("OnConnectFaield  ");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("OnConnectRequest");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("OnCustomAuthenticationResponse");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("OnDisconnectedFromServer");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("OnHostMigration");
    }


    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log("OnInputMissing");
    }


    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerLeft");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        Debug.Log("OnReliableDataReceived");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        //if (NetworkRunnerHandler.GameStarted)
        //{
        //    if (runner.IsServer)
        //    {
        //        Debug.Log("OnPlayerJoined this is server. Spawning player");
        //        runner.Spawn(_playerPrefab, _spawnPoints.GetRandomSpawnPoint(), Quaternion.identity, player);
        //    }
        //    else Debug.Log("OnPlayerJoined");
        //}
        if (!runner.LocalPlayer.IsValid)
        {
            runner.SetPlayerObject(NetworkRunnerHandler.LocalPlayerRef, runner.Spawn(_playerPrefab, _spawnPoints.GetRandomSpawnPoint(), Quaternion.identity, NetworkRunnerHandler.LocalPlayerRef).GetComponent<NetworkObject>());
        }
        //runner.Spawn(_playerPrefab, _spawnPoints.GetRandomSpawnPoint(), Quaternion.identity);
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("OnSceneLoadStart");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("OnSessionListUpdated");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("OnShutdown");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("OnUserSimulationMessage");
    }

    private void OnDestroy()
    {
        NetworkRunnerHandler.RemoveCallbackToNetworkRunner(this);
    }
}
