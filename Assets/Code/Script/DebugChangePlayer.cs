using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMultiplayer.Player;
using ProjectMultiplayer.Connection;
using ProjectMultiplayer.UI;

public class DebugChangePlayer : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private PlayersPrefabList _playersPrefabList;
    private Hud _hud;
    private SpawnAnchor _spawnAnchor;
    private CameraControl _cameraControl;

    private void Awake()
    {
        _spawnAnchor = FindObjectOfType<SpawnAnchor>();
        _cameraControl = FindObjectOfType<CameraControl>();
        _hud = FindObjectOfType<Hud>();
    }

    public void ChangeCharacter(int index)
    {
        if (_spawnAnchor)
        {
            Player temp = FindObjectOfType<Player>();
            NetworkManagerReference.Instance.NetworkRunner.Despawn(temp.Object);
            Transform obj = NetworkManagerReference.Instance.NetworkRunner.Spawn(_playersPrefabList.GetPlayerPrefab((NetworkManager.PlayerType)index),
                _spawnAnchor.GetSpawnPosition((NetworkManager.PlayerType)index),
                Quaternion.identity,
                NetworkManagerReference.Instance.PlayersDictionary[NetworkManagerReference.LocalPlayerIDInServer].PlayerRef).GetComponent<Transform>();
            if (_hud) _hud.UpdateIcons((NetworkManager.PlayerType)index);
            if (_cameraControl) _cameraControl.ResetCameraTracking(obj);
        }
        else
        {
            Debug.LogWarning("Cant find SpawnAnchor in scene");
        }
    }
#endif
}