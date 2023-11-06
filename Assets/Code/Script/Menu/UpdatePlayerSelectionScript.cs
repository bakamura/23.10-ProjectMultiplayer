using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

namespace ProjectMultiplayer.UI
{
    public class UpdatePlayerSelectionScript : NetworkBehaviour
    {
        private MainScreen _mainScreen;
        [Networked] public PlayerRef RecentlyJoinedPlayer { get; set; }

        private void Awake()
        {
            _mainScreen = FindObjectOfType<MainScreen>();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void Rpc_UpdatePlayerTypeUI(PlayerRef playerID, NetworkManager.PlayerType playerType)
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
                NetworkManagerReference.Instance.PlayersDictionary.Set(playerID, new NetworkManager.PlayerData(playerID, playerType));
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
    }
}