using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;

public class NetworkManager : NetworkBehaviour
{
    //private static NetworkManager _instance;
    //public static NetworkManager Instance
    //{
    //    get
    //    {
    //        // se ainda n tiver uma referência da instancia, procura ela no GameObject
    //        if (_instance == null)
    //        {
    //            NetworkManager[] results = GameObject.FindObjectsOfType<NetworkManager>();
    //            if (results.Length > 0)
    //            {
    //                if(results.Length > 1) Debug.Log($"Multiple Instances of NetworkManager found, destroing extras");
    //                for (int i = 1; i < results.Length; i++)
    //                {
    //                    Destroy(results[i]);
    //                }
    //                _instance = results[0];
    //            }
    //        }
    //        // se ainda n tiver uma referência da instancia, cria uma do tipo desejado
    //        //if (_instance == null)
    //        //    _instance = new GameObject($"Instance of Type: {typeof(NetworkManager)}").AddComponent<NetworkManager>();
    //        return _instance;
    //    }
    //}
    [SerializeField] private GameObject _networkSetupPrefab;
    [SerializeField] private NetworkRunner _networkRunner;
    [SerializeField] private NetworkSceneManagerDefault _networkSceneManager;
    private List<INetworkRunnerCallbacks> _callbacksRequested = new List<INetworkRunnerCallbacks>();
    private Queue<INetworkRunnerCallbacks> _requestedCallbacks = new Queue<INetworkRunnerCallbacks>();

    public const byte MaxPlayerCount = 3;
    [Networked(OnChanged = nameof(OnPlayersDataChanged), OnChangedTargets = OnChangedTargets.InputAuthority), Capacity(MaxPlayerCount)] public NetworkDictionary<int, PlayerData> PlayersData => default;
    public NetworkSceneManagerDefault NetworkSceneManager => _networkSceneManager;
    public NetworkRunner NetworkRunner => _networkRunner;
    public Action OnPlayersDataChangedCallback;
    public enum PlayerType
    {
        Heavy,
        Springer,
        Navi
    }
    [Serializable]
    public struct PlayerData
    {
        public PlayerRef PlayerRef;
        public PlayerType PlayerType;

        public PlayerData(PlayerRef playerRef, PlayerType playerType)
        {
            PlayerRef = playerRef;
            PlayerType = playerType;
        }
    }

    public override void Spawned()
    {
        base.Spawned();
        if (!_networkRunner)
        {
            GameObject temp = Instantiate(_networkSetupPrefab, null);
            _networkRunner = temp.GetComponent<NetworkRunner>();
            _networkSceneManager = temp.GetComponent<NetworkSceneManagerDefault>();
        }
        UpdateCallbacks();
    }

    //private void Awake()
    //{
    //    if (!_networkRunner)
    //    {
    //        GameObject temp = Instantiate(_networkSetupPrefab, null);
    //        _networkRunner = temp.GetComponent<NetworkRunner>();
    //        _networkSceneManager = temp.GetComponent<NetworkSceneManagerDefault>();
    //    }
    //    //if (_instance != this)
    //    //{
    //    //    Destroy(this);
    //    //    return;
    //    //}
    //}
    //private void Start()
    //{
    //    UpdateCallbacks();
    //}

    /// <summary>
    /// add a script with INetworkRunnerCallbacks to have callbacks from NetworkRunner, add it in the Start method. Remember to call RemoveCallbackToNetworkRunner once it is not necessary anymore
    /// </summary>
    /// <param name="request"></param>
    public void AddCallbackToNetworkRunner(INetworkRunnerCallbacks request)
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

    public void RemoveCallbackToNetworkRunner(INetworkRunnerCallbacks request)
    {
        if (_callbacksRequested.Contains(request))
        {
            _callbacksRequested.Remove(request);
            _networkRunner.RemoveCallbacks(request);
        }
    }
    private void UpdateCallbacks()
    {
        int currentSize = _requestedCallbacks.Count;
        for (int i = 0; i < currentSize; i++)
        {
            _networkRunner.AddCallbacks(_requestedCallbacks.Dequeue());
        }
    }

    protected async Task<StartGameResult> InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress netAddress, SceneRef sceneRef, string SessionName, Action<NetworkRunner> initialized)
    {
        runner.ProvideInput = true;
        var task = await runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = netAddress,
            Scene = sceneRef,
            SessionName = SessionName,
            Initialized = initialized,
            CustomLobbyName = SessionName,
            SceneManager = _networkSceneManager
        });
        if (!task.Ok)
        {
            Debug.LogError($"Erron on Initializing Game Match, Reason: {task.ShutdownReason}");
        }
        return task;
        //return runner.StartGame(new StartGameArgs
        //{
        //    GameMode = gameMode,
        //    Address = netAddress,
        //    Scene = sceneRef,
        //    SessionName = SessionName,
        //    Initialized = initialized,
        //    CustomLobbyName = SessionName,
        //    SceneManager = _networkSceneManager
        //});
    }

    public async void CreateMatch(string sessionName, SceneRef sceneRef, Action<NetworkRunner> OnMatchCreated = null)
    {
        var task = await InitializeNetworkRunner(_networkRunner, GameMode.Host, NetAddress.Any(), sceneRef, sessionName, OnMatchCreated);
    }

    //public void JoinMacth(SessionInfo info)
    //{
    //    //para os clientes n é necessário ter o id da cena correta pois eles sempre irão para a cena em q o servdor estiver
    //    InitializeNetworkRunner(_networkRunner, GameMode.Client, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, info.Name, null);
    //}

    public async void JoinMacth(string sessionName, Action<NetworkRunner> OnMatchCreated = null)
    {
        var task = await InitializeNetworkRunner(_networkRunner, GameMode.Client, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, sessionName, OnMatchCreated);
    }

    public static void OnPlayersDataChanged(Changed<NetworkManager> changed)
    {
        changed.Behaviour.OnPlayersDataChanged();
    }

    private void OnPlayersDataChanged()
    {
        OnPlayersDataChangedCallback?.Invoke();
    }
    //public bool JoinLobby(string sessionName)
    //{
    //    return JoinLobbyTask(sessionName) != null;
    //}

    //private async Task JoinLobbyTask(string sessionName)
    //{
    //    StartGameResult operation = await _networkRunner.JoinSessionLobby(SessionLobby.Custom, sessionName);

    //    if (!operation.Ok)
    //    {
    //        Debug.LogError($"Not possible to Join loby {sessionName}");
    //        operation = null;
    //    }
    //}
}