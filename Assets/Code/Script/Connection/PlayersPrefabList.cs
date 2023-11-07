using UnityEngine;
using ProjectMultiplayer.Connection;

using ProjectMultiplayer.Connection;

[CreateAssetMenu(fileName = "PlayersPrefabList", menuName = "Data/PlayerPrefabList")]
public class PlayersPrefabList : ScriptableObject
{
    public PlayerPrefabData[] PlayersPrefab;
    [System.Serializable]
    public struct PlayerPrefabData
    {
        public NetworkManager.PlayerType PlayerType;
        public GameObject Prefab;
    }

    public GameObject GetPlayerPrefab(NetworkManager.PlayerType playerType)
    {
        for (int i = 0; i < PlayersPrefab.Length; i++)
        {
            if (PlayersPrefab[i].PlayerType == playerType)
                return PlayersPrefab[i].Prefab;
        }
        Debug.LogError($"the is no prefab assigned to the type {playerType}");
        return null;
    }
}
