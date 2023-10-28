using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOptionButton : MonoBehaviour
{
    [SerializeField] private NetworkManager.PlayerType _playerType;
    [HideInInspector] public MainScreen MainScreen;
    [HideInInspector]
    public Image Image
    {
        get
        {
            if (!Image) Image = GetComponent<Image>();
            return Image;
        }
        private set { }
    }

    [HideInInspector]
    public UnityEngine.UI.Button Button
    {
        get
        {
            if (!Button) Button = GetComponent<UnityEngine.UI.Button>();
            return Button;
        }
        private set { }
    }

    public void UpdateCharacterSelected()
    {
        MainScreen.Rpc_UpdatePlayerTypeUI(NetworkManager.Instance.NetworkRunner.LocalPlayer.PlayerId, _playerType);
    }
}
