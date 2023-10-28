using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class UpdatePlayerSelectionScript : NetworkBehaviour
{
    private MainScreen _mainScreen;
    public override void Spawned()
    {
        base.Spawned();
        _mainScreen = FindObjectOfType<MainScreen>();
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.InputAuthority)]
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
            
            if(Object.HasStateAuthority) NetworkManagerReference.Instance.PlayersData.Add(playerId, new NetworkManager.PlayerData(NetworkManagerReference.Instance.PlayersData[playerId].PlayerRef, playerType));
            _mainScreen.UpdateSelectedPlayer(index, playerType);
        }
        //need to ask to Rogerio if there is a way to get the max player count from NetwrokConfigs
        if (NetworkManagerReference.Instance.NetworkRunner.IsServer)
        {
            //checks if all players chose a different character
            List<NetworkManager.PlayerType> tempList = (List<NetworkManager.PlayerType>)NetworkManagerReference.Instance.PlayersData.Select(x => x.Value.PlayerType);
            var tempHashSet = new HashSet<NetworkManager.PlayerType>();
            _mainScreen.UpdateStartGameInteractableState(tempList.All(tempHashSet.Add) && NetworkManagerReference.Instance.PlayersData.Count == NetworkManager.MaxPlayerCount);
        }
    }
}
