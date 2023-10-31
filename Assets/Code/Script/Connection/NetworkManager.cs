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
    [SerializeField] private GameObject _networkSetupPrefab;
    [SerializeField] private NetworkRunner _networkRunner;
    [SerializeField] private NetworkSceneManagerDefault _networkSceneManager;
    private List<INetworkRunnerCallbacks> _callbacksRequested = new List<INetworkRunnerCallbacks>();
    private Queue<INetworkRunnerCallbacks> _requestedCallbacks = new Queue<INetworkRunnerCallbacks>();

    public const byte MaxPlayerCount = 3;
    [Networked(OnChanged = nameof(OnPlayersDataChanged), OnChangedTargets = OnChangedTargets.All), Capacity(MaxPlayerCount)] public NetworkDictionary<int, PlayerData> PlayersData => default;
    public NetworkSceneManagerDefault NetworkSceneManager
    {
        get
        {
            // se ainda n tiver uma refer�ncia da instancia, procura ela no GameObject
            if (_networkSceneManager == null)
            {
                NetworkSceneManagerDefault[] results = GameObject.FindObjectsOfType<NetworkSceneManagerDefault>();
                if (results.Length > 0)
                {
                    if (results.Length > 1) Debug.Log($"Multiple Instances of NetworkManager found, destroing extras");
                    for (int i = 1; i < results.Length; i++)
                    {
                        Destroy(results[i]);
                    }
                    _networkSceneManager = results[0];
                }
            }
            // se ainda n tiver uma refer�ncia da instancia, cria uma do tipo desejado
            //if (_instance == null)
            //    _instance = new GameObject($"Instance of Type: {typeof(NetworkManager)}").AddComponent<NetworkManager>();
            return _networkSceneManager;
        }
    }
    public NetworkRunner NetworkRunner
    {
        get
        {
            // se ainda n tiver uma refer�ncia da instancia, procura ela no GameObject
            if (_networkRunner == null)
            {
                NetworkRunner[] results = GameObject.FindObjectsOfType<NetworkRunner>();
                if (results.Length > 0)
                {
                    if (results.Length > 1) Debug.Log($"Multiple Instances of NetworkManager found, destroing extras");
                    for (int i = 1; i < results.Length; i++)
                    {
                        Destroy(results[i]);
                    }
                    _networkRunner = results[0];
                }
            }
            // se ainda n tiver uma refer�ncia da instancia, cria uma do tipo desejado
            //if (_instance == null)
            //    _instance = new GameObject($"Instance of Type: {typeof(NetworkManager)}").AddComponent<NetworkManager>();
            return _networkRunner;
        }
    }
    public Action OnPlayersDataChangedCallback;
    public Action OnFixedNetworkUpdate;
    private bool _transferedDataFromStaticDictionary;
    public enum PlayerType
    {
        Heavy,
        Springer,
        Navi
    }
    [Serializable]
    public struct PlayerData : INetworkStruct
    {
        public PlayerRef PlayerRef;
        public PlayerType PlayerType;

        public PlayerData(PlayerRef playerRef, PlayerType playerType)
        {
            PlayerRef = playerRef;
            PlayerType = playerType;
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        foreach (var value in PlayersData)
        {
            if (!PlayersDictionaryContainer.PlayersData.ContainsKey(value.Key))
                PlayersDictionaryContainer.PlayersData.Add(value.Key, value.Value);
            else
                PlayersDictionaryContainer.PlayersData[value.Key] = value.Value;
        }
    }

    public override void Spawned()
    {
        //if (!NetworkRunnerRef)
        //{
        //    GameObject temp = Instantiate(_networkSetupPrefab, null);
        //    NetworkRunnerRef = temp.GetComponent<NetworkRunner>();
        //    NetworkSceneManager = temp.GetComponent<NetworkSceneManagerDefault>();
        //}
        
        UpdateCallbacks();
    }

    //private void Awake()
    //{
    //    if (!NetworkRunnerRef)
    //    {
    //        GameObject temp = Instantiate(_networkSetupPrefab, null);
    //        NetworkRunnerRef = temp.GetComponent<NetworkRunner>();
    //        NetworkSceneManager = temp.GetComponent<NetworkSceneManagerDefault>();
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

    public override void FixedUpdateNetwork()
    {
        UpdatePlayersDictionary();
        OnFixedNetworkUpdate?.Invoke();
    }

    private void UpdatePlayersDictionary()
    {
        if (!_transferedDataFromStaticDictionary)
        {
            PlayersDictionaryContainer.StartClass();
            bool isEmpty = true;
            foreach (var value in PlayersDictionaryContainer.PlayersData)
            {
                isEmpty = false;
                break;
            }
            if (!isEmpty)
            {
                foreach (var value in PlayersDictionaryContainer.PlayersData)
                {
                    PlayersData.Add(value.Key, value.Value);
                }
            }
            _transferedDataFromStaticDictionary = true;
        }
    }
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
            if (NetworkRunner)
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
            SceneManager = NetworkSceneManager
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
        //    SceneManager = NetworkSceneManager
        //});
    }

    public async void CreateMatch(string sessionName, SceneRef sceneRef, Action<NetworkRunner> OnMatchCreated = null)
    {
        var task = await InitializeNetworkRunner(_networkRunner, GameMode.Host, NetAddress.Any(), sceneRef, sessionName, OnMatchCreated);
    }

    //public void JoinMacth(SessionInfo info)
    //{
    //    //para os clientes n � necess�rio ter o id da cena correta pois eles sempre ir�o para a cena em q o servdor estiver
    //    InitializeNetworkRunner(NetworkRunnerRef, GameMode.Client, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, info.Name, null);
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
    //    StartGameResult operation = await NetworkRunnerRef.JoinSessionLobby(SessionLobby.Custom, sessionName);

    //    if (!operation.Ok)
    //    {
    //        Debug.LogError($"Not possible to Join loby {sessionName}");
    //        operation = null;
    //    }
    //}
}