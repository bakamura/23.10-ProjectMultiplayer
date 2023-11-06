using System.Collections.Generic;
using Fusion;

namespace ProjectMultiplayer.Connection
{
    public static class PlayersDictionaryContainer
    {
        public static Dictionary<PlayerRef, NetworkManager.PlayerData> PlayersData;

        static PlayersDictionaryContainer()
        {
            PlayersData = new Dictionary<PlayerRef, NetworkManager.PlayerData>();
        }
        /// <summary>
        /// Call this to start the dictionary
        /// </summary>
        public static void StartDictionary()
        {
            //bool isNull = true;
            //foreach (var value in PlayersData)
            //{
            //    isNull = false;
            //    break;
            //}
            //if (isNull)
            //{
            //    PlayersData = new NetworkDictionary<PlayerRef, NetworkManager.PlayerData>();
            //}
        }

        /// <summary>
        /// The dictionary from photon cant verify if its null, use this to verify
        /// </summary>
        /// <returns></returns>
        public static bool CanReadFromDictionary()
        {
            foreach (var value in PlayersData)
            {
                return true;
            }
            return false;
        }
    }
}