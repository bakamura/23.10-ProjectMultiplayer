using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System;

public class LobbyOptionUI : MonoBehaviour
{
    public Text _sessionNameText;
    public Text _playerCountText;
    public Button _joinSessionBtn;
    [HideInInspector] private SessionInfo _sessionInfo;
    public Action<SessionInfo> OnJoinSession;

    public void SetSessionInfo(SessionInfo info)
    {
        _sessionInfo = info;

        _sessionNameText.text = info.Name;
        _playerCountText.text = $"{info.PlayerCount}/{info.MaxPlayers}";

        _joinSessionBtn.interactable = info.PlayerCount < info.MaxPlayers;        
    }

    public void OnClick()
    {
        OnJoinSession?.Invoke(_sessionInfo);
    }
}
