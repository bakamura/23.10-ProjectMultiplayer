using ProjectMultiplayer.Connection;
using ProjectMultiplayer.Player;
using System.Collections;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    [Header("Parameters")]

    //[SerializeField] private float _sensitivityX;
    //[SerializeField] private float _sensitivityY;
    [SerializeField] private Vector3 _followTargetOffset;

    [Header("Cache")]

    private Transform _playerTarget;
    private Transform _followTarget;
    //private Vector2 _deltaInputCache;
    //private float _deltaInputCacheUnit;

#if UNITY_EDITOR
    [Header("Debug")]

    [SerializeField] private bool _debugLogs;
#endif

    //private void Start() {
    //    StartCoroutine(KeepTryingGetPlayer());
    //}

    private void Awake()
    {
        _followTarget = new GameObject("CameraTracker").transform;
        Cinemachine.CinemachineFreeLook temp = GetComponent<Cinemachine.CinemachineFreeLook>();
        temp.Follow = _followTarget;
        temp.LookAt = _followTarget;
        _playerTarget = transform.parent;
    }

    private void LateUpdate()
    {
        _followTarget.position = _playerTarget.position + _followTargetOffset;
    }

    //private void Update() {
    //_followTarget.position = _playerTarget.position;
    //_deltaInputCache = InitializeInputPlayer.Instance.PlayerActions.actions["MoveCamera"].ReadValue<Vector2>();
    //_deltaInputCacheUnit = _deltaInputCache.x * _sensitivityX;
    //_deltaInputCache[0] = _deltaInputCache.y * _sensitivityY;
    //_deltaInputCache[1] = _deltaInputCacheUnit;
    //_followTarget.position = _playerTarget.position + _followTargetOffset;
    //_followTarget.eulerAngles += (Vector3)_deltaInputCache;
    //}

//    private IEnumerator KeepTryingGetPlayer() {
//        _followTarget = new GameObject("CameraTracker").transform;
//        GetComponent<Cinemachine.CinemachineFreeLook>().Follow = _followTarget;

//        while (TryGetPlayer()) { yield return null; }
//    }

//    private bool TryGetPlayer() {
//#if UNITY_EDITOR
//        if (_debugLogs) Debug.Log("Camera trying to find active Player");
//#endif
//        Player[] players = FindObjectsOfType<Player>();
//        if (players != null) {
//            foreach (Player player in players)
//                if (player.Type == NetworkManagerReference.Instance.PlayersDictionary[NetworkManagerReference.LocalPlayerIDInServer].PlayerType) {
//                    _playerTarget = player.transform;
//                    _followTarget.position = _playerTarget.position + _followTargetOffset;
//#if UNITY_EDITOR
//                    if (_debugLogs) Debug.Log("Camera found active Player");
//#endif
//                    return false; // Found
//                }
//        }
//        return true; // Keep trying
//    }

//#if UNITY_EDITOR
//    public void ResetCameraTracking(Transform playerTransform)
//    {
//        _followTarget.parent = playerTransform;
//        _followTarget.localPosition = _followTargetOffset;
//    }
//#endif

}
