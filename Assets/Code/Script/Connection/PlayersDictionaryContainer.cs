using Fusion;

public static class PlayersDictionaryContainer
{
    public static NetworkDictionary<int, NetworkManager.PlayerData> PlayersData;

    static PlayersDictionaryContainer()
    {
        PlayersData = new NetworkDictionary<int, NetworkManager.PlayerData>();
    }
    /// <summary>
    /// Call this to start this class
    /// </summary>
    public static void StartClass()
    {

    }
}
