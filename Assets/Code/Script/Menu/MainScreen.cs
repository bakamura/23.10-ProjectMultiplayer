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
using System.Threading.Tasks;
using ProjectMultiplayer.Connection;

namespace ProjectMultiplayer.UI
{
    public class MainScreen : Menu, INetworkRunnerCallbacks
    {
        [Header("General Components")]
        [SerializeField] private CanvasGroup _selectCharacterUI;
        [SerializeField] private CharacterSelection _characterSelectionUI;
        [SerializeField] private UpdatePlayerSelectionScript _updatePlayerSelectionScript;
        [SerializeField] private TMP_Text _serverNameText;

        [Header("Server Components")]
        [SerializeField] private TMP_InputField _serverInputField;
        [SerializeField] private UnityEngine.UI.Button _startGameBtn;

        [Header("Client Components")]
        [SerializeField] private TMP_InputField _clientInputField;
        [SerializeField] private CanvasGroup _clientUI;

        [Header("Debug")]
        [SerializeField] private Text _feedbackText;

        private bool _updatePlayerDataDictionary;
        private bool _alreadySavedServerIDLocaly;
        List<NetworkManager.PlayerData> _playerDataCacheToAdd = new List<NetworkManager.PlayerData>();
        List<NetworkManager.PlayerData> _playerDataCacheToRemove = new List<NetworkManager.PlayerData>();
        private List<int> _currentlyAvailableSelectorUIs = new List<int>();

        protected override void Awake()
        {
            base.Awake();            
            for(int i = 0; i < NetworkManager.MaxPlayerCount; i++)
            {
                _currentlyAvailableSelectorUIs.Add(i);
            }
        }

        private void Start()

        {
            NetworkManagerReference.Instance.AddCallbackToNetworkRunner(this);
            NetworkManagerReference.Instance.OnPlayersDataChangedCallback += UpdateSelectPlayerUI;
            NetworkManagerReference.Instance.OnFixedNetworkUpdate += UpdatePlayersDataDictionary;
        }

        private void OnEnable()
        {
            //InitializeInputPlayer.Instance.PlayerActions.UI.Cancel.performed += ReturnToPreviousCanvas;
        }

