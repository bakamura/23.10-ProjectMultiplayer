using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using ProjectMultiplayer.Player;
using UnityEngine.InputSystem;
using ProjectMultiplayer.Connection;
using UnityEngine.UI;

namespace ProjectMultiplayer.UI
{
    public class CutsceneUI : NetworkBehaviour
    {
        [SerializeField] private Image _skipUIIcon;
        [SerializeField] private GameObject _skipText;
        [SerializeField] private string _levelToOpen;
        [SerializeField] private SkipIconData[] _skipIcons;
        [Networked(OnChanged = nameof(OnSkipRequestUpdate), OnChangedTargets = OnChangedTargets.StateAuthority)] private byte _currentSkipRequests { get; set; }
        private InputAction _skipInput;

        [System.Serializable]
        private struct SkipIconData
        {
            public NetworkManager.PlayerType PlayerType;
            public Sprite Icon;
        }

        private void Awake()
        {
            _skipInput = InitializeInputPlayer.Instance.PlayerActions.actions["Cancel"];
        }

        private void OnEnable()
        {
            _skipInput.Enable();
        }

        private void OnDisable()
        {
            _skipInput.Disable();
        }

        private void Update()
        {
            if (_skipInput.WasPressedThisFrame() && !_skipUIIcon.enabled)
            {
                _skipText.SetActive(false);
                _skipUIIcon.sprite = GetIconForLocalPlayer(NetworkManagerReference.Instance.PlayersDictionary[NetworkManagerReference.LocalPlayerIDInServer].PlayerType);
                _skipUIIcon.enabled = true;
                TransitionLevel();
            }
        }

        public void TransitionLevel()
        {
            _currentSkipRequests++;
        }

        private Sprite GetIconForLocalPlayer(NetworkManager.PlayerType playerType)
        {
            for(int i = 0; i < _skipIcons.Length; i++)
            {
                if (_skipIcons[i].PlayerType == playerType)
                    return _skipIcons[i].Icon;
            }
            Debug.LogWarning($"There is no icon for the character {playerType}");
            return null;
        }

        private static void OnSkipRequestUpdate(Changed<CutsceneUI> changed)
        {
            if (changed.Behaviour._currentSkipRequests == 3)
            {
                changed.Behaviour.Rpc_FadeUI();                
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void Rpc_FadeUI()
        {
            FadeUIReference.Instance.OnFadeEnd += OnFadeEnd;
            FadeUIReference.Instance.Rpc_ChangeFade(FadeUI.FadeTypes.FADEIN);
        }

        private void OnFadeEnd()
        {
            if (Runner.IsServer)
            {
                FadeUIReference.Instance.OnFadeEnd -= OnFadeEnd;
                NetworkManagerReference.Instance.NetworkRunner.SetActiveScene(_levelToOpen);
            }
        }
    }
}
