using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerReference : MonoBehaviour
{
    private static NetworkManager _instance;
    public static NetworkManager Instance
    {
        get
        {
            // se ainda n tiver uma referência da instancia, procura ela no GameObject
            if (_instance == null)
            {
                NetworkManager[] results = GameObject.FindObjectsOfType<NetworkManager>();
                if(results.Length > 0)
                {
                    if(results.Length > 1) Debug.Log($"Multiple Instances of {typeof(NetworkManagerReference).Name} found, destroing extras");
                    for(int i = 1; i < results.Length; i++)
                    {
                        Destroy(results[i]);
                    }
                    _instance = results[0];
                    _instance.Spawned();
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
}
