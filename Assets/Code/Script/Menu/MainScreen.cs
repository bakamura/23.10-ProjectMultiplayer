using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class MainScreen : Menu, INetworkRunnerCallbacks
{
    [Header("General Components")]
    [SerializeField] private CanvasGroup _selectCharacterUI;
    [SerializeField] private PlayerSelectionUI[] _playerSelectionUIs;

    [Header("Server Components")]
    [SerializeField] private TMP_InputField _serverInputField;
    [SerializeField] private UnityEngine.UI.Button _startGameBtn;

    [Header("Client Components")]
    [SerializeField] private TMP_InputField _clientInputField;
    [SerializeField] private CanvasGroup _clientUI;

    [Header("Debug")]
    [SerializeField] private Text _feedbackText;

    private void Start()
    {
        NetworkManager.Instance.AddCallbackToNetworkRunner(this);
    }

    private void OnEnable()
    {
        InitializeInputPlayer.Instance.PlayerActions.UI.Cancel.performed += ReturnToPreviousCanvas;
    }

    private void OnDisable()
    {
        InitializeInputPlayer.Instance.PlayerActions.UI.Cancel.performed -= ReturnToPreviousCanvas;
        NetworkManager.Instance.RemoveCallbackToNetworkRunner(this);
    }

    private void ReturnToPreviousCanvas(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() == 1 && GetPreviousCanvasGroup(out CanvasGroup temp))
        {
            ChangeCurrentCanvas(temp);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void JoinMatch()
    {
        if (!string.IsNullOrEmpty(_clientInputField.text))
        {
            NetworkManager.Instance.JoinMacth(_clientInputField.text, OnMatchResult);            
        }
        else
        {
            _feedbackText.text = "INSERT A SESSION NAME TO LOOK FOR";
        }
    }

    public void CreateMatch()
    {
        if (!string.IsNullOrEmpty(_serverInputField.text))
        {
            NetworkManager.Instance.CreateMatch(_serverInputField.text, SceneManager.GetActiveScene().buildIndex, OnMatchResult);
        }
        else
        {
            _feedbackText.text = "INSERT A NAME FOR THE SESSION";
        }
    }

    public void StartMatch()
    {

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_UpdatePlayerTypeUI(int playerId, NetworkManager.PlayerType playerType)
    {
        if (NetworkManager.Instance.PlayersData.ContainsKey(playerId))
        {
            NetworkManager.Instance.PlayersData.Add(playerId, new NetworkManager.PlayerData(NetworkManager.Instance.PlayersData[playerId].PlayerRef, playerType));
            _playerSelectionUIs[NetworkManager.Instance.PlayersData.Keys.ToList().IndexOf(playerId)].UpdateSelectedPlayer(playerType);
        }
        if (NetworkManager.Instance.NetworkRunner.IsServer)
        {
            //checks if all players chose a different character
            List<NetworkManager.PlayerType> tempList = (List<NetworkManager.PlayerType>)NetworkManager.Instance.PlayersData.Values.Select(x => x.PlayerType);
            var tempHashSet = new HashSet<NetworkManager.PlayerType>();
            _startGameBtn.interactable = tempList.All(tempHashSet.Add);
        }
    }

    private void OnMatchResult(NetworkRunner runner)
    {
        ChangeCurrentCanvas(_selectCharacterUI);
        UpdateSelectPlayerUIInteractions();
        ClearFeedbackText();
    }

    private void UpdateSelectPlayerUIInteractions()
    {
        //for(int i = 0; i < _playerSelectionUIs.Length; i++)
        //{
        //    _playerSelectionUIs[i].SetIsInteractable(false);
        //}
        _playerSelectionUIs[NetworkManager.Instance.PlayersData.Keys.ToList().IndexOf(NetworkManager.Instance.NetworkRunner.LocalPlayer.PlayerId)].SetIsInteractable(true);
    }

    private void ClearFeedbackText()
    {
        _feedbackText.text = "";
    }
    #region Fusion Callbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            NetworkManager.Instance.PlayersData.Add(player.PlayerId, new NetworkManager.PlayerData(player, NetworkManager.PlayerType.Heavy));
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer && NetworkManager.Instance.PlayersData.ContainsKey(player.PlayerId))
        {
            NetworkManager.Instance.PlayersData.Remove(player.PlayerId);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
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
        if (runner.IsServer) NetworkManager.Instance.PlayersData.Clear();
        ChangeCurrentCanvas(_clientUI);
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