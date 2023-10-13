using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;

[RequireComponent(typeof(NetworkRunner), typeof(NetworkSceneManagerDefault))]
public class NetworkRunnerHandler : MonoBehaviour
{
    private static NetworkRunner _networkRunner;
    private NetworkSceneManagerDefault _networkSceneManager;
    private static List<INetworkRunnerCallbacks> _callbacksRequested = new List<INetworkRunnerCallbacks>();
    private static Queue<INetworkRunnerCallbacks> _requestedCallbacks =  new Queue<INetworkRunnerCallbacks>();
    public static NetworkRunner NetworkRunner => _networkRunner;

    private void Awake()
    {
        _networkRunner = GetComponent<NetworkRunner>();
        _networkSceneManager = GetComponent<NetworkSceneManagerDefault>();
    }

    public static void AddCallbackToNetworkRunner(INetworkRunnerCallbacks request)
    {
        if (!_callbacksRequested.Contains(request))
        {
            _requestedCallbacks.Enqueue(request);
            _callbacksRequested.Add(request);
            if (_networkRunner)
            {
                UpdateCallbacks();
            }
        }
        else
        {
            Debug.Log($"the requested object is already in the callback list");
        }
    }

    private static void UpdateCallbacks()
    {
        int currentSize = _requestedCallbacks.Count;
        for (int i = 0; i < currentSize; i++)
        {
            _networkRunner.AddCallbacks(_requestedCallbacks.Dequeue());
        }
    }

    void Start()
    {
        UpdateCallbacks();
        Task clientTask = InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
        //_networkRunner = Instantiate(_networkRunnerPrefab);
        //_networkRunner.name = "Network Runner";


        //Debug.Log("Server Netwrok Runner Started");
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress netAddress, SceneRef sceneRef, Action<NetworkRunner> initialized)
    {
        //INetworkSceneManager sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        //if (sceneManager == null)
        //{
        //    sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        //}
        INetworkSceneManager sceneManager = _networkSceneManager;
        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = netAddress,
            Scene = sceneRef,
            SessionName = "TestRoom",
            Initialized = initialized,
            SceneManager = sceneManager
        });
    }
}
