using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnchor : MonoBehaviour {

    [SerializeField] private NetworkManager.PlayerType _playerType;
    public NetworkManager.PlayerType PlayerType { get { return _playerType; } }

}
