using UnityEngine;

public class SpawnAnchor : MonoBehaviour {

    [SerializeField] private NetworkManager.PlayerType _playerType;
    public NetworkManager.PlayerType PlayerType { get { return _playerType; } }

}
