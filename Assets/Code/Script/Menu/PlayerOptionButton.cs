using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectMultiplayer.Connection;

namespace ProjectMultiplayer.UI
{
    public class PlayerOptionButton : MonoBehaviour
    {
        [SerializeField] private NetworkManager.PlayerType _playerType;
        [HideInInspector] public UpdatePlayerSelectionScript UpdatePlayerSelectionScript;
        private UnityEngine.UI.Button _button;
        private Image _image;
        public Image Image
        {
            get
            {
                if (_image == null) _image = GetComponent<Image>();
                return _image;
            }
        }

        public UnityEngine.UI.Button Button
        {
            get
            {
                if (!_button) _button = GetComponent<UnityEngine.UI.Button>();
                return _button;
            }
        }

        public void UpdateCharacterSelected()
        {
            UpdatePlayerSelectionScript.Rpc_UpdatePlayerTypeUI(NetworkManagerReference.Instance.NetworkRunner.LocalPlayer, _playerType);
            //UpdatePlayerSelectionScript.Rpc_UpdatePlayerTypeUI(NetworkManagerReference.LocalPlayerIDInServer, _playerType);
        }
    }
}