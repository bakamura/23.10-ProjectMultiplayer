using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class UpdatePlayerSelectionScript : NetworkBehaviour
{
    private MainScreen _mainScreen;

    private void Awake()
    {
        _mainScreen = FindObjectOfType<MainScreen>();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_UpdatePlayerTypeUI(int playerId, NetworkManager.PlayerType playerType)
    {
        if (NetworkManagerReference.Instance.PlayersData.ContainsKey(playerId))
        {
            int index = 0;
            foreach(var values in NetworkManagerReference.Instance.PlayersData)
            {
                if (values.Key == playerId) break;
                index++;
            }
            
            if(Object.HasStateAuthority) NetworkManagerReference.Instance.PlayersData.Set(playerId, new NetworkManager.PlayerData(NetworkManagerReference.Instance.PlayersData[playerId].PlayerRef, playerType));
            //_mainScreen.UpdateSelectedPlayerUiVisual(index, playerType);
        }
        //need to ask to Rogerio if there is a way to get the max player count from NetwrokConfigs
        if (NetworkManagerReference.Instance.NetworkRunner.IsServer)
        {
            //checks if all players chose a different character
            List<NetworkManager.PlayerType> tempList = new List<NetworkManager.PlayerType>();
            foreach (var playerData in NetworkManagerReference.Instance.PlayersData)
            {
                tempList.Add(playerData.Value.PlayerType);
            }
            _mainScreen.UpdateStartGameInteractableState(tempList.Distinct().Count() == tempList.Count && NetworkManagerReference.Instance.PlayersData.Count == NetworkManager.MaxPlayerCount);
        }
    }
}
