using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;
using ProjectMultiplayer.Connection;
using System;

namespace ProjectMultiplayer.UI
{
    public class UpdatePlayerSelectionScript : NetworkBehaviour
    {
        private MainScreen _mainScreen;
        [SerializeField] private CharacterSelection _characterSelection;
        [HideInInspector, Networked] public int RecentlyJoinedPlayer { get; set; }
        [/*HideInInspector,*/ Networked(OnChanged = nameof(OnPlayersSelectorUIDictionaryChanged), OnChangedTargets = OnChangedTargets.All), Capacity(NetworkManager.MaxPlayerCount)] public NetworkDictionary<int, int> PlayersSelectorUIDictionary => default;

        private void Awake()
        {
            _mainScreen = FindObjectOfType<MainScreen>();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void Rpc_UpdatePlayerTypeUI(int playerID, NetworkManager.PlayerType playerType)
        {
            Debug.Log($"the user {playerID} is changing to {playerType}");
            if (NetworkManagerReference.Instance.PlayersDictionary.ContainsKey(playerID))
            {
                //int index = NetworkManagerReference.Instance.PlayersData.Count - 1;
                //foreach (var values in NetworkManagerReference.Instance.PlayersData)
                //{
                //    if (values.Key == playerId) break;
                //    index--;
                //}
                NetworkManagerReference.Instance.PlayersDictionary.Set(playerID, new NetworkManager.PlayerData(NetworkManagerReference.Instance.PlayersDictionary[playerID].PlayerRef, playerType));
                //NetworkManagerReference.Instance.PlayersDictionary.Set(playerID, new NetworkManager.PlayerData(NetworkManagerReference.Instance.PlayersDictionary[playerID].PlayerRef, playerType, playerID));
                //_mainScreen.UpdateSelectedPlayerUiVisual(index, playerType);
            }
            else
            {
                Debug.LogWarning($"the user {playerID} is not in the dictionary");
            }
            //need to ask to Rogerio if there is a way to get the max player count from NetwrokConfigs
            if (NetworkManagerReference.Instance.NetworkRunner.IsServer)
            {
                //checks if all players chose a different character
                List<NetworkManager.PlayerType> tempList = new List<NetworkManager.PlayerType>();
                foreach (var playerData in NetworkManagerReference.Instance.PlayersDictionary)
                {
                    tempList.Add(playerData.Value.PlayerType);
                }
                _mainScreen.UpdateStartGameInteractableState(tempList.Distinct().Count() == tempList.Count && NetworkManagerReference.Instance.PlayersDictionary.Count == NetworkManager.MaxPlayerCount);
            }
        }

        //ASK ROGERIO ON HOW TO OPTIMIZE THIS
        private static void OnPlayersSelectorUIDictionaryChanged(Changed<UpdatePlayerSelectionScript> changed)
        {
            var newValue = changed.Behaviour.PlayersSelectorUIDictionary;
            changed.LoadOld();
            var oldValue = changed.Behaviour.PlayersSelectorUIDictionary;
            if(oldValue.Count > newValue.Count)
            {
                List<int> playersIDsThatNoLongerExists = new List<int>();
                foreach(var playerID in newValue)
                {
                    oldValue.Remove(playerID.Key);
                }
                foreach(var selectorUiID in oldValue)
                {
                    playersIDsThatNoLongerExists.Add(selectorUiID.Value);
                }
                changed.Behaviour.OnPlayersSelectorUIDictionaryChanged(playersIDsThatNoLongerExists);
            }
        }

        private void OnPlayersSelectorUIDictionaryChanged(List<int> playersIDsThatNoLongerExists)
        {
            for(int i = 0; i < playersIDsThatNoLongerExists.Count; i++)
            {
                _characterSelection.DeactivatePlayerSelectorVisual(playersIDsThatNoLongerExists[i]);
            }
        }
    }
}