        private void OnDestroy()
        {
            //InitializeInputPlayer.Instance.PlayerActions.UI.Cancel.performed -= ReturnToPreviousCanvas;
            NetworkManagerReference.Instance.OnPlayersDataChangedCallback -= UpdateSelectPlayerUI;
            NetworkManagerReference.Instance.OnFixedNetworkUpdate -= UpdatePlayersDataDictionary;
            NetworkManagerReference.Instance.RemoveCallbackToNetworkRunner(this);
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

        public async void JoinMatch()
        {
            if (!string.IsNullOrEmpty(_clientInputField.text))
            {
                _feedbackText.text = "JOINING MATCH...";
                _currentCanvasOpened.Peek().interactable = false;
                var task = await NetworkManagerReference.Instance.JoinMacth(_clientInputField.text, OnMatchResult);
                if (!task.Ok)
                {
                    _feedbackText.text = $"FAIELD TO JOIN MACTH, REASON: {task.ShutdownReason}";
                    _currentCanvasOpened.Peek().interactable = true;
                }
            }
            else
            {
                _feedbackText.text = "INSERT A SESSION NAME TO LOOK FOR";
            }
        }

        public async void CreateMatch()
        {
            if (!string.IsNullOrEmpty(_serverInputField.text))
            {
                _feedbackText.text = "CREATING MATCH...";
                _currentCanvasOpened.Peek().interactable = false;
                var task = await NetworkManagerReference.Instance.CreateMatch(_serverInputField.text, SceneManager.GetActiveScene().buildIndex, OnMatchResult);
                if (!task.Ok)
                {
                    _feedbackText.text = $"FAIELD TO CREATE MACTH, REASON: {task.ShutdownReason}";
                    _currentCanvasOpened.Peek().interactable = true;
                }
            }
            else
            {
                _feedbackText.text = "INSERT A NAME FOR THE SESSION";
            }
        }

        public void StartMatch()
        {
            //TO DO get the chose level from server to open
            NetworkManagerReference.Instance.NetworkRunner.SetActiveScene("MatchTestScene");
        }

        /// <summary>
        /// updates the CharacterSelectionUI visuals
        /// </summary>
        /// <param name="index"></param>
        /// <param name="playerType"></param>
        //private void UpdateSelectedPlayerUiVisual(int index, NetworkManager.PlayerType playerType)
        //{
        //    _playerSelectionUIs[index].UpdateSelectedPlayer(playerType);
        //}
        /// <summary>
        /// activates/deactivates the start game buttton
        /// </summary>
        /// <param name="isInteractable"></param>
        public void UpdateStartGameInteractableState(bool isInteractable)
        {
            _startGameBtn.interactable = isInteractable;
        }
        /// <summary>
        /// callback when Photon sucsessfully started the lobby
        /// </summary>
        /// <param name="runner"></param>
        private void OnMatchResult(NetworkRunner runner)
        {
            _currentCanvasOpened.Peek().interactable = true;
            _serverNameText.text = $"Session Name: {runner.SessionInfo.Name}";
            ChangeCurrentCanvas(_selectCharacterUI);
            //UpdateSelectPlayerUI();
            ClearFeedbackText();
        }

        /// <summary>
        /// Makes the Local Player can interact only with its respective Character Selection UI and updates all Character Selection UIs visual
        /// </summary>
        private void UpdateSelectPlayerUI()
        {
            if (!_alreadySavedServerIDLocaly)
            {
                NetworkManagerReference.LocalPlayerIDInServer = _updatePlayerSelectionScript.RecentlyJoinedPlayer;
                _alreadySavedServerIDLocaly = true;
            }

            //updates visual for all UIs
            foreach (var player in NetworkManagerReference.Instance.PlayersDictionary)
            {
                _characterSelectionUI.ActivatePlayerSelectorVisual(_updatePlayerSelectionScript.PlayersSelectorUIDictionary[player.Key],
                    NetworkManagerReference.Instance.PlayersDictionary[player.Key].PlayerType,
                    player.Key == NetworkManagerReference.LocalPlayerIDInServer);
            }
            #region Old Version
            ////NetworkManagerReference.Instance.SavePlayerServerIDToLocalPlayer();
            ////Enables Interaction of local player to its respective SelectCharacter UI 
            //if (!_alreadyAssignedWithCharacterUI)
            //{
            //    _characterSelectionUIs[_updatePlayerSelectionScript.RecentlyJoinedPlayer.PlayerId].SetIsInteractable(true);
            //    NetworkManagerReference.LocalPlayerIDInServer = _updatePlayerSelectionScript.RecentlyJoinedPlayer.PlayerId;
            //    _alreadyAssignedWithCharacterUI = true;
            //    //foreach (var player in NetworkManagerReference.Instance.PlayersDictionary)
            //    //{
            //    //    if (!_updatePlayerSelectionScript.RecentlyJoinedPlayer.Contains(player.Key))
            //    //    {
            //    //        Debug.Log("found the new");
            //    //        _characterSelectionUIs[player.Key.PlayerId].SetIsInteractable(true);
            //    //        NetworkManagerReference.LocalPlayerIDInServer = player.Key.PlayerId;
            //    //        _alreadyAssignedWithCharacterUI = true;
            //    //        willUpdatePreviousPlayersInDictionary = true;
            //    //        break;
            //    //    }
            //    //}
            //}

            ////updates visual for all UIs
            //foreach (var player in NetworkManagerReference.Instance.PlayersDictionary)
            //{
            //    //Debug.Log($"the user {values.Key} is now {values.Value.PlayerType}");
            //    _characterSelectionUIs[player.Key.PlayerId].UpdateSelectedPlayer(player.Value.PlayerType);
            //}
            ////Debug.Log($"new previous size is {_updatePlayerSelectionScript.PreviousPlayersInDictionary.Count}");
            #endregion
        }

        //[ContextMenu("PrintPlayers")]
        //private void PrintPlayers()
        //{
        //    foreach (var values in NetworkManagerReference.Instance.PlayersDictionary)
        //    {
        //        Debug.Log($"the player id is {values.Key.PlayerId}");
        //    }
        //}

        private void UpdatePlayersDataDictionary()
        {
            if (_updatePlayerDataDictionary)
            {
                for (int i = 0; i < _playerDataCacheToAdd.Count; i++)
                {
                    //Debug.Log("addning new player to Dictionary");
                    //if (NetworkManagerReference.Instance.PlayersData.ContainsKey(_playerDataCacheToAdd[i].PlayerRef))
                    //{
                    //    Debug.Log($"the user that just joined is already in the dictionary");
                    //}
                    NetworkManagerReference.Instance.PlayersDictionary.Add(_playerDataCacheToAdd[i].PlayerRef.PlayerId, _playerDataCacheToAdd[i]);
                }
                for (int i = 0; i < _playerDataCacheToRemove.Count; i++)
                {
                    NetworkManagerReference.Instance.PlayersDictionary.Remove(_playerDataCacheToRemove[i].PlayerRef.PlayerId);
                }
                _playerDataCacheToAdd.Clear();
                _playerDataCacheToRemove.Clear();
                _updatePlayerDataDictionary = false;
            }
        }

        private void ClearFeedbackText()
        {
            _feedbackText.text = "";
        }

        #region Fusion Callbacks
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"the player joined has the ID off {player.PlayerId}");
            if (runner.IsServer)
            {
                _updatePlayerSelectionScript.RecentlyJoinedPlayer = player.PlayerId;
                //Debug.Log($"the player that joined is {player.PlayerId}");
                _updatePlayerDataDictionary = true;
                _playerDataCacheToAdd.Add(new NetworkManager.PlayerData(player, NetworkManager.PlayerType.Heavy/*, player.PlayerId*/));
                _updatePlayerSelectionScript.PlayersSelectorUIDictionary.Add(player.PlayerId, _currentlyAvailableSelectorUIs[0]);
                _currentlyAvailableSelectorUIs.RemoveAt(0);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer && NetworkManagerReference.Instance.PlayersDictionary.ContainsKey(player.PlayerId))
            {
                _updatePlayerDataDictionary = true;
                _playerDataCacheToRemove.Add(new NetworkManager.PlayerData(player, NetworkManager.PlayerType.Heavy/*, player.PlayerId*/));
                _currentlyAvailableSelectorUIs.Add(_updatePlayerSelectionScript.PlayersSelectorUIDictionary[player.PlayerId]);
                _updatePlayerSelectionScript.PlayersSelectorUIDictionary.Remove(player.PlayerId);
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
            NetworkManagerReference.Instance.PlayersDictionary.Clear();
            ReturnToDefaultUI();
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {

        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            if (runner.IsServer) NetworkManagerReference.Instance.PlayersDictionary.Clear();
            ReturnToDefaultUI();
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