using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Debug")]
    [SerializeField] private Text _feedbackText;

    [Header("UI Client Components")]
    [SerializeField] private GameObject _sessionOptionsPrefab;
    [SerializeField] private RectTransform _sessionList;
    [SerializeField] private InputField _seessionNameToFind;
    [SerializeField] private GameObject _findGameUI;
    [SerializeField] private GameObject _waitForServerUI;

    [Header("UI Client Server Components")]    
    [SerializeField] private Text _playerCountText;
    [SerializeField] private InputField _sessionNameText;
    [SerializeField] private GameObject _startGameUI;
    [SerializeField] private GameObject _createMatchUI;

    [Header("Network Components")]
    [SerializeField] private NetworkRunnerHandler _networkHandler;
    private byte _playerCount;
    private void Awake()
    {
        NetworkRunnerHandler.AddCallbackToNetworkRunner(this);
    }
    #region FusionCallbacks
    public void OnConnectedToServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer)
        {
            _playerCount++;
            _playerCountText.text = $"Clients: {_playerCount}";
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        if(sessionList.Count > 0)
        {
            ClearSessionOptions();
            for(int i = 0; i < sessionList.Count; i++)
            {
                AddSessionOption(sessionList[i]);
            }
        }
        else
        {
            _feedbackText.text = "There are no Matches currently going";
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }
#endregion
    public void CreateMatch()
    {
        if (!string.IsNullOrEmpty(_sessionNameText.text))
        {
            _networkHandler.CreateMatch(_sessionNameText.text, SceneManager.GetActiveScene().buildIndex);
            _createMatchUI.SetActive(false);
            _startGameUI.SetActive(true);
        }
        else
        {
            _feedbackText.text = "Insert a Name to the session";
        }
    }

    private void AddSessionOption(SessionInfo info)
    {
        LobbyOptionUI temp = Instantiate(_sessionOptionsPrefab, _sessionList).GetComponent<LobbyOptionUI>();
        temp.SetSessionInfo(info);
        temp.OnJoinSession += OnJoinSessionCallback;
    }

    private void OnJoinSessionCallback(SessionInfo info)
    {
        if (_networkHandler.JoinLobby(info.Name))
        {
            _findGameUI.SetActive(false);
            _waitForServerUI.SetActive(true);
        }
    }

    private void ClearSessionOptions()
    {
        RectTransform[] childs = _sessionList.GetComponentsInChildren<RectTransform>();
        for (int i = 0; i < childs.Length; i++)
        {
            Destroy(childs[i].gameObject);
        }
    }

    public void FindMatch()
    {
        if (!string.IsNullOrEmpty(_seessionNameToFind.text))
        {
            //mostra só a partida
        }
        else
        {
            //mostra todas as partidas disponiveis
            //_feedbackText.text = "Insert a session name";
        }
    }

    public void StartMatch()
    {
        //int n = 0;
        //foreach (PlayerRef p in NetworkRunnerHandler.NetworkRunner.ActivePlayers)
        //    n++;
        if (_playerCount > 0)
        {
            //_networkHandler.
        }
        else
        {
            _feedbackText.text = "Not Enough Players";
        }
    }

    private void OnDestroy()
    {
        NetworkRunnerHandler.RemoveCallbackToNetworkRunner(this);
    }
}
