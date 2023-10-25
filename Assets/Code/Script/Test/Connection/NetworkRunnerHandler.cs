using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;

public class NetworkRunnerHandler : MonoBehaviour
{
    [SerializeField] private GameObject _networkSetupPrefab;
    private static NetworkRunner _networkRunner;
    private static NetworkSceneManagerDefault _networkSceneManager;
    private static List<INetworkRunnerCallbacks> _callbacksRequested = new List<INetworkRunnerCallbacks>();
    private static Queue<INetworkRunnerCallbacks> _requestedCallbacks =  new Queue<INetworkRunnerCallbacks>();
    private static bool _gameStarted;
    public static NetworkRunner NetworkRunner => _networkRunner;
    public static bool GameStarted => _gameStarted;
    public static List<PlayerRef> PlayersRefs =  new List<PlayerRef>();
    public NetworkSceneManagerDefault NetworkSceneManager => _networkSceneManager;
    
    private void Awake()
    {
        if (!_networkRunner)
        {
            GameObject temp = Instantiate(_networkSetupPrefab, null);
            _networkRunner = temp.GetComponent<NetworkRunner>();
            _networkSceneManager = temp.GetComponent<NetworkSceneManagerDefault>();
        }
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

    protected async Task<StartGameResult> InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress netAddress, SceneRef sceneRef, string SessionName, Action<NetworkRunner> initialized)
    {
        //sempre enviar a cena correta q é para todos os jogadores estarem        
        runner.ProvideInput = true;
        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = netAddress,
            Scene = sceneRef,
            SessionName = SessionName,
            Initialized = initialized,
            CustomLobbyName = SessionName,
            SceneManager = _networkSceneManager
        });
        if (result.Ok)
        {
            Debug.Log("Start Game");
            return result;
        }
        else
        {
            Debug.LogError($"Faield to start game {result.ShutdownReason}");
            return null;
        }
        //return runner.StartGame(new StartGameArgs
        //{
        //    GameMode = gameMode,
        //    Address = netAddress,
        //    Scene = sceneRef,
        //    //SessionName = SessionName,
        //    Initialized = initialized,
        //    CustomLobbyName = SessionName,
        //    SceneManager = _networkSceneManager
        //});
    }
    protected async Task<StartGameResult> InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, string SessionName)
    {
        //sempre enviar a cena correta q é para todos os jogadores estarem        
        runner.ProvideInput = true;
        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            SessionName = SessionName,
            SceneManager = _networkSceneManager
        });
        if (result.Ok)
        {
            Debug.Log("Start Game");
            return result;
        }
        else
        {
            Debug.LogError($"Faield to start game {result.ShutdownReason}");
            return null;
        }
        //return runner.StartGame(new StartGameArgs
        //{
        //    GameMode = gameMode,
        //    Address = netAddress,
        //    Scene = sceneRef,
        //    //SessionName = SessionName,
        //    Initialized = initialized,
        //    CustomLobbyName = SessionName,
        //    SceneManager = _networkSceneManager
        //});
    }

    public async Task CreateMatch(string sessionName, SceneRef sceneRef)
    {
        var result = await InitializeNetworkRunner(_networkRunner, GameMode.Host, NetAddress.Any(), sceneRef, sessionName, null);
        if (result.Ok)
        {
            Debug.Log("Start Game");
        }
        else
        {
            Debug.LogError($"Faield to start game {result.ShutdownReason}");
        }
    }

    public async Task JoinMacth(SessionInfo info)
    {
        //para os clientes n é necessário ter o id da cena correta pois eles sempre irão para a cena em q o servdor estiver
        var result = await InitializeNetworkRunner(_networkRunner, GameMode.Client, info.Name);
        if (result.Ok)
        {
            Debug.Log("Start Game");
        }
        else
        {
            Debug.LogError($"Faield to start game {result.ShutdownReason}");
        }
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
