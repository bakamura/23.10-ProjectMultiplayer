using ProjectMultiplayer.Connection;
using ProjectMultiplayer.Player;
using System.Collections;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    [Header("Parameters")]

    [SerializeField] private float _sensitivityX;
    [SerializeField] private float _sensitivityY;

    [Header("Cache")]

    private Transform _followTarget;
    private Vector2 _deltaInputCache;
    private float _deltaInputCacheUnit;

    private void Start() {
        StartCoroutine(KeepTryingGetPlayer());
    }

    private void Update() {
        _deltaInputCache = InitializeInputPlayer.Instance.PlayerActions.actions["MoveCamera"].ReadValue<Vector2>();
        _deltaInputCacheUnit = _deltaInputCache.x * _sensitivityX;
        _deltaInputCache[0] = _deltaInputCache.y * _sensitivityY;
        _deltaInputCache[1] = _deltaInputCacheUnit;
        _followTarget.eulerAngles += (Vector3)_deltaInputCache;
    }

    private IEnumerator KeepTryingGetPlayer() {
        _followTarget = new GameObject("CameraTracker").transform;
        GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = _followTarget;

        while (TryGetPlayer()) { yield return null; }
    }

    private bool TryGetPlayer() {
        Player[] players = FindObjectsOfType<Player>();
        if (players != null) {
            foreach (Player player in players)
                if (player.Type == NetworkManagerReference.Instance.PlayersDictionary[NetworkManagerReference.LocalPlayerIDInServer].PlayerType) {
                    _followTarget.parent = player.transform;
                    return false; // Found
                }
        }
        return true; // Keep trying
    }

}
