using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectMultiplayer.Connection
{
    public class NetworkManagerReference : MonoBehaviour
    {
        public static int LocalPlayerIDInServer;
        //public static bool AlreadySavedId;

        private static NetworkManager _instance;
        //private static List<byte> _playerKeysAlreadyUsed = new List<byte>();
        //private static System.Random _randomGenerator = new System.Random();
        //private static byte[] _byteArrayCached = new byte[1];

        public static NetworkManager Instance
        {
            get
            {
                // se ainda n tiver uma referência da instancia, procura ela no GameObject
                if (_instance == null)
                {
                    NetworkManager[] results = GameObject.FindObjectsOfType<NetworkManager>();
                    if (results.Length > 0)
                    {
                        if (results.Length > 1) Debug.Log($"Multiple Instances of {typeof(NetworkManagerReference).Name} found, destroing extras");
                        for (int i = 1; i < results.Length; i++)
                        {
                            Destroy(results[i]);
                        }
                        _instance = results[0];
                        //_instance.Spawned();
                    }
                }
                // se ainda n tiver uma referência da instancia, cria uma do tipo desejado
                //if (_instance == null)
                //{
                //    _instance = new GameObject($"Instance of Type: {typeof(NetworkManager)}").AddComponent<NetworkManager>();
                //    _instance.Spawned();
                //}
                return _instance;
            }
        }

        //public static byte GenerateRandomPlayerID()
        //{
        //    _randomGenerator.NextBytes(_byteArrayCached);
        //    while (_playerKeysAlreadyUsed.Contains(_byteArrayCached[0]))
        //    {
        //        _randomGenerator.NextBytes(_byteArrayCached);
        //    }
        //    _playerKeysAlreadyUsed.Add(_byteArrayCached[0]);
        //    return _byteArrayCached[0];
        //}

        //public static void RemoveKeyFromList(byte key)
        //{
        //    _playerKeysAlreadyUsed.Remove(key);
        //}
    }
}