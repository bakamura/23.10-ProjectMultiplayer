using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class LobbyUI : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Debug")]
    [SerializeField] private Text _feedbackText;

    [Header("UI Client Components")]
    [SerializeField] private GameObject _sessionOptionsPrefab;
    [SerializeField] private RectTransform _sessionList;
    [SerializeField] private InputField _sessionNameToFind;
    [SerializeField] private GameObject _findGameUI;
    [SerializeField] private GameObject _waitForServerUI;

    [Header("UI Client Server Components")]
    [SerializeField] private Text _playerCountText;
    [SerializeField] private InputField _sessionNameText;
    [SerializeField] private GameObject _startGameUI;
    [SerializeField] private GameObject _createMatchUI;

    [Header("Network Components")]
    [SerializeField] private NetworkRunnerHandler _networkHandler;
    [SerializeField] private string _sceneToLoadWhenLobbyReady;
    private Dictionary<string, SessionInfo> _currentSessions = new Dictionary<string, SessionInfo>();
    private void Start()
    {
        NetworkRunnerHandler.AddCallbackToNetworkRunner(this);
    }
    #region FusionCallbacks
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("LobbyUIOnConnectedToServer");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("LobbyUIOnConnectFailed");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("LobbyUIOnConnectRequest");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("LobbyUIOnCustomAuthenticationResponse");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("LobbyUIOnDisconnectedFromServer");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("LobbyUIOnHostMigration");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        Debug.Log("LobbyUIOnInput");
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log("LobbyUIOnInputMissing");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        //if (!runner.IsServer)
        //{
        //runner.SetPlayerObject(player, null);
        
        NetworkRunnerHandler.PlayersRefs.Add(player);
        _playerCountText.text = $"Players: {NetworkRunnerHandler.PlayersRefs.Count}";
        //}
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (NetworkRunnerHandler.PlayersRefs.Contains(player)) NetworkRunnerHandler.PlayersRefs.Remove(player);
        Debug.Log("LobbyUIOnPlayerLeft");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        Debug.Log("LobbyUIOnReliableDataReceived");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("LobbyUIOnSceneLoadDone");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("LobbyUIOnSceneLoadStart");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"New session Created");
        if (sessionList.Count > 0)
        {
            ClearSessionOptions();
            for (int i = 0; i < sessionList.Count; i++)
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
        Debug.Log("LobbyUIOnShutdown");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("LobbyUIOnUserSimulationMessage");
    }
    #endregion
    public void CreateMatch()
    {
        if (!string.IsNullOrEmpty(_sessionNameText.text))
        {
            var result = _networkHandler.CreateMatch(_sessionNameText.text, SceneManager.GetActiveScene().buildIndex);
            if (result.IsCompleted)
            {
                _createMatchUI.SetActive(false);
                _startGameUI.SetActive(true);
            }
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
        _currentSessions.Add(info.Name, info);
    }
    private void ClearSessionOptions()
    {
        _currentSessions.Clear();
        if (_sessionList.GetComponentsInChildren<RectTransform>().Length > 0)
        {
            RectTransform[] childs = _sessionList.GetComponentsInChildren<RectTransform>();
            for (int i = 0; i < childs.Length; i++)
            {
                Destroy(childs[i].gameObject);
            }
        }
    }

    public void UpdateSessionsList()
    {
        ClearSessionOptions();
        SessionInfo[] temp = _currentSessions.Values.ToArray();
        for (int i = 0; i < temp.Length; i++)
        {
            AddSessionOption(temp[i]);
        }
    }

    private void OnJoinSessionCallback(SessionInfo info)
    {
        if (_networkHandler.JoinLobby(info.Name))
        {
            _findGameUI.SetActive(false);
            _waitForServerUI.SetActive(true);
        }
    }


    public void FindMatch()
    {
        ClearSessionOptions();
        if (!string.IsNullOrEmpty(_sessionNameToFind.text) && _currentSessions.ContainsKey(_sessionNameToFind.text))
        {
            AddSessionOption(_currentSessions[_sessionNameToFind.text]);
        }
        else
        {
            UpdateSessionsList();
        }
    }

    public void StartMatch()
    {
        //int n = 0;
        //foreach (PlayerRef p in NetworkRunnerHandler.NetworkRunner.ActivePlayers)
        //    n++;
        if (NetworkRunnerHandler.PlayersRefs.Count > 0)
        {
            //só funciona para caso o jogo tenha sempre apenas 1 cena ativa, caso tenha mais, criar uma classe q implementa INetworkSceneManager
            NetworkRunnerHandler.NetworkRunner.SetActiveScene(_sceneToLoadWhenLobbyReady);
            //_networkHandler.NetworkRunner.SetActiveScene(SceneUtility.GetBuildIndexByScenePath($"Assets/Game/Scene/{_sceneToLoadWhenLobbyReady}"));
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
