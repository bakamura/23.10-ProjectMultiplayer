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
    //SceneUtility.GetBuildIndexByScenePath($"Assets/Game/Scene/{sceneName}");
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

    public static void RemoveCallbackToNetworkRunner(INetworkRunnerCallbacks request)
    {
        if (_callbacksRequested.Contains(request))
        {
            _callbacksRequested.Remove(request);
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
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress netAddress, SceneRef sceneRef, string SessionName, Action<NetworkRunner> initialized)
    {
        //sempre enviar a cena correta q é para todos os jogadores estarem
        INetworkSceneManager sceneManager = _networkSceneManager;
        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = netAddress,
            Scene = sceneRef,
            SessionName = SessionName,
            Initialized = initialized,
            SceneManager = sceneManager,
        });
    }   

    public void CreateMatch(string sessionName, SceneRef sceneRef)
    {
        InitializeNetworkRunner(_networkRunner, GameMode.Host, NetAddress.Any(), sceneRef, sessionName, null);
    }

    public void JoinMacth(SessionInfo info)
    {        
        //para os clientes n é necessário ter o id da cena correta pois eles sempre irão para a cena em q o servdor estiver
        InitializeNetworkRunner(_networkRunner, GameMode.Client, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, info.Name, null);
    }

    public bool JoinLobby(string sessionName)
    {
       return JoinLobbyTask(sessionName) != null;
    }

    private async Task JoinLobbyTask(string sessionName)
    {
        StartGameResult operation = await _networkRunner.JoinSessionLobby(SessionLobby.Custom, sessionName);

        if (!operation.Ok)
        {
            Debug.LogError($"Not possible to Join loby {sessionName}");
            operation = null;
        }
    }
}